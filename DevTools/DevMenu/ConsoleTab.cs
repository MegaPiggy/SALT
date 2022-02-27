using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SALT.Console;
using SALT.Extensions;
using SALT.Windows;
using UnityEngine;

namespace SALT.DevTools.DevMenu
{
	/// <summary>
	/// The tab for the console added by Guu
	/// </summary>
	public class ConsoleTab : DevTab
	{
		//+ CONSTANTS
		private const string COMMAND_CONTROL_NAME = "consoleInput";
		
		private const string SYSTEM_CMD_MENU = "<size=16><b>Console Menu</b></size>";
		private const string USER_CMD_MENU = "<size=16><b>User Buttons</b></size>";

		private const int AUTO_COMPLETE_WIDTH = 200;
		private const string AUTO_COMPLETE_BULLET = "►";
		private const string AUTO_COMPLETE_LABEL = "<b>Auto Complete</b>";

		private static readonly Font CONSOLE_FONT = Font.CreateDynamicFontFromOSFont(new []{ "Lucida Console", "Monaco" }, 13);
		
		//+ VARIABLES
		// Console Structures
		private static Vector2 consoleScroll = Vector2.zero;
		private static Vector2 systemCmdScroll = Vector2.zero;
		private static Vector2 usedCmdScroll = Vector2.zero;
		private static Vector2 textSize = Vector2.zero;
		
		// Console Works
		private static int currHistory = -1;
		
		private static string fullText;
		internal static string input = string.Empty;

		private static bool moveCursor;
		private static bool removeLastSpace;

		// Auto Complete Structures
		private static Vector2 acScroll = Vector2.zero;
		private static Rect acRect;
		
		// Auto Complete Works
		private static int completeIndex;
		private static bool autoComplete;
		private static string acSelection;
		private static string lastInput;
		private static string oldInput;
		private static bool justActivated = false;

		private static List<string> acCache = new List<string>();

		//+ PROPERTIES
		/// <summary>The title of this tab</summary>
		public override string Title => "Console";
		
		private static TextEditor TxtEditor => GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl) as TextEditor;

		//+ CONSTRUCTOR
		/// <summary>Creates a new console tab</summary>
		public ConsoleTab() : base("consoleTab")
		{
			Console.Console.Log("Console Window running.");
			Console.Console.Log("Use command '<color=#77DDFF>help</color>' for a list of all commands");
			Console.Console.Log("Use command '<color=#77DDFF>mods</color>' for a list of all mods loaded");
			Console.Console.Log("You can also check the menu on the right");
		}
		
		//+ ACTIONS
		internal override void OnShow()
		{
			autoComplete = false;
			currHistory = -1;
			completeIndex = 0;

			oldInput = null;
			justActivated = false;
			lastInput = null;
			moveCursor = false;
			removeLastSpace = false;
		}

		internal override void OnHide()
		{
			autoComplete = false;
			currHistory = -1;
			completeIndex = 0;

			oldInput = null;
			justActivated = false;
			lastInput = null;
			moveCursor = false;
			removeLastSpace = false;
		}

		internal override void OnUpdate()
		{
			//& Adapt the text being displayed
			if (Console.Console.updateConsole)
			{
				fullText = string.Empty;
				foreach (string t in Console.Console.lines) fullText += $"\n{t}";
				fullText = fullText.TrimStart('\n');

				textSize = WindowManager.Skin.textArea.CalcSize(new GUIContent(fullText));
				consoleScroll.y = textSize.y; 
				Console.Console.updateConsole = false;
			}
			CheckChanges();
			
			//& Normalizes the current history
			if (string.IsNullOrWhiteSpace(input) && currHistory > -1) currHistory = -1;
			
			//& Adapts the auto complete rect
			if (autoComplete) acRect = new Rect(acRect.x, 
												DebugHandler.devWindow.Rect.height - DebugHandler.devWindow.Rect.height * 0.3f - 32, 
												AUTO_COMPLETE_WIDTH, 
												DebugHandler.devWindow.Rect.height * 0.3f);
			
			//& Fixes input on special cases
			if (string.IsNullOrWhiteSpace(input)) input = string.Empty;

			if (!removeLastSpace) return;
			input = input.Contains(" ") ? input.Substring(0, input.LastIndexOf(' ')) : input;
			removeLastSpace = false;
		}

