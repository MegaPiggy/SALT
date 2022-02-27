using SALT.Diagnostics;
using System;

/// <summary>Contains extension methods for Exceptions</summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Uses Stack Tracing technology to ascertain source information for this exception's stack traces using
    /// Source Database querying and parses the stack trace to add such information.
    /// </summary>
    /// <param name="this">This exception</param>
    /// <returns>The exception's message with the stack trace parsed</returns>
    public static string ParseTraceWithoutName(this Exception @this) => @this.Message + "\n" + StackTracing.ParseStackTrace(@this);

    /// <summary>
    /// Uses Stack Tracing technology to ascertain source information for this exception's stack traces using
    /// Source Database querying and parses the stack trace to add such information.
    /// </summary>
    /// <param name="this">This exception</param>
    /// <returns>The exception's message with the stack trace parsed</returns>
    public static string ParseTrace(this Exception @this) => (@this.Message.Contains("Exception: ") ? @this.Message : @this.GetType().Name + ": " + @this.Message) + "\n" + StackTracing.ParseStackTrace(@this);

    /// <summary>
    /// Uses Stack Tracing technology to ascertain source information for this stack trace using
    /// Source Database querying and parses the stack trace to add such information.
    /// </summary>
    /// <param name="this">This stack trace</param>
    /// <returns>Parsed stack trace </returns>
    public static string Parse(this System.Diagnostics.StackTrace @this) => StackTracing.ParseStackTrace(@this);
}
