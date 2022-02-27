using System.Collections.Generic;
using System.Linq;
using SALT.Console;
using UnityEngine;

namespace SALT.DevTools.DevMenu
{
	/// <summary>
	/// The tab that contains a cheat menu that allows the player to manipulate certain
	/// aspects of the game
	/// </summary>
	public class CheatMenuTab : DevTab
	{
		//+ CONSTANTS

		//+ VARIABLES
		// Cheat Menu Structures

		//+ PROPERTIES
		/// <summary>The title of this tab</summary>
		public override string Title => "Cheat Menu";
		
		//+ CONSTRUCTOR
		/// <summary>Creates a new console tab</summary>
		public CheatMenuTab() : base("cheatMenuTab") { }
		
		//+ ACTIONS
		internal override void OnShow()
		{
		}

		internal override void OnHide()
		{
		}

		internal override void OnUpdate()
		{
			
		}

		//+ DISPLAY
		internal override void DrawTab()
		{
			//& Draws the hierarchy side
			GUILayout.BeginVertical(GUILayout.Width(DebugHandler.devWindow.Rect.width * 0.4f));
			
			GUILayout.EndVertical();
			
			//& Draws the inspector side
			// Fixes the weird margins
			GUIStyle.none.margin = new RectOffset(0, 0, 4, 4);
			
			// Draws the menu panel
			GUILayout.BeginVertical();
			
			GUILayout.EndVertical();
		}

		//+ INPUT CONTROL
		internal override bool OnProcessInput(EventModifiers mods)
		{
			return false;
		}
		
		//+ HELPERS
		
	}
}