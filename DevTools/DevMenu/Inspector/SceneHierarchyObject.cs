using SALT.Extensions;
using UnityEngine;

namespace SALT.DevTools.DevMenu
{
	internal class SceneHierarchyObject
	{
		public string FullName { get; }
		public SceneHierarchyObject Parent { get; }
		public Object Object { get; }
		public GameObject GameObject => Object as GameObject;
		public ScriptableObject ScriptableObject => Object as ScriptableObject;
		public ObjectInspector SOInspector { get; }
		public int ChildIndent { get; }
		public bool IsUnfolded { get; set; }
		public bool IsHidden { get; set; }

		internal SceneHierarchyObject(Object @object, int indent)
		  : this(null, @object, indent)
		{
		}

		internal SceneHierarchyObject(SceneHierarchyObject parent, Object @object, int indent)
		{
			this.Parent = parent;
			this.Object = @object;
			this.ChildIndent = indent;
			if (@object is ScriptableObject sObject)
			{
				this.FullName = sObject.name;
				this.SOInspector = new ObjectInspector(sObject);
			}
			else if (@object is GameObject gameObject)
			{
				this.FullName = gameObject.GetFullName();
				this.SOInspector = null;
			}
		}

		internal bool HasChildren() => this.Object is GameObject gameObject && gameObject.transform.childCount > 0;
	}
}