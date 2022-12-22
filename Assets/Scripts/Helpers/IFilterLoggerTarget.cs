using UnityEngine;

namespace Helpers
{
    public interface IFilterLoggerTarget
    {
        public LogLevel GetLogLevel();
    }
}
