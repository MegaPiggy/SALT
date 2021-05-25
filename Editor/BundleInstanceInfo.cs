using UnityEngine;

namespace SALT.Editor
{
    public struct BundleInstanceInfo : IInstanceInfo
    {
        [SerializeField]
        public IDType idtype;
        [SerializeField]
        public int id;

        public IDType idType => this.idtype;

        public int ID => this.id;
    }
}
