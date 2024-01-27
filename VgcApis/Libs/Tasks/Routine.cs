using System;
using System.Timers;

namespace VgcApis.Libs.Tasks
{
    public sealed class Routine : BaseClasses.Disposable
    {
        readonly Action action;
        readonly Timer timer;
        readonly Bar bar;

        public Routine(Action action, int interval)
        {
            this.action = action ?? throw new ArgumentException("Job must not empty!");

            if (interval <= 0)
            {
                throw new ArgumentException("Interval must greater then zero.");
            }

            bar = new Bar();
            timer = CreateTimer(interval);
            timer.Elapsed += Task;
        }

        #region public methods
        /// <summary>
        /// Start routine.
        /// </summary>
        public void Run() => timer.Start();

        public void Pause() => timer.Stop();

        #endregion

        #region protected methods
        /// <summary>
        /// Dispose timer only!
        /// </summary>
        protected override void Cleanup()
        {
            timer.Stop();
            timer.Dispose();
        }
        #endregion

        #region private methods
        void Task(object sender, EventArgs args)
        {
            if (!bar.Install())
            {
                return;
            }

            try
            {
                action();
            }
            finally
            {
                bar.Remove();
            }
        }

        Timer CreateTimer(int interval) =>
            new Timer
            {
                AutoReset = true,
                Interval = interval,
                Enabled = false,
            };
        #endregion
    }
}
