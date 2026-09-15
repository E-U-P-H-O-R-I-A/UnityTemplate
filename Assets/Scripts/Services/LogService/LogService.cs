using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Services.LogService
{
    public class LogService : ILogService
    {
        private const string LOGS_FOLDER_NAME = "Logs";
        private const int CAPACITY = 2000;

        private readonly LogRecord[] buffer = new LogRecord[CAPACITY];

        private int head;
        private int count;

        public string GetAllLogs() =>
            SaveLogsToFile(Snapshot(_ => true), "all");

        public string GetLogsByCategory(LogCategory category) =>
            SaveLogsToFile(Snapshot(log => log.Category == category), $"category_{category}");

        public string GetLogsBySeverity(LogSeverity severity) =>
            SaveLogsToFile(Snapshot(log => log.Severity == severity), $"severity_{severity}");

        public void Log(string msg, LogCategory category = LogCategory.General) =>
            Write(msg, category, LogSeverity.Info);

        public void LogError(string msg, LogCategory category = LogCategory.General) =>
            Write(msg, category, LogSeverity.Error);

        public void LogWarning(string msg, LogCategory category = LogCategory.General) =>
            Write(msg, category, LogSeverity.Warning);

        private void Write(string msg, LogCategory category, LogSeverity severity)
        {
            Store(new LogRecord(msg, category, severity, DateTime.UtcNow));

            if (!LogSettings.IsCategoryEnabled(category) || !LogSettings.IsSeverityEnabled(severity))
                return;

            var formattedMessage = FormatMessage(msg, category);

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

        private void Store(LogRecord record)
        {
            buffer[(head + count) % CAPACITY] = record;

            if (count < CAPACITY)
                count++;
            else
                head = (head + 1) % CAPACITY;
        }

        private List<LogRecord> Snapshot(Func<LogRecord, bool> predicate)
        {
            var result = new List<LogRecord>(count);

            for (int i = 0; i < count; i++)
            {
                var record = buffer[(head + i) % CAPACITY];

                if (predicate(record))
                    result.Add(record);
            }

            return result;
        }

        private static string FormatMessage(string message, LogCategory category)
        {
            var colorHex = ColorUtility.ToHtmlStringRGB(LogSettings.GetColor(category));
            return $"<color=#{colorHex}>[{category}]</color> {message}";
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
