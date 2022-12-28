using UnityEngine;

namespace Helpers
{
    public interface IFilterLoggerTarget
    {
        public string GetLogName()
        {
            return this.ToString();
        }

        public LogLevel GetLogLevel();
    }
}
