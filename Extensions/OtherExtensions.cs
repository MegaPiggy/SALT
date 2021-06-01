using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SALT.Extensions
{
    public static class OtherExtensions
    {
        public static Vector3 manualWorldToScreenPoint(this Camera cam, Vector3 wp)
        {
            // calculate view-projection matrix
            Matrix4x4 mat = cam.projectionMatrix * cam.worldToCameraMatrix;

            // multiply world point by VP matrix
            Vector4 temp = mat * new Vector4(wp.x, wp.y, wp.z, 1f);

            if (temp.w == 0f)
            {
                // point is exactly on camera focus point, screen point is undefined
                // unity handles this by returning 0,0,0
                return Vector3.zero;
            }
            else
            {
                // convert x and y from clip space to window coordinates
                temp.x = (temp.x / temp.w + 1f) * .5f * cam.pixelWidth;
                temp.y = (temp.y / temp.w + 1f) * .5f * cam.pixelHeight;
                return new Vector3(temp.x, temp.y, wp.z);
            }
        }

        public static bool IsWorldPointOnScreen(this Camera camera, Vector3 point)
        {
            var position = camera.WorldToViewportPoint(point);
            //float distX = Vector3.Distance(new Vector3(Screen.width / 2, 0f, 0f), new Vector3(position.x, 0f, 0f));
            //float distY = Vector3.Distance(new Vector3(0f, Screen.height / 2, 0f), new Vector3(0f, position.y, 0f));
            //bool isVisible = !(distX > Screen.width / 2 || distY > Screen.height / 2);
            return position.z > 0 && position.x > 0 && position.x < 1 && position.y > 0 && position.y < 1;
        }

        public static bool IsWorldPointInViewport(this Camera camera, Vector3 point)
        {
            var position = camera.WorldToViewportPoint(point);
            return position.x > 0 && position.y > 0;
        }

        /// <summary>
        /// Swap Rigidbody IsKinematic and DetectCollisions
        /// </summary>
        /// <param name="body"></param>
        /// <param name="state"></param>
        public static void SetBodyState(this Rigidbody body, bool state)
        {
            body.isKinematic = !state;
            body.detectCollisions = state;
        }

        public static bool HasMethod(this object target, string methodName)
        {
            return target.GetType().GetMethod(methodName) != null;
        }

        public static bool HasField(this object target, string fieldName)
        {
            return target.GetType().GetField(fieldName) != null;
        }

        public static bool HasProperty(this object target, string propertyName)
        {
            return target.GetType().GetProperty(propertyName) != null;
        }

        // Toggle layers lock
        //Tools.lockedLayers = 1 << LayerMask.NameToLayer("LayerName"); // Replace the whole value of lockedLayers. 
        //Tools.lockedLayers |= 1 << LayerMask.NameToLayer("LayerName"); // Add a value to lockedLayers. 
        //Tools.lockedLayers &= ~(1 << LayerMask.NameToLayer("LayerName")); // Remove a value from lockedLayers. 
        //Tools.lockedLayers ^= 1 << LayerMask.NameToLayer("LayerName")); // Toggle a value in lockedLayers.


        public static LayerMask ToLayerMask(this int layer)
        {
            return 1 << layer;
        }

        public static bool LayerInMask(this LayerMask mask, int layer)
        {
            return ((1 << layer) & mask) != 0;
        }


        /// <summary>
        /// Convert KeyCode to read-friendly format
        /// Full = "Left Mouse Button", otherwise "LMB"
        /// </summary>
        public static string ToReadableString(this KeyCode key, bool full = false)
        {
            switch (key)
            {
                case KeyCode.Mouse0:
                    return full ? "Left Mouse Button" : "LMB";
                case KeyCode.Mouse1:
                    return full ? "Right Mouse Button" : "RMB";
                case KeyCode.Mouse2:
                    return full ? "Middle Mouse Button" : "MMB";

                default:
                    return Regex.Replace(key.ToString(), "(\\B[A-Z])", " $1");
            }
        }

        public static void DrawArrow(Vector3 from, Vector3 direction, float headLength = 0.25f, float headAngle = 20.0f)
        {
            #if UNITY_EDITOR
            Gizmos.DrawRay(from, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(180 + headAngle, 0, 180 + headAngle) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(180 - headAngle, 0, 180 - headAngle) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(from + direction, right * headLength);
            Gizmos.DrawRay(from + direction, left * headLength);
            #endif
        }

	    public class WaitForUnscaledSeconds : CustomYieldInstruction
	    {
		    private readonly float _waitTime;

		    public override bool keepWaiting
		    {
			    get { return Time.realtimeSinceStartup < _waitTime; }
		    }

		    public WaitForUnscaledSeconds(float secondsToWait)
		    {
			    _waitTime = Time.realtimeSinceStartup + secondsToWait;
		    }
	    }

		/// <summary>
		/// Invoke Action on Delay
		/// </summary>
		public static IEnumerator DelayedAction(float waitSeconds, Action action, bool unscaled = false)
		{
			if (unscaled) yield return new WaitForUnscaledSeconds(waitSeconds);
			else yield return new WaitForSeconds(waitSeconds);

			if (action != null) action.Invoke();
		}

		/// <summary>
		/// Invoke Action on Delay
		/// </summary>
		public static Coroutine DelayedAction(this MonoBehaviour invoker, float waitSeconds, Action action, bool unscaled = false)
		{
			return invoker.StartCoroutine(DelayedAction(waitSeconds, action, unscaled));
		}

		/// <summary>
		/// Invoke Action next frame
		/// </summary>
		public static IEnumerator DelayedAction(Action action)
		{
			yield return null;

			if (action != null) action.Invoke();
		}

		/// <summary>
		/// Invoke Action next frame
		/// </summary>
		public static Coroutine DelayedAction(this MonoBehaviour invoker, Action action)
		{
			return invoker.StartCoroutine(DelayedAction(action));
		}


        #region Log Array

        private static StringBuilder _stringBuilder;

        public static void LogArray<T>(T[] toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            _stringBuilder.Append("Log Array: ").Append(typeof(T).Name).Append(" (").Append(toLog.Length).Append(")\n");
            for (var i = 0; i < toLog.Length; i++)
            {
                _stringBuilder.Append("\n\t").Append(i.ToString().Colored(Colors.brown)).Append(": ").Append(toLog[i]);
            }

            Debug.Log(_stringBuilder.ToString());
        }

        public static void LogArray<T>(IList<T> toLog)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Length = 0;

            var count = toLog.Count;
            _stringBuilder.Append("Log Array: ").Append(typeof(T).Name).Append(" (").Append(count).Append(")\n");

            for (var i = 0; i < count; i++)
            {
                _stringBuilder.Append("\n\t" + i.ToString().Colored(Colors.brown) + ": " + toLog[i]);
            }

            Debug.Log(_stringBuilder.ToString());
        }

        #endregion

        public static void LogColor(Color color)
        {
            Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">████████████</color> = " + color);
        }

        #region Debug Bounds 

        /// <summary>
        /// Draw bounds of Mesh
        /// </summary>
        public static void DrawDebugBounds(MeshFilter mesh, Color color)
        {
#if UNITY_EDITOR
            if (mesh == null) return;
            var renderer = mesh.GetComponent<MeshRenderer>();
            DrawDebugBounds(renderer, color);
#endif
        }

        /// <summary>
        /// Draw bounds of MeshRenderer
        /// </summary>
        public static void DrawDebugBounds(MeshRenderer renderer, Color color)
        {
#if UNITY_EDITOR
            var bounds = renderer.bounds;
            DrawDebugBounds(bounds, color);
#endif
        }

        /// <summary>
        /// Draw bounds of Bounds
        /// </summary>
        public static void DrawDebugBounds(Bounds bounds, Color color)
        {
#if UNITY_EDITOR
            Vector3 v3Center = bounds.center;
            Vector3 v3Extents = bounds.extents;

            var v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z); // Front top left corner
            var v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z); // Front top right corner
            var v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z); // Front bottom left corner
            var v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z); // Front bottom right corner
            var v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z); // Back top left corner
            var v3BackTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z); // Back top right corner
            var v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z); // Back bottom left corner
            var v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z); // Back bottom right corner

            Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, color);
            Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, color);
            Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, color);
            Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, color);

            Debug.DrawLine(v3BackTopLeft, v3BackTopRight, color);
            Debug.DrawLine(v3BackTopRight, v3BackBottomRight, color);
            Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, color);
            Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, color);

            Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, color);
            Debug.DrawLine(v3FrontTopRight, v3BackTopRight, color);
            Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, color);
            Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, color);
