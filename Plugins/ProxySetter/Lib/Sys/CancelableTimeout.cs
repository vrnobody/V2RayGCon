using System;
using System.Timers;

namespace ProxySetter.Lib.Sys
{
    public class CancelableTimeout
    {
        Timer timer;
        readonly int TIMEOUT;
        Action worker;

        public CancelableTimeout(Action worker, int timeout)
        {
            if (timeout <= 0 || worker == null)
            {
                throw new ArgumentException();
            }

            this.TIMEOUT = timeout;
            this.worker = worker;

            timer = new Timer
            {
                Enabled = false,
                AutoReset = false
            };

            timer.Elapsed += OnTimeout;
        }

        private void OnTimeout(object sender, EventArgs e)
        {
            this.worker();
        }

        public void Timeout()
        {
            Cancel();
            this.worker();
        }

        public void Start()
        {
            timer.Interval = this.TIMEOUT;
            timer.Enabled = true;
        }

        public void Cancel()
        {
            timer.Enabled = false;
        }

        public void Release()
        {
            Cancel();
            timer.Elapsed -= OnTimeout;
            this.worker = null;
            timer.Close();
        }
    }
}
