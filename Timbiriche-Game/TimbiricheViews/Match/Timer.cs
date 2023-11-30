using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TimbiricheViews.Match
{
    public class Timer
    {
        private readonly int _initialSeconds;
        private int _seconds;
        private bool _isRunning;
        private readonly System.Timers.Timer timer;

        public event EventHandler CountDownFinished;

        public Timer(int initialSeconds)
        {
            _initialSeconds = initialSeconds;
            _seconds = initialSeconds;
            _isRunning = false;
            timer = new System.Timers.Timer(1000); // 1 - second inteval
            timer.Elapsed += TimerElapsed;
        }

        public string GetTime()
        {
            int minutes = _seconds / 60;
            int remainingSeconds = _seconds % 60;

            return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
        }

        public void Start()
        {
            if(!_isRunning && _seconds > 0)
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
            if(_seconds > 0)
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
