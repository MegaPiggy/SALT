﻿using SALT.Extensions;
using UnityEngine;

namespace SALT.UI
{
    internal class ModListUI : MonoBehaviour
    {
        private GameObject modList => this.gameObject;
        private TextArea modArea => this.gameObject.GetComponentInChildren<TextArea>();
        internal bool NoModsYet = true;
        private string NewLine => this.NoModsYet ? "  " : System.Environment.NewLine + "  ";
        private const string ModFormat = "<size=25>[Name] v[Version] </size><size=22>--- [Author] </size><size=20>--- [Description]</size>";
        private string ModString(string name, string description, ModInfo.ModVersion version, string author) => NewLine + ModFormat.Replace("[Name]", name).Replace("[Version]", $"{version}").Replace("[Author]", author).Replace("[Description]",description);

        public void Awake()
        {
        }

        public void Start()
        {
            foreach (var mod in ModLoader.GetMods())
            {
                AddModInfo(mod.ModInfo);
                NoModsYet = false;
            }
        }

        public void AddModInfo(ModInfo info)
        {
            modArea.Edit(modArea.text.text + ModString(info.Name, info.Description, info.Version, info.Author));
        }
    }
}
