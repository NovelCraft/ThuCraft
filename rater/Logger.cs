using System;
using System.Diagnostics;

/// <summary>
/// Logger class provides logging functionality.
/// </summary>
public class Logger {
  #region Fields and properties
  private readonly string _loggingNamespace;
  #endregion

  #region Constructors and finalizers
  /// <summary>
  /// Creates a new Logger instance.
  /// </summary>
  public Logger(string loggingNamespace) {
    _loggingNamespace = loggingNamespace;
  }
  #endregion

  #region Methods
  /// <summary>
  /// Logs an debug message.
  /// </summary>
  /// <param name="message">The message</param>
  public void Debug(string message) {
    lock (Console.Out) {
      DebugInternal(message);
    }
  }

  /// <summary>
  /// Logs an information message.
  /// </summary>
  /// <param name="message">The message</param>
  public void Info(string message) {
    lock (Console.Out) {
      PrintStdout($"{GetCurrentTimeString()} ", ConsoleColor.Cyan);
      PrintStdout($"INFO  ", ConsoleColor.Blue);
      PrintStdout($"[{_loggingNamespace}] {message}", ConsoleColor.White);
      Console.WriteLine();
    }
  }

  /// <summary>
  /// Logs an warning message.
  /// </summary>
  /// <param name="message">The message</param>
  public void Warning(string message) {
    lock (Console.Out) {
      PrintStderr($"{GetCurrentTimeString()} ", ConsoleColor.Cyan);
      PrintStderr($"WARN  ", ConsoleColor.Yellow);
      PrintStderr($"[{_loggingNamespace}] {message}", ConsoleColor.Yellow);
      Console.WriteLine();
    }
  }

  /// <summary>
  /// Logs an error message.
  /// </summary>
  /// <param name="message">The message</param>
  public void Error(string message) {
    lock (Console.Out) {
      PrintStderr($"{GetCurrentTimeString()} ", ConsoleColor.Cyan);
      PrintStderr($"ERROR ", ConsoleColor.Red);
      PrintStderr($"[{_loggingNamespace}] {message}", ConsoleColor.Red);
      Console.WriteLine();
    }
  }

  [Conditional("DEBUG")]
  private void DebugInternal(string message) {
    PrintStdout($"{GetCurrentTimeString()} ", ConsoleColor.Cyan);
    PrintStdout($"DEBUG ", ConsoleColor.Gray);
    PrintStdout($"[{_loggingNamespace}] {message}", ConsoleColor.Gray);
    Console.WriteLine();
  }

  /// <summary>
  /// Gets the current time string.
  /// </summary>
  /// <returns>The current time string</returns>
  private static string GetCurrentTimeString() {
    return DateTime.Now.ToString("HH:mm:ss");
  }

  /// <summary>
  /// Prints a message.
  /// </summary>
  /// <param name="message">The message</param>
  /// <param name="messageColor">The message color</param>
  private static void PrintStdout(string message, ConsoleColor messageColor) {
    Console.ForegroundColor = messageColor;
    Console.Write(message);
    Console.ResetColor();
  }

  /// <summary>
  /// Prints a message to stderr.
  /// </summary>
  /// <param name="message">The message</param>
  /// <param name="messageColor">The message color</param>
  private static void PrintStderr(string message, ConsoleColor messageColor) {
    Console.ForegroundColor = messageColor;
    Console.Error.Write(message);
    Console.ResetColor();
  }
  #endregion
}
