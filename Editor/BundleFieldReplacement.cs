using System;
using System.Reflection;
using UnityEngine;

namespace SAL.Editor
{
    [Serializable]
    public struct BundleFieldReplacement : IFieldReplacement
    {
        [SerializeField]
        public string fieldToReplaceType;
        [SerializeField]
        public string fieldToReplaceFieldName;
        [SerializeField]
        public string replacementSourceType;
        [SerializeField]
        public string replacementSourceFieldName;

        public bool TryResolveSource(out FieldInfo field) => this.Resolve(this.replacementSourceType, this.replacementSourceFieldName, out field);

        public bool TryResolveTarget(out FieldInfo field) => this.Resolve(this.fieldToReplaceType, this.fieldToReplaceFieldName, out field);

        private bool Resolve(string typeName, string fieldName, out FieldInfo field)
        {
            Type type = Type.GetType(typeName + ", Assembly-CSharp");
            FieldInfo field1 = null;
            int num;
            if (type != null)
            {
                field1 = type.GetField(fieldName);
                num = field1 != null ? 1 : 0;
            }
            else
                num = 0;
            if (num != 0)
            {
                field = field1;
                return true;
            }
            field = (FieldInfo)null;
            return false;
        }
    }
}
