using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SAL.Editor
{
    [CreateAssetMenu(menuName = "SAL/Replacers/FieldReplacer")]
    public class BundleFieldReplacer : ScriptableObject, IFieldReplacer
    {
#pragma warning disable 0649
        [SerializeField]
        private BundleInstanceInfo instanceInfo;
        [SerializeField]
        private bool replaceInChildren;
        [SerializeField]
        private List<BundleFieldReplacement> fieldReplacements;
#pragma warning restore 0649

        public IInstanceInfo InstanceInfo => (IInstanceInfo)this.instanceInfo;

        public bool ReplaceInChildren => this.replaceInChildren;

        public ICollection<IFieldReplacement> FieldReplacements => (ICollection<IFieldReplacement>)this.fieldReplacements.Select<BundleFieldReplacement, IFieldReplacement>((Func<BundleFieldReplacement, IFieldReplacement>)(x => (IFieldReplacement)x)).ToList<IFieldReplacement>();
    }
}