		//+ DISPLAY
		internal override void DrawTab()
		{
			// Keeps the input box focused
			GUI.FocusControl(COMMAND_CONTROL_NAME);
			
			//& Draws the console side
			GUILayout.BeginVertical(GUILayout.Width(DebugHandler.devWindow.Rect.width * 0.8f));
			DrawLogDisplay();
			GUILayout.EndVertical();
			
			//& Draws the menu side
			// Fixes the weird margins
			GUIStyle.none.margin = new RectOffset(0, 0, 4, 4);
			
			// Draws the menu panel
			GUILayout.BeginVertical();
			DrawMenuDisplay();
			GUILayout.EndVertical();
			
			//& Moves the cursor
			if (moveCursor)
			{
				TxtEditor.MoveTextEnd();
				moveCursor = false;
				autoComplete = false;
			}

			//& Draws the auto complete
			if (autoComplete)
				DrawAutoComplete();
		}

		private static void DrawLogDisplay()
		{
			// Set the console font
			Font font = GUI.skin.font;
			GUI.skin.font = CONSOLE_FONT;

			//& Draws the log display
			// Prevents scrolling if auto complete is open
			bool scroll = Event.current.IgnoreIf(EventType.ScrollWheel, autoComplete && acRect.Contains(Event.current.mousePosition));
			
			// Makes sure the text is displayed inside a scroll view
			GUILayout.BeginHorizontal(GUI.skin.textArea);
			consoleScroll = GUILayout.BeginScrollView(consoleScroll, GUILayout.ExpandHeight(true));
			GUILayout.Label(fullText, GUIStyle.none);
			GUILayout.EndScrollView();
			GUILayout.EndHorizontal();
			
			// Reverts scrolling state if changed
			if (scroll) Event.current.Restore();
			
			// Draws the command line
			GUI.SetNextControlName(COMMAND_CONTROL_NAME);
			input = GUILayout.TextField(input, GUILayout.Height(20), GUILayout.ExpandWidth(true));
			
			if (input.EndsWith(" ") && justActivated)
			{
				oldInput = null;
				justActivated = false;
			}

			// Revert the font to normal
			GUI.skin.font = font;
		}
		
		private static void DrawMenuDisplay()
		{
			//& Draws the menu display
			// Draws the buttons from the system
			GUILayout.BeginVertical(GUI.skin.textArea, GUILayout.ExpandHeight(true));
			GUILayout.Label(SYSTEM_CMD_MENU, GUI.skin.GetStyle("centerLabel"), GUILayout.ExpandWidth(true), GUILayout.Height(20));
			systemCmdScroll = GUILayout.BeginScrollView(systemCmdScroll, GUILayout.Height(DebugHandler.devWindow.Rect.height * 0.3f));

			var buttons = Console.Console.cmdButtons.Where(kvp => !kvp.Key.StartsWith("user.")).ToDictionary<string, ConsoleButton>();
			var userButtons = Console.Console.cmdButtons.Where(kvp => kvp.Key.StartsWith("user.")).ToDictionary<string, ConsoleButton>();

			foreach (ConsoleButton button in buttons.Values)
			{
				if (GUILayout.Button(button.Text)) SALT.Console.Console.ExecuteCommand(button.Command, true);
			}
			
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			
			// Draws the buttons from the users
			GUILayout.BeginVertical(GUI.skin.textArea, GUILayout.ExpandHeight(true));
			GUILayout.Label(USER_CMD_MENU, GUI.skin.GetStyle("centerLabel"), GUILayout.ExpandWidth(true), GUILayout.Height(20));
			usedCmdScroll = GUILayout.BeginScrollView(usedCmdScroll, GUILayout.ExpandHeight(true));

			foreach (ConsoleButton button in userButtons.Values)
			{
				if (GUILayout.Button(button.Text)) SALT.Console.Console.ExecuteCommand(button.Command, true);
			}
			
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
		}

