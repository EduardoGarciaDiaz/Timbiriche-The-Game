using System;
using System.Timers;

namespace TimbiricheViews.Match
{
    public class Timer
    {
        private const int DEFAULT_TIME = 0;
        private readonly int _initialSeconds;
        private readonly System.Timers.Timer timer;
        private int _seconds;
        private bool _isRunning;

        public event EventHandler CountDownFinished;

        public Timer(int initialSeconds)
        {
            int intervalInMiliseconds = 1000;
            _initialSeconds = initialSeconds;
            _seconds = initialSeconds;
            _isRunning = false;
            timer = new System.Timers.Timer(intervalInMiliseconds);
            timer.Elapsed += TimerElapsed;
        }

        public string GetTime()
        {
            float secondsPerMinute = 60;
            float minutes = (float)_seconds / secondsPerMinute;
            float remainingSeconds = _seconds % secondsPerMinute;
            string timeFormat = "{0:00}:{1:00}";

            return string.Format(timeFormat, minutes, remainingSeconds);
        }

        public void Start()
        {
            if (!_isRunning && _seconds > DEFAULT_TIME)
            {
                _isRunning = true;
                timer.Start();
            }
        }

        public void Stop()
        {
            _isRunning = false;
            timer.Stop();
        }

        public void Reset()
        {
            _seconds = _initialSeconds;
            Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_seconds > DEFAULT_TIME)
            {
                _seconds--;
            }
            else
            {
                Stop();
                OnCountDownFinished();
            }
        }

        protected virtual void OnCountDownFinished()
        {
            CountDownFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}
