using System.Linq;
using SALT.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SALT.UI
{
    /// <summary>
    /// Class used to generate an error message 
    /// </summary>
    internal class ErrorUI : MonoBehaviour
    {
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        } 
        internal static void CreateError(string message, bool doAbort = true)
        {
            GameObject versionObject = UnityEngine.Object.FindObjectsOfType<RectTransform>().FirstOrDefault(tmp => tmp.gameObject.name == "Version").gameObject;
            GameObject errorUI = versionObject.CloneInstance();
            errorUI.name = "errorUI";
            errorUI.AddComponent<UI.ErrorUI>();
            GameObject errorCanvas = new GameObject("errorCanvas", typeof(RectTransform));
            var canvas = errorCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
            var canvasScaler = errorCanvas.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0;
            canvasScaler.referencePixelsPerUnit = 100;
            errorCanvas.AddComponent<CanvasRenderer>();
            RectTransform ecRT = errorCanvas.GetComponent<RectTransform>();
            ecRT.anchorMin = new Vector2(0, 0);
            ecRT.anchorMax = new Vector2(0, 0);
            ecRT.pivot = new Vector2(0.5f, 0.5f);
            GameObject errorBackground = new GameObject("errorBackground", typeof(RectTransform), typeof(CanvasRenderer));
            RectTransform ebRT = errorBackground.GetComponent<RectTransform>();
            ebRT.anchorMin = new Vector2(0, 0);
            ebRT.anchorMax = new Vector2(1, 1);
            ebRT.pivot = new Vector2(0.5f, 0.5f);
            errorBackground.layer = errorUI.layer;
            var img = errorBackground.AddComponent<Image>();
            img.color = new Color32(14, 14, 14, byte.MaxValue);
            errorCanvas.AddChild(errorBackground, false);
            versionObject.transform.parent.gameObject.AddChild(errorUI, false);
            RectTransform vRT = versionObject.GetComponent<RectTransform>();
            RectTransform errRT = errorUI.GetComponent<RectTransform>().GetCopyOf(vRT);
            errRT.SetPivotAndAnchors(new Vector2(0.5f, 0.5f));
            errRT.localPosition = vRT.localPosition;
            errRT.localScale = vRT.localScale;
            errRT.anchoredPosition = vRT.anchoredPosition;
            errRT.sizeDelta = vRT.sizeDelta;
            //errRT.offsetMin = vRT.offsetMin;
            //errRT.offsetMax = vRT.offsetMax;
            errRT.anchoredPosition3D = vRT.anchoredPosition3D;
            errRT.localPosition = vRT.localPosition;
            errRT.anchorMin = new Vector2(0, 0);
            errRT.anchorMax = new Vector2(1, 1);
            errRT.offsetMax = new Vector2(-50, -100);
            errRT.offsetMin = new Vector2(50, 30);
            var txt = errorUI.GetComponent<TextMeshProUGUI>();
            txt.text = message;
            txt.alignment = TextAlignmentOptions.Top;
            txt.enableWordWrapping = true;
            errorCanvas.AddChild(errorUI);
        }
    }
}
