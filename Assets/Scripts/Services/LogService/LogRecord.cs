using System;

namespace Services.LogService
{
    public readonly struct LogRecord
    {
        public LogRecord(string message, LogCategory category, LogSeverity severity, DateTime createdAtUtc)
        {
            Message = message ?? string.Empty;
            Category = category;
            Severity = severity;
            CreatedAtUtc = createdAtUtc;
        }

        public string Message { get; }

        public LogCategory Category { get; }

        public LogSeverity Severity { get; }

        public DateTime CreatedAtUtc { get; }
    }
}