		private static void DrawAutoComplete()
		{
			try
			{
				acRect.x = TxtEditor.graphicalCursorPos.x - DebugHandler.devWindow.Rect.x + AUTO_COMPLETE_WIDTH - 25;

				// Draws the scroll view
				GUILayout.BeginArea(acRect, GUI.skin.box);
				GUILayout.Label(AUTO_COMPLETE_LABEL, GUI.skin.GetStyle("centerLabel"), GUILayout.ExpandWidth(true), GUILayout.Height(20));
				acScroll = GUILayout.BeginScrollView(acScroll, false, true);

				// Creates a text field with specific configurations
				GUIStyle customField = new GUIStyle(GUI.skin.textField)
				{
					richText = true,
					alignment = TextAnchor.MiddleLeft
				};

				// Draws the auto complete content
				for (int i = 0; i < acCache.Count; i++)
				{
					GUI.backgroundColor = completeIndex == i ? Color.cyan : Color.white;

					string acContent = acCache[i];
					if (acContent.Length < acSelection.Length)
					{
						autoComplete = false;
						continue;
					}

					string text = $" {(completeIndex == i ? AUTO_COMPLETE_BULLET + " " : string.Empty)}" +
								  $"<b><color=#77DDFF>{acContent.Substring(0, acSelection.Length)}</color></b>{acContent.Substring(acSelection.Length)}";

					if (!GUILayout.Button(text, customField, GUILayout.Height(25), GUILayout.Width(AUTO_COMPLETE_WIDTH - 35)))
						continue;

					input = input.Substring(0, input.Length - acSelection.Length) + acCache[i];

					moveCursor = true;
					autoComplete = false;
					break;
				}

				// Ends the scroll view
				GUILayout.EndScrollView();
				GUILayout.EndArea();
			}
			catch(System.Exception e)
			{
				Console.Console.LogException(e);
			}
		}
		
