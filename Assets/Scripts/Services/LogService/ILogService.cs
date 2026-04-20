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

    public interface ILogService : IService
    {
        public string GetAllLogs();
        public string GetLogsBySeverity(LogSeverity severity);
        public string GetLogsByCategory(LogCategory category);
        
        public void Log(string msg, LogCategory category = LogCategory.General);
        public void LogError(string msg, LogCategory category = LogCategory.General);
        public void LogWarning(string msg, LogCategory category = LogCategory.General);
    }
}
