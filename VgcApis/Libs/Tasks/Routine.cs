using System;
using System.Timers;

namespace VgcApis.Libs.Tasks
{
    public sealed class Routine :
        Models.BaseClasses.Disposable
    {
        readonly Action action;

        Timer schedule;
        Bar bar;

        public Routine(Action action, int interval)
        {
            this.action = action ?? throw new ArgumentException("Job must not empty!");

            if (interval <= 0)
            {
                throw new ArgumentException("Interval must greater then zero.");
            }

            bar = new Bar();
            schedule = MakeSchedule(interval);
            schedule.Elapsed += Task;
        }

        #region public methods
        /// <summary>
        /// Start routine.
        /// </summary>
        public void Run() => schedule.Start();

        public void Pause() => schedule.Stop();

        #endregion

        #region protected methods
        /// <summary>
        /// Dispose timer only!
        /// </summary>
        protected override void Cleanup()
        {
            schedule.Stop();
            lock (action)
            {
                schedule.Dispose();
            }
        }
        #endregion

        #region private methods
        void Task(object sender, EventArgs args)
        {
            if (!bar.Install())
            {
                return;
            }

            lock (action)
            {
                action();
            }
            bar.Remove();
        }

        Timer MakeSchedule(int interval) =>
            new Timer
            {
                AutoReset = true,
                Interval = interval,
                Enabled = false,
            };
        #endregion
    }
}
