using SALT.Windows;
using UnityEngine;

namespace SALT.DevTools.DevMenu
{
	/// <summary>
	/// Represents a Tab for the Dev Menu
	/// </summary>
	public abstract class DevTab
	{
		//+ PROPERTIES
		/// <summary>The ID of this tab</summary>
		public string ID { get; }

		/// <summary>The title of this tab</summary>
		public abstract string Title { get; }

		/// <summary>The Rect to draw this tab on</summary>
		public Rect Rect { get; internal set; }

		public bool Visible => DevMenuWindow.currentTab == ID;
		
		//+ CONSTRUCTOR
		/// <summary>
		/// Creates a new tab with the given ID
		/// </summary>
		/// <param name="id">The ID to register this tab with</param>
		protected DevTab(string id)
		{
			ID = id;
		}
		
		//+ ACTIONS
		internal void Show()
		{
			OnShow();
		}

		internal void Hide()
		{
			OnHide();
		}
		
		internal virtual void OnShow() { }
		
		internal virtual void OnHide() { }
		
		internal virtual void OnUpdate() { }

		//+ DISPLAY
		internal abstract void DrawTab();
		
		//+ INPUT CONTROL
		internal virtual bool OnProcessInput(EventModifiers mods) => false;
	}
}