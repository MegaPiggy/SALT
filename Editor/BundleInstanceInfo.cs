using UnityEngine;

namespace SALT.Editor
{
    public struct BundleInstanceInfo : IInstanceInfo, System.IEquatable<BundleInstanceInfo>
    {
        #pragma warning disable 0649
        [SerializeField]
        internal IDType idtype;
        [SerializeField]
        internal int id;
        #pragma warning restore 0649

        public IDType idType => this.idtype;

        public int ID => this.id;

        public bool Equals(BundleInstanceInfo other) => other.ID == this.ID && other.idType == this.idType;
    }
}
