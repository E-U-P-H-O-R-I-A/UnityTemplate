namespace Services.LogService
{
    public interface ILogService : IService
    {
        public void Log(string msg, LogCategory category = LogCategory.General);

        public void LogError(string msg, LogCategory category = LogCategory.General);

        public void LogWarning(string msg, LogCategory category = LogCategory.General);
    }
}
