using UnityEngine;

namespace SALT.DevTools.DevMenu
{
	internal class ObjectComponent
	{
		public Component Component { get; }
		public bool IsUnfolded { get; set; }
		public ObjectInspector Inspector { get; }

		internal ObjectComponent(Component component)
		{
			Component = component;
			IsUnfolded = true;
			Inspector = new ObjectInspector(component);
		}
	}
}