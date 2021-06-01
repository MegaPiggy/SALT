using SALT.Extensions;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SALT.UI
{
    internal class TimerUI : MonoBehaviour
    {
        private TextMeshProUGUI tmp;
        public static string defaultTime => string.Format("{0:00}:{1:00}.{2:000}", 0, 0, 0) + System.Environment.NewLine + "Deaths: 0" + System.Environment.NewLine + "Bubbas: 0" + System.Environment.NewLine + "Saves Enabled: Yes";

        public void Init()
        {
            tmp = this.GetComponent<TextMeshProUGUI>();
            tmp.text = defaultTime;
            tmp.enabled = false;
        }

        public void Awake() => Init();
        public void Start() => Init();

        public void Update()
        {
            if (!Levels.isMainMenu())
            {
                if (!MainScript.victory)
                {
                    System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(Main.mainScript.levelTime);
                    if (MainScript.language == Language.Japanese)
                        tmp.text = string.Format("{0:00}:{1:00}.{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds) + System.Environment.NewLine + "デス数: " + LevelManager.levelManager.deaths + System.Environment.NewLine + "Bubba: " + LevelManager.levelManager.bubbaTokens.Where(torf => torf == true).Count() + System.Environment.NewLine + "セーブデータ: " + Main.SavesEnabled.ToOnOff(true);
                    else
                        tmp.text = string.Format("{0:00}:{1:00}.{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds) + System.Environment.NewLine + "Deaths: " + LevelManager.levelManager.deaths + System.Environment.NewLine + "Bubba: " + LevelManager.levelManager.bubbaTokens.Where(torf => torf == true).Count() + System.Environment.NewLine + "Saves Enabled: " + Main.SavesEnabled.ToYesOrNo(); 
                }
                tmp.enabled = true;
                return;
            }
            tmp.enabled = false;
            tmp.text = defaultTime;
        }
    }
}
