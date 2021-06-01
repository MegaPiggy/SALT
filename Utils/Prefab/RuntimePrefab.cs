using UnityEngine;

namespace SALT.Utils.Prefab
{
    public class RuntimePrefab : MonoBehaviour
    {
        public bool ShouldEnableOnInstantiate = true;
        private bool AvoidDestroy = true;
    }
}
