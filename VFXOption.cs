using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SALT
{
    public class VFXOption : MonoBehaviour
    {
        private PauseOption po;
        private Language currentLanguage;

        private void Start()
        {
            this.po = this.GetComponent<PauseOption>();
            this.SetStrings();
            this.po.currentSelection = PlayerPrefs.GetInt("vfx", 0);
            this.SetVFX();
        }

        private void Update()
        {
            if (this.currentLanguage == MainScript.language)
                return;
            this.SetStrings();
        }

        internal void SetStrings()
        {
            List<string> stringList = new List<string>();
            if (MainScript.language == Language.Japanese)
            {
                stringList.Add("オン");
                stringList.Add("オフ");
            }
            else
            {
                stringList.Add("On");
                stringList.Add("Off");
            }
            this.po.selectionStrings = stringList;
            this.currentLanguage = MainScript.language;
        }

        public void SetVFX()
        {
            Patches.PoundVFXPatch.SetEnabled(this.po.currentSelection == 0);
            PlayerPrefs.SetInt("vfx", this.po.currentSelection);
        }

        private void Awake()
        {
        }

        private void OnDestroy()
        {
        }
    }
}
