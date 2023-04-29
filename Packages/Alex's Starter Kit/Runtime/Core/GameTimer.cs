using System;
using ASK.Helpers;

namespace ASK.Core
{
    public enum TimerState
    {
        Inactive,
        Running,
        Paused,
        Finished
    }
    public class GameTimer : IFilterLoggerTarget
    {
        public float TimerValue { get; private set; }
        protected float _duration { get; private set; }
        private string _name;
        private bool _paused;

        private bool _active;

        public event Action OnFinished;

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

            if (timer.Finished()) return TimerState.Finished;

            return timer._paused ? TimerState.Paused : TimerState.Running;
        }

        public static bool TimerFinished(GameTimer timer)
        {
            return GetTimerState(timer) == TimerState.Finished;
        }

        public static bool TimerInactive(GameTimer timer)
        {
            return GetTimerState(timer) == TimerState.Inactive;
        }

        public static void Pause(GameTimer timer)
        {
            if (timer != null)
            {
                timer._paused = true;
            }
        }
        
        public static void UnPause(GameTimer timer)
        {
            if (timer != null)
            {
                timer._paused = false;
            }
        }

        public static void Reset(GameTimer timer, float newDuration = -1)
        {
            if (timer != null)
            {
                timer.Reset();
            }

            if (newDuration > 0 && timer != null)
            {
                timer._duration = newDuration;
            }
        }

        protected virtual void Update()
        {
            if (!Finished() && !_paused)
            {
                TimerValue -= Game.Instance.DeltaTime;
                FilterLogger.Log(this, $"{_name}: {TimerValue}");

                if (Finished())
                {
                    OnFinished?.Invoke();
                }
            }

        }

        private void FixedUpdate()
        {
            if (!Finished() && !_paused)
            {
                TimerValue -= Game.Instance.FixedDeltaTime;
                FilterLogger.Log(this, $"{_name}: {TimerValue}");
            }
        }
        protected void Reset()
        {
            TimerValue = _duration;
            _active = true;
        }

        protected bool Finished()
        {
            return TimerValue <= 0;
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
            return TimerValue <= _length && TimerValue > 0;
        }
    }
}