		//+ INPUT CONTROL
		internal override bool OnProcessInput(EventModifiers mods)
		{
			if (Event.current == null) return false;

			switch (Event.current.keyCode)
			{
				// Closes auto complete if open and ESC is pressed
				case KeyCode.Escape when autoComplete:
					autoComplete = false;

					return true;

				// Submits the input of the console if ENTER/RETURN is pressed
				case KeyCode.Return:
				case KeyCode.KeypadEnter:
					SALT.Console.Console.ExecuteCommand(input.TrimEnd(' '));
					input = string.Empty;
					oldInput = null;

					currHistory = -1;
					completeIndex = 0;
					autoComplete = false;

					return true;
				
				// Toggles the auto complete if CTRL+Space is pressed (CMD for mac)
				case KeyCode.Space when mods == EventModifiers.Control || mods == EventModifiers.Command:
					CheckChanges();

					removeLastSpace = true;

					if (acCache.Count == 0) return true;

					autoComplete = true;

					justActivated = true;
					oldInput = null;

					return true;
				
				// Executes auto complete when TAB is pressed without modifiers
				case KeyCode.Tab when mods == EventModifiers.None && autoComplete:
					input = input.Substring(0, input.Length - acSelection.Length) + acCache[completeIndex];
					moveCursor = true;
					return true;
				
				// Changes the history if the UP ARROW was pressed
				case KeyCode.UpArrow when !autoComplete && SALT.Console.Console.history.Count > 0:
					currHistory = currHistory == -1 ? SALT.Console.Console.history.Count - 1 : currHistory > 0 ? currHistory - 1 : currHistory;
					input = SALT.Console.Console.history[currHistory];

					moveCursor = true;
					return true;
				
				// Changes the history if the DOWN ARROW was pressed
				case KeyCode.DownArrow when !autoComplete && SALT.Console.Console.history.Count > 0:
					if (currHistory == -1) return true;

					currHistory = currHistory < SALT.Console.Console.history.Count - 1 ? currHistory + 1 : -1;
					input = currHistory == -1 ? string.Empty : SALT.Console.Console.history[currHistory];
					
					moveCursor = true;
					return true;
				
				// Changes the auto complete selection if UP ARROW was pressed
				case KeyCode.UpArrow when autoComplete:
					completeIndex = completeIndex == 0 ? acCache.Count - 1 : completeIndex - 1;
					acScroll.y = 25 * completeIndex;

					return true;

				// Changes the auto complete selection if DOWN ARROW was pressed
				case KeyCode.DownArrow when autoComplete:
					completeIndex = completeIndex == acCache.Count - 1 ? 0 : completeIndex + 1;
					acScroll.y = 25 * completeIndex;

					return true;

				// Changes the auto complete selection if DOWN ARROW was pressed
				case KeyCode.DownArrow when autoComplete:
					completeIndex = completeIndex == acCache.Count - 1 ? 0 : completeIndex + 1;
					acScroll.y = 25 * completeIndex;

					return true;

				// Anything else
				default:

					// TRIGGER AUTO COMPLETE
					if (Event.current.keyCode != KeyCode.None && Event.current.keyCode != KeyCode.Space && Event.current.keyCode != KeyCode.Escape)
					{
						if ((!input.Equals(oldInput) || !input.Equals(lastInput) || string.IsNullOrEmpty(input)) && (mods == EventModifiers.None || mods == EventModifiers.Shift))
							autoComplete = string.IsNullOrEmpty(input);
					}

					// FIXES SPACE && BACKSPACE
					if (Event.current.keyCode == KeyCode.Space && mods == EventModifiers.None)
						autoComplete = Regex.Matches(input, "\"").Count % 2 == 0;

					return false;
			}
		}
		
		//+ HELPERS
		private static void CheckChanges()
		{
			if (lastInput?.Equals(input) ?? false) return;
			lastInput = input;
			
			bool spaces = input.Contains(" ");
			string cmd = spaces ? input.Substring(0, input.IndexOf(' ')) : input;
			if (TxtEditor.hasSelection)
				cmd = string.Empty;

			if (!spaces || TxtEditor.hasSelection)
			{
				acCache.Clear();
				acSelection = cmd;

				foreach (string id in SALT.Console.Console.commands.Keys)
				{
					if (id.ToLowerInvariant().StartsWith(cmd.ToLowerInvariant())) acCache.Add(id);
				}

				if (!cmd.Equals(oldInput))
					oldInput = cmd;
			}
			else
			{
				if (!SALT.Console.Console.commands.ContainsKey(cmd))
					return;

				string[] args = SALT.Console.Console.StripArgs(input, true);
				int count = args.Length;
				string last = args[count - 1];

				List<string> autoC = Console.Console.commands[cmd].GetAutoComplete(count - 1, last);

				if (autoC == null || autoC.Count == 0 || Regex.Matches(input, "\"").Count % 2 != 0)
					autoComplete = false;

				acSelection = last;
				acCache.Clear();
				acCache.RemoveAll(s => true); // .Clear() gets broken sometimes when you dynamically load an Assembly, so do the same another way

				if (autoComplete)
				{
					if (!Console.Console.commands[cmd].AllowSpaces)
						autoC?.RemoveAll(s => s.Contains(" "));

					if (!input.Equals(oldInput))
					{
						foreach (string arg in autoC)
						{
							if (arg.ToLowerInvariant().StartsWith(last.ToLowerInvariant()))
								acCache.Add(arg);
						}

						oldInput = input;
					}
				}
			}

			if (acCache.Count == 0)
			{
				autoComplete = false;
				return;
			}
			
			if (completeIndex >= acCache.Count) 
				completeIndex = acCache.Count - 1;
		}
	}
}