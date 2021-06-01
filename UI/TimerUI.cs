using SALT.Extensions;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SALT.UI
{
    internal class TimerUI : MonoBehaviour
    {
        private TextMeshProUGUI tmp;
        public static string defaultTime => string.Format("{0:00}:{1:00}.{2:000}", 0, 0, 0);

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
                    tmp.text = string.Format("{0:00}:{1:00}.{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds); 
                }
                tmp.enabled = true;
                return;
            }
            tmp.enabled = false;
            tmp.text = defaultTime;
        }
    }
}
