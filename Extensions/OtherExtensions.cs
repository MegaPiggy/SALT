using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

namespace SALT.Extensions
{
	public static class OtherExtensions
	{
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
		public static IEnumerator DelayedAction(this Action action)
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

		public static void LogArray<T>(this T[] toLog)
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

		public static void LogArray<T>(this IList<T> toLog)
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


		/// <summary>
		/// Methods to check if renderer is visible from a certain point.
		/// </summary>
		/// <param name="renderer">Renderer to operate with.</param>
		/// <param name="camera">Camera instance tha will be used for calculation.</param>
		/// <returns></returns>
		public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
		{
			var planes = GeometryUtility.CalculateFrustumPlanes(camera);
			return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
		}

		/// <summary>
		/// Renderer Bounds of the renderer. Method also take children in consideration.
		/// </summary>
		/// <param name="renderer">Renderer to operate with.</param>
		/// <returns>Calculated renderer bounds.</returns>
		public static Bounds GetRendererBounds(this Renderer renderer)
		{
			return renderer.gameObject.GetRendererBounds();
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

		private static TextAssetTranslation DEFAULT_AT = new TextAssetTranslation() { language = (Language)(-1), text = new TextAsset(string.Empty) };
		public static string GetText(this TextArea textArea, Language language)
		{
			if (textArea == null)
				throw new ArgumentNullException(nameof(textArea));
			if (textArea.texts == null || textArea.texts.Count == 0)
			{
				Console.Console.LogError("No translations in text area.");
				textArea.texts = new List<TextAssetTranslation>();
				return string.Empty;
			}
			return textArea.texts.FirstOrDefault(tat => tat.language == language, DEFAULT_AT).text.text;
		}
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


		private static Translation DEFAULT = new Translation() { language = (Language)(-1), text = string.Empty };
		public static string GetText(this TextLanguageScript textLanguageScript, Language language)
		{
			if (textLanguageScript == null)
				throw new ArgumentNullException(nameof(textLanguageScript));
			if (textLanguageScript.translations == null || textLanguageScript.translations.Count == 0)
			{
				Console.Console.LogError("No translations in language script.");
				textLanguageScript.translations = new List<Translation>();
				return string.Empty;
			}
			Translation test = textLanguageScript.translations.FirstOrDefault(translation => translation.language == language, DEFAULT);
			return test.text;
		}
		public static string GetEnglishText(this TextLanguageScript textLanguageScript) => textLanguageScript.GetText(Language.English);
		public static string GetJapaneseText(this TextLanguageScript textLanguageScript) => textLanguageScript.GetText(Language.Japanese);

		public static void Log(this string str, bool logToFile = true) => Console.Console.Log(str, logToFile);

		public static void EditLabels(this PauseOption pauseOption, string text) => pauseOption.labels = new TranslationCollection { translations = text.ToTranslations() };
		public static void EditLabels(this PauseOption pauseOption, string text, string jtext) => pauseOption.labels = new TranslationCollection { translations = text.ToTranslations(jtext) };
		public static void AddMethod(this PauseOption pauseOption, UnityAction state) => pauseOption.methods.AddListener(state);
		public static void RemoveAllMethods(this PauseOption pauseOption) => pauseOption.methods.RemoveAllListeners();

		public static void AddMethod(this GenericButtonScript genericButton, UnityAction state) => genericButton.methods.AddListener(state);
		public static void RemoveMethod(this GenericButtonScript genericButton, UnityAction state) => genericButton.methods.RemoveListener(state);
		public static void RemoveAllMethods(this GenericButtonScript genericButton) => genericButton.methods.RemoveAllListeners();


		public static void SetText(this TranslationCollection translationCollection, Language language, string text) => translationCollection.translations[translationCollection.translations.IndexOf(translationCollection.translations.FirstOrDefault(translation => translation.language == language))] = new Translation { language = language, text = text };
		public static string GetEnglishText(this TranslationCollection translationCollection) => translationCollection.GetText(Language.English);
		public static string GetJapaneseText(this TranslationCollection translationCollection) => translationCollection.GetText(Language.Japanese);
	}
}
