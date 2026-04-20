using UnityEngine;

namespace Services.LogService
{
    public class LogService : ILogService
    {
        public void Log(string msg, LogCategory category = LogCategory.General) =>
            Write(msg, category, LogSeverity.Info);

        public void LogError(string msg, LogCategory category = LogCategory.General) =>
            Write(msg, category, LogSeverity.Error);

        public void LogWarning(string msg, LogCategory category = LogCategory.General) =>
            Write(msg, category, LogSeverity.Warning);

        private void Write(string msg, LogCategory category, LogSeverity severity)
        {
            if (!LogSettings.IsCategoryEnabled(category) || !LogSettings.IsSeverityEnabled(severity))
                return;

            var formattedMessage = FormatMessage(msg, category, LogSettings.GetColor(category));
            
            switch (severity)
            {
                case LogSeverity.Info:
                    Debug.Log(formattedMessage);
                    break;
                case LogSeverity.Warning:
                    Debug.LogWarning(formattedMessage);
                    break;
                case LogSeverity.Error:
                    Debug.LogError(formattedMessage);
                    break;
            }
        }

        private string FormatMessage(string message, LogCategory category, Color color)
        {
            var colorHex = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{colorHex}>[{category}]</color> {message}";
        }
    }
}
