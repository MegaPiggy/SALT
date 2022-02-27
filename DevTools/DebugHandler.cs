using SALT.Windows;
using SALT.DevTools.DevMenu;
using UnityEngine;
using SALT.Services;

namespace SALT.DevTools
{
	internal class DebugHandler : USingleton<DebugHandler>, IServiceInternal, IService
	{
		internal static DevMenuWindow devWindow;

		protected override void Awake()
		{
			base.Awake();
			
			devWindow = new DevMenuWindow();
			WindowManager.RegisterWindow(devWindow);
		}
		
		private void Update()
		{
			// Opens the Dev Menu
#if OLD_CONSOLE
			if (Input.GetKeyDown(KeyCode.F8) && !devWindow.IsOpen)
            {
				devWindow.pendingClose = false;
				devWindow.Open();
			}
#else

			// LISTENS TO MAIN INPUT
			if (Event.current != null && Event.current.isKey && Event.current.type == EventType.KeyDown)
			{
				// TOGGLES THE WINDOW
				if ((Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command) && Event.current.keyCode == KeyCode.Tab && !Levels.isTitleScreen())
				{
					if (!devWindow.IsOpen)
					{
						devWindow.pendingClose = false;
						devWindow.Open();
					}
				}
			}
#endif
		}
	}
}