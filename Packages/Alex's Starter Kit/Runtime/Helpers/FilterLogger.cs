using System.Collections.Generic;

using UnityEngine;

namespace ASK.Helpers
{
    using DebugLogMethod = System.Action<object>;

    public enum LogLevel
    {
        None = 0,
        Error,
        Warning,
        Info,
    }

    public static class FilterLogger
    {
        private static Dictionary<LogLevel, DebugLogMethod> DebugLogMethods = new Dictionary<LogLevel, DebugLogMethod>
        {
            { LogLevel.Error, Debug.LogError },
            { LogLevel.Warning, Debug.LogWarning },
            { LogLevel.Info, Debug.Log },
        };

        public static void Log(IFilterLoggerTarget caller, object message)
        {
            LogAtLevel(LogLevel.Info, caller, message);
        }

        public static void LogWarning(IFilterLoggerTarget caller, object message)
        {
            LogAtLevel(LogLevel.Warning, caller, message);
        }

        public static void LogError(IFilterLoggerTarget caller, object message)
        {
            LogAtLevel(LogLevel.Error, caller, message);
        }

        private static void LogAtLevel(LogLevel logLevel, IFilterLoggerTarget caller, object message)
        {
            DebugLogMethod doLogAtLevel;
            if (!DebugLogMethods.TryGetValue(logLevel, out doLogAtLevel))
            {
                Debug.LogWarning("Unallowed log level. Level must be either Log, Warning, or Error.");
            }

            if (logLevel <= caller.GetLogLevel())
            {
                doLogAtLevel($"[{caller.GetLogName()}] {message}");
            }
        }
    }
}