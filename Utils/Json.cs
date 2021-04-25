using System;
using System.Collections.Generic;
using System.IO;
using System.Dynamic;
using System.Text.Json;

namespace SAL.Utils
{
	/// <summary>Provides methods for working with data in JavaScript Object Notation (JSON) format.</summary>
	public static class Json
	{
		/// <summary>Converts data in JavaScript Object Notation (JSON) format into a data object.</summary>
		/// <returns>The JSON-encoded data converted to a data object.</returns>
		/// <param name="value">The JSON-encoded string to convert.</param>
		public static ExpandoObject Decode(string value)
		{
			return JsonSerializer.Deserialize<ExpandoObject>(value);
		}

		/// <summary>Converts data in JavaScript Object Notation (JSON) format into the specified strongly typed data list.</summary>
		/// <returns>The JSON-encoded data converted to a strongly typed list.</returns>
		/// <param name="value">The JSON-encoded string to convert.</param>
		/// <typeparam name="T">The type of the strongly typed list to convert JSON data into.</typeparam>
		public static T Decode<T>(string value)
		{
			var generatedType = JsonSerializer.Deserialize<T>(value);
			return (T)Convert.ChangeType(generatedType, typeof(T));
		}
	}
}
