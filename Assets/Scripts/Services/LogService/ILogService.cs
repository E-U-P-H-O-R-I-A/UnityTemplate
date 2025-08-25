namespace Services.LogService
{
    public interface ILogService : IService
    {
        public void Log(string msg);

        public void LogError(string msg);

        public void LogWarning(string msg);
    }
}