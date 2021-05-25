using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SALT
{
    /// <summary>
    /// The logger for the log file
    /// </summary>
    public static class FileLogger
    {
        // THE LOG FILE
        internal static string saltLogFile = Path.Combine(Application.persistentDataPath, "SALT/salt.log");

        private static bool Initialized = false;

        /// <summary>
        /// Initializes the file logger (run this before Console.Init)
        /// </summary>
        internal static void Init()
        {
            if (!Directory.Exists(Path.GetDirectoryName(saltLogFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(saltLogFile));

            if (File.Exists(saltLogFile))
                File.Delete(saltLogFile);

            File.Create(saltLogFile).Close();
            Initialized = true;
        }

        /// <summary>
        /// Logs a info message
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void Log(string message)
        {
            if (!Initialized) return;
            LogEntry(LogType.Log, message);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogWarning(string message)
        {
            if (!Initialized) return;
            LogEntry(LogType.Warning, message);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogError(string message)
        {
            if (!Initialized) return;
            LogEntry(LogType.Error, message);
        }

        private static string TypeToText(LogType logType)
        {
            if (logType == LogType.Error || logType == LogType.Exception)
                return "ERRO";

            return logType == LogType.Warning ? "WARN" : "INFO";
        }

        internal static void LogEntry(LogType logType, string message)
        {
            if (!Initialized) return;
            using (StreamWriter writer = File.AppendText(saltLogFile))
                writer.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{TypeToText(logType)}] {Regex.Replace(message, @"<material[^>]*>|<\/material>|<size[^>]*>|<\/size>|<quad[^>]*>|<b>|<\/b>|<color=[^>]*>|<\/color>|<i>|<\/i>", "")}");
        }
    }
}