#endif
        }

        #endregion


        public static void DrawString(string text, Vector3 worldPos, Color? colour = null)
        {
#if UNITY_EDITOR
            var defaultColor = GUI.color;

            Handles.BeginGUI();
            if (colour.HasValue) GUI.color = colour.Value;
            var view = SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);

            Handles.EndGUI();

            GUI.color = defaultColor;
#endif
        }


        /// <summary>
        /// Draw directional arrow
        /// </summary>
        public static void DrawArrowRay(Vector3 position, Vector3 direction, float headLength = 0.25f, float headAngle = 20.0f)
        {
#if UNITY_EDITOR
            var rightVector = new Vector3(0, 0, 1);
            var directionRotation = Quaternion.LookRotation(direction);

            Debug.DrawRay(position, direction);
            Vector3 right = directionRotation * Quaternion.Euler(0, 180 + headAngle, 0) * rightVector;
            Vector3 left = directionRotation * Quaternion.Euler(0, 180 - headAngle, 0) * rightVector;
            Debug.DrawRay(position + direction, right * headLength);
            Debug.DrawRay(position + direction, left * headLength);
#endif
        }


        /// <summary>
        /// Draw XYZ dimensional RGB cross
        /// </summary>
        public static void DrawDimensionalCross(Vector3 position, float size)
        {
#if UNITY_EDITOR
            var halfSize = size / 2;
            Debug.DrawLine(position.OffsetY(-halfSize), position.OffsetY(halfSize), Color.green, .2f);
            Debug.DrawLine(position.OffsetX(-halfSize), position.OffsetX(halfSize), Color.red, .2f);
            Debug.DrawLine(position.OffsetZ(-halfSize), position.OffsetZ(halfSize), Color.blue, .2f);
#endif
        }

        private static readonly Dictionary<System.Type, string> m_TypeAliases = new Dictionary<System.Type, string>()
          {
            {
              typeof (void),
              "void"
            },
            {
              typeof (byte),
              "byte"
            },
            {
              typeof (sbyte),
              "sbyte"
            },
            {
              typeof (short),
              "short"
            },
            {
              typeof (ushort),
              "ushort"
            },
            {
              typeof (int),
              "int"
            },
            {
              typeof (uint),
              "uint"
            },
            {
              typeof (long),
              "long"
            },
            {
              typeof (ulong),
              "ulong"
            },
            {
              typeof (float),
              "float"
            },
            {
              typeof (double),
              "double"
            },
            {
              typeof (Decimal),
              "decimal"
            },
            {
              typeof (object),
              "object"
            },
            {
              typeof (bool),
              "bool"
            },
            {
              typeof (char),
              "char"
            },
            {
              typeof (string),
              "string"
            },
            {
              typeof (Vector2),
              "Vector2"
            },
            {
              typeof (Vector3),
              "Vector3"
            },
            {
              typeof (Vector4),
              "Vector4"
            }
          };

        public static string GetTypeAlias(this Type type)
        {
            return !m_TypeAliases.TryGetValue(type, out string str) ? type.ToString() : str;
        }

        public static List<TextAssetTranslation> ToAssetTranslations(this string text)
        {
            return new List<TextAssetTranslation>
            {
                new TextAssetTranslation
                {
                    language = Language.English,
                    text = new TextAsset(text),
                },
                new TextAssetTranslation
                {
                    language = Language.Japanese,
                    text = new TextAsset(text),
                }
            };
        }

        public static List<TextAssetTranslation> ToAssetTranslations(this string text, string jtext)
        {
            return new List<TextAssetTranslation>
            {
                new TextAssetTranslation
                {
                    language = Language.English,
                    text = new TextAsset(text),
                },
                new TextAssetTranslation
                {
                    language = Language.Japanese,
                    text = new TextAsset(jtext),
                }
            };
        }

        public static void Edit(this TextArea textArea, string text)
        {
            textArea.texts = text.ToAssetTranslations();
            textArea.SetText();
        }

        public static void Edit(this TextArea textArea, string text, string jtext)
        {
            textArea.texts = text.ToAssetTranslations(jtext);
            textArea.SetText();
        }

        public static string GetText(this TextArea textArea, Language language) => textArea.texts.FirstOrDefault(tat => tat.language == language).text.text;
        public static string GetEnglishText(this TextArea textArea) => textArea.GetText(Language.English);
        public static string GetJapaneseText(this TextArea textArea) => textArea.GetText(Language.Japanese);

        public static List<Translation> ToTranslations(this string text)
        {
            return new List<Translation>
            {
                new Translation
                {
                    language = Language.English,
                    text = text,
                },
                new Translation
                {
                    language = Language.Japanese,
                    text = text,
                }
            };
        }

        public static List<Translation> ToTranslations(this string text, string jtext)
        {
            return new List<Translation>
            {
                new Translation
                {
                    language = Language.English,
                    text = text,
                },
                new Translation
                {
                    language = Language.Japanese,
                    text = jtext,
                }
            };
        }

        public static void Edit(this TextLanguageScript textLanguageScript, string text)
        {
            textLanguageScript.translations = text.ToTranslations();
            textLanguageScript.SetLanguage();
        }

        public static void Edit(this TextLanguageScript textLanguageScript, string text, string jtext)
        {
            textLanguageScript.translations = text.ToTranslations(jtext);
            textLanguageScript.SetLanguage();
        }

        public static string GetText(this TextLanguageScript textLanguageScript, Language language) => textLanguageScript.translations.FirstOrDefault(translation => translation.language == language).text;
        public static string GetEnglishText(this TextLanguageScript textLanguageScript) => textLanguageScript.GetText(Language.English);
        public static string GetJapaneseText(this TextLanguageScript textLanguageScript) => textLanguageScript.GetText(Language.Japanese);

        public static void Log(this string str, bool logToFile = true) => Console.Console.Log(str, logToFile);

        public static void EditLabels(this PauseOption pauseOption, string text) => pauseOption.labels = new TranslationCollection { translations = text.ToTranslations() };
        public static void EditLabels(this PauseOption pauseOption, string text, string jtext) => pauseOption.labels = new TranslationCollection { translations = text.ToTranslations(jtext) };
        public static void AddMethod(this PauseOption pauseOption, UnityAction state) => pauseOption.methods.AddListener(state);
        public static void RemoveAllMethods(this PauseOption pauseOption) => pauseOption.methods.RemoveAllListeners();


        public static void SetText(this TranslationCollection translationCollection, Language language, string text) => translationCollection.translations[translationCollection.translations.IndexOf(translationCollection.translations.FirstOrDefault(translation => translation.language == language))] = new Translation { language = language, text = text };
        public static string GetEnglishText(this TranslationCollection translationCollection) => translationCollection.GetText(Language.English);
        public static string GetJapaneseText(this TranslationCollection translationCollection) => translationCollection.GetText(Language.Japanese);
    }
}
