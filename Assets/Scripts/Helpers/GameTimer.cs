using Core;

namespace Helpers
{
    public enum TimerState
    {
        Inactive,
        Running,
        Finished
    }
    public class GameTimer : IFilterLoggerTarget
    {
        private float _timerValue;
        private float _duration;
        private string _name;

        private bool _active;

        private GameTimer(float duration, string name)
        {
            _duration = duration;
            _name = name;
        }

        public static GameTimer StartNewTimer(float duration, string name = "Timer")
        {
            GameTimer timer = new GameTimer(duration, name);
            timer.Reset();
            return timer;
        }

        public static void Update(GameTimer timer)
        {
            if (timer != null)
            {
                timer.Update();
            }
        }

        public static void FixedUpdate(GameTimer timer)
        {
            if (timer != null)
            {
                timer.FixedUpdate();
            }
        }

        public static void Clear(GameTimer timer)
        {
            if (timer != null)
            {
                timer._active = false;
            }
        }

        public static TimerState GetTimerState(GameTimer timer)
        {
            if (timer == null || !timer._active)
            {
                return TimerState.Inactive;
            }

            return timer.Finished() ? TimerState.Finished : TimerState.Running;
        }

        private void Reset()
        {
            _timerValue = _duration;
            _active = true;
        }

        private void Update()
        {
            if (!Finished())
            {
                _timerValue = _timerValue - Game.Instance.DeltaTime;
                FilterLogger.Log(this, $"{_name}: {_timerValue}");
            }

        }

        private void FixedUpdate()
        {
            if (!Finished())
            {
                _timerValue = _timerValue - Game.Instance.FixedDeltaTime;
                FilterLogger.Log(this, $"{_name}: {_timerValue}");
            }
        }

        private bool Finished()
        {
            return _timerValue <= 0;
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Warning;
        }
    }
}
