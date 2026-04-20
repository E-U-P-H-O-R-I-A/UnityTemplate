using System.Collections.Generic;
using UnityEngine;

namespace Services.LogService
{
    public enum LogSeverity
    {
        Info,
        Error,
        Warning,
    }
    
    public enum LogCategory
    {
        Scene = 0,
        General = 1,
        Service = 2,
        Utility = 3,
        Tutorial = 4,
        PrivateModel = 5,
        Infrastructure = 6,
        Windows = 7,
        LevelEditor = 8,
    }
    
    public static class LogSettings
    {
        private static readonly Dictionary<LogSeverity, bool> SEVERITY_ENABLED = new()
        {
            { LogSeverity.Info, true },
            { LogSeverity.Error, true },
            { LogSeverity.Warning, true },
        };

        private static readonly Dictionary<LogCategory, bool> CATEGORY_ENABLED = new()
        {
            { LogCategory.Scene , true},
            { LogCategory.General , true},
            { LogCategory.Service , true},
            { LogCategory.Utility , true},
            { LogCategory.Windows , true},
            { LogCategory.Tutorial , true},
            { LogCategory.LevelEditor , true},
            { LogCategory.PrivateModel , true},
            { LogCategory.Infrastructure , true},
        };
        
        private static readonly Dictionary<LogCategory, Color> COLORS_SETTING = new()
        {
            { LogCategory.Scene , new Color(0.96f, 0.78f, 0.38f)},
            { LogCategory.General , new Color(0.86f, 0.86f, 0.86f)},
            { LogCategory.Service , new Color(0.85f, 0.80f, 0.98f)},
            { LogCategory.Utility , new Color(0.78f, 0.88f, 0.56f)},
            { LogCategory.Windows , new Color(0.82f, 0.60f, 0.99f)},
            { LogCategory.Tutorial , new Color(0.56f, 0.89f, 0.45f)},
            { LogCategory.LevelEditor , new Color(0.98f, 0.92f, 0.55f)},
            { LogCategory.PrivateModel , new Color(0.71f, 0.90f, 0.90f)},
            { LogCategory.Infrastructure , new Color(0.36f, 0.73f, 0.94f)},
        };

        public static Color GetColor(LogCategory category) =>
            !COLORS_SETTING.TryGetValue(category, out Color color)
                ? COLORS_SETTING[LogCategory.General]
                : color;

        public static bool IsCategoryEnabled(LogCategory category) =>
            !CATEGORY_ENABLED.TryGetValue(category, out bool isEnable)
                ? CATEGORY_ENABLED[LogCategory.General]
                : isEnable;

        public static bool IsSeverityEnabled(LogSeverity severity) =>
            !SEVERITY_ENABLED.TryGetValue(severity, out bool isEnable)
                ? SEVERITY_ENABLED[LogSeverity.Info]
                : isEnable;
    }
}
