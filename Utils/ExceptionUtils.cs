using System;

namespace SALT.Utils
{
    /// <summary>An utility class to help with exceptions</summary>
    public static class ExceptionUtils
    {
        /// <summary>
        /// This is made to ignore exceptions so it doesn't fill the log
        /// with unneeded errors. As all of these errors are accounted for,
        /// beforehand.
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="func">Function to execute</param>
        /// <param name="default">The default value to return</param>
        /// <returns>Object returned</returns>
        public static T IgnoreErrors<T>(Func<T> func, T @default = default)
        {
            if (func == null)
                return @default;
            try
            {
                return func();
            }
            catch
            {
                return @default;
            }
        }

        /// <summary>
        /// This is made to ignore exceptions so it doesn't fill the log
        /// with unneeded errors. As all of these errors are accounted for,
        /// beforehand.
        /// </summary>
        /// <param name="action">Action to execute</param>
        public static void IgnoreErrors(Action action)
        {
            if (action == null)
                return;
            try
            {
                action();
            }
            catch
            {
            }
        }

        /// <summary>
        /// This is made to throw a message if any exception is encountered,
        /// useful if you just want to throw the same message no matter the
        /// exception.
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="func">Function to execute</param>
        /// <param name="message">The message to display if an exception is found. {exception} will be replaced with the exception's message and stack trace</param>
        /// <param name="default">The default value to return</param>
        /// <returns>Object returned</returns>
        public static T ThrowMessage<T>(Func<T> func, string message, T @default = default)
        {
            if (func == null)
                return @default;
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                Console.Console.LogError(message.Replace("{exception}", ex.Message + "\n" + ex.StackTrace));
                return @default;
            }
        }

        /// <summary>
        /// This is made to throw a message if any exception is encountered,
        /// useful if you just want to throw the same message no matter the
        /// exception.
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="message">The message to display if an exception is found. {exception} will be replaced with the exception's message and stack trace</param>
        public static void ThrowMessage(Action action, string message)
        {
            if (action == null)
                return;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.Console.LogError(message.Replace("{exception}", ex.Message + "\n" + ex.StackTrace));
            }
        }

        /// <summary>
        /// This is made to throw a message if no exceptions are encountered,
        /// useful if you just want to throw some success message if no errors
        /// occur.
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="func">Function to execute</param>
        /// <param name="message">The message to display if no exception is found</param>
        /// <param name="default">The default value to return</param>
        /// <returns>Object returned</returns>
        public static T ThrowSuccessMessage<T>(Func<T> func, string message, T @default = default)
        {
            if (func == null)
                return @default;
            try
            {
                T obj = func();
                Console.Console.Log(message);
                return obj;
            }
            catch
            {
                return @default;
            }
        }

        /// <summary>
        /// This is made to throw a message if no exceptions are encountered,
        /// useful if you just want to throw some success message if no errors
        /// occur.
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="message">The message to display if no exception is found</param>
        public static void ThrowSuccessMessage(Action action, string message)
        {
            if (action == null)
                return;
            try
            {
                action();
                Console.Console.Log(message);
            }
            catch
            {
            }
        }

        /// <summary>
        /// This is made to throw a message if an exception is found,
        /// and another one if it no exceptions are encountered.
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="func">Function to execute</param>
        /// <param name="success">The message to display if no exception is found</param>
        /// <param name="error">The message to display if an exception is found. {exception} will be replaced with the exception's success and stack trace</param>
        /// <param name="default">The default value to return</param>
        /// <returns>Object returned</returns>
        public static T ThrowAllMessages<T>(Func<T> func, string success, string error, T @default = default)
        {
            if (func == null)
                return @default;
            try
            {
                T obj = func();
                Console.Console.Log(success);
                return obj;
            }
            catch (Exception ex)
            {
                Console.Console.LogError(error.Replace("{exception}", ex.Message + "\n" + ex.StackTrace));
                return @default;
            }
        }

        /// <summary>
        /// This is made to throw a message if an exception is found,
        /// and another one if it no exceptions are encountered.
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="success">The message to display if no exception is found</param>
        /// <param name="error">The message to display if an exception is found. {exception} will be replaced with the exception's success and stack trace</param>
        public static void ThrowAllMessages(Action action, string success, string error)
        {
            if (action == null)
                return;
            try
            {
                action();
                Console.Console.Log(success);
            }
            catch (Exception ex)
            {
                Console.Console.LogError(error.Replace("{exception}", ex.Message + "\n" + ex.StackTrace));
            }
        }
    }
}
