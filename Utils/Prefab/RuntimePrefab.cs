using UnityEngine;

namespace SALT.Utils.Prefab
{
    public class RuntimePrefab : MonoBehaviour
    {
        public bool ShouldEnableOnInstantiate = true;
        #pragma warning disable 0414
        private bool AvoidDestroy = true;
        #pragma warning restore 0414
    }
}
