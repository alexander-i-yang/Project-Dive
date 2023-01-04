using System.Buffers.Text;
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
        protected float _timerValue { get; private set; }
        protected float _duration { get; }
        private string _name;

        private bool _active;

        internal GameTimer(float duration, string name)
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

        internal void Reset()
        {
            _timerValue = _duration;
            _active = true;
        }

        protected virtual void Update()
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

        internal bool Finished()
        {
            return _timerValue <= 0;
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Warning;
        }
    }
    
    public enum TimerStateWindowed
    {
        Inactive,
        BeforeWindow,
        InWindow,
        AfterWindow
    }

    public class GameTimerWindowed : GameTimer
    {
        /**
         * A GameWindowedTimer is a timer with an appropriate window where the Timer is active.
         * _length is the length of the window.
         * base._duration becomes the length of the window + the delay before the window.
         */
        private float _length;
        internal GameTimerWindowed(float duration, float length, string name) : base(duration, name)
        {
            _length = length;
        }
        
        public static GameTimerWindowed StartNewWindowedTimer(float delay, float length, string name = "Timer")
        {
            GameTimerWindowed timer = new GameTimerWindowed(delay+length, length, name);
            timer.Reset();
            return timer;
        }
        
        public static TimerStateWindowed GetTimerState(GameTimerWindowed timer)
        {
            if (GameTimer.GetTimerState(timer) == TimerState.Inactive) return TimerStateWindowed.Inactive;

            if (timer.InWindow()) return TimerStateWindowed.InWindow;
            return timer.Finished() ? TimerStateWindowed.AfterWindow : TimerStateWindowed.BeforeWindow;
        }

        private bool InWindow()
        {
            return _timerValue <= _length && _timerValue > 0;
        }
    }
}
