using UnityEngine;

namespace ASK.Helpers
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
