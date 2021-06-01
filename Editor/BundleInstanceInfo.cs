using UnityEngine;

namespace SALT.Editor
{
    public struct BundleInstanceInfo : IInstanceInfo, System.IEquatable<BundleInstanceInfo>
    {
        [SerializeField]
        internal IDType idtype;
        [SerializeField]
        internal int id;

        public IDType idType => this.idtype;

        public int ID => this.id;

        public bool Equals(BundleInstanceInfo other) => other.ID == this.ID && other.idType == this.idType;
    }
}
