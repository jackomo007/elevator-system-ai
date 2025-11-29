using System;
using System.Collections.Concurrent;
using System.Threading;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Infrastructure.Logging
{
    /// <summary>
    /// Thread-safe console logger that cooperates with user input:
    /// - While the user is typing, log messages are buffered.
    /// - When typing stops, buffered messages are flushed to the console.
    /// </summary>
    public sealed class ConsoleLogger : ILogger
    {
        // Single lock for the console to avoid interleaved writes.
        private static readonly object _consoleLock = new();

        // Buffered messages while user is typing.
        private static readonly ConcurrentQueue<string> _bufferedMessages = new();

        // 0 = false, 1 = true; use Interlocked/Volatile for thread safety.
        private static int _userTypingFlag;

        /// <summary>
        /// Indicates whether the user is currently typing.
        /// Set by the console app before/after Console.ReadLine().
        /// </summary>
        public static void SetUserTyping(bool isTyping)
        {
            Interlocked.Exchange(ref _userTypingFlag, isTyping ? 1 : 0);

            // When typing stops, flush any buffered messages.
            if (!isTyping)
            {
                FlushBufferedMessages();
            }
        }

        private static bool IsUserTyping =>
            Volatile.Read(ref _userTypingFlag) == 1;

        private static void FlushBufferedMessages()
        {
            while (_bufferedMessages.TryDequeue(out var msg))
            {
                lock (_consoleLock)
                {
                    Console.WriteLine(msg);
                }
            }
        }

        private static void LogInternal(string level, string message)
        {
            var entry = $"{DateTime.UtcNow:O} [{level}] {message}";

            // If the user is typing, buffer the message instead of printing.
            if (IsUserTyping)
            {
                _bufferedMessages.Enqueue(entry);
                return;
            }

            lock (_consoleLock)
            {
                Console.WriteLine(entry);
            }
        }

        public void Info(string message)  => LogInternal("INFO ", message);
        public void Debug(string message) => LogInternal("DEBUG", message);
        public void Warn(string message)  => LogInternal("WARN ", message);
        public void Error(string message) => LogInternal("ERROR", message);
    }
}
