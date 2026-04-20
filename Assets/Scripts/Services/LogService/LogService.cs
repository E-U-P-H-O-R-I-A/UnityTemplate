using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services.LogService
{
    public class LogService : ILogService
    {
        private const string LOGS_FOLDER_NAME = "Logs";
        private readonly List<LogRecord> logs = new();
        
        public string GetAllLogs() =>
            SaveLogsToFile(logs, "all");
        
        public string GetLogsByCategory(LogCategory category) => 
            SaveLogsToFile(logs.Where(log => log.Category == category).ToList(), $"category_{category}");

        public string GetLogsBySeverity(LogSeverity severity) => 
            SaveLogsToFile(logs.Where(log => log.Severity == severity).ToList(), $"severity_{severity}");

        public void Log(string msg, LogCategory category = LogCategory.General) =>
            Write(msg, category, LogSeverity.Info);

        public void LogError(string msg, LogCategory category = LogCategory.General) =>
            Write(msg, category, LogSeverity.Error);

        public void LogWarning(string msg, LogCategory category = LogCategory.General) =>
            Write(msg, category, LogSeverity.Warning);

        private string FormatMessage(string message, LogCategory category, Color color)
        {
            var colorHex = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{colorHex}>[{category}]</color> {message}";
        }

        private void Write(string msg, LogCategory category, LogSeverity severity)
        {
            var formattedMessage = FormatMessage(msg, category, LogSettings.GetColor(category));
            
            logs.Add(new LogRecord(msg, category, severity, DateTime.UtcNow));

            if (!LogSettings.IsCategoryEnabled(category) || !LogSettings.IsSeverityEnabled(severity))
                return;
            
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

        private string SaveLogsToFile(IReadOnlyList<LogRecord> logsToSave, string fileSuffix)
        {
            try
            {
                var folderPath = Path.Combine(Application.persistentDataPath, LOGS_FOLDER_NAME);
                Directory.CreateDirectory(folderPath);

                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                var fileName = $"logs_{fileSuffix}_{timestamp}.txt";
                var filePath = Path.Combine(folderPath, fileName);

                var builder = new StringBuilder();
                builder.AppendLine("CreatedAtUtc | Severity | Category | Message");

                for (int i = 0; i < logsToSave.Count; i++)
                {
                    var logRecord = logsToSave[i];
                    builder.Append(logRecord.CreatedAtUtc.ToString("O"));
                    builder.Append(" | ");
                    builder.Append(logRecord.Severity);
                    builder.Append(" | ");
                    builder.Append(logRecord.Category);
                    builder.Append(" | ");
                    builder.AppendLine(logRecord.Message);
                }

                File.WriteAllText(filePath, builder.ToString());
                return filePath;
            }
            catch (Exception exception)
            {
                LogError($"Failed to save logs to file. {exception}", LogCategory.Service);
                return string.Empty;
            }
        }
    }
}
