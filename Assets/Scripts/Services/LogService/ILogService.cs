namespace Services.LogService
{
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
