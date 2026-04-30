using System;
using System.Threading.Tasks;
using VgcApis.Interfaces.CoreCtrlComponents;

namespace VgcApis.Libs.Tasks
{
    public sealed class Routine : BaseClasses.Disposable
    {
        readonly IRoutineOwner owner;
        readonly Action action;
        readonly int interval;
        readonly object locker = new object();

        bool isRunning = false;
        bool again = false;

        public Routine(IRoutineOwner owner, int interval)
            : this(null, owner, interval) { }

        public Routine(Action action, int interval)
            : this(action, null, interval) { }

        Routine(Action action, IRoutineOwner rtAction, int interval)
        {
            if (action == null && rtAction == null)
            {
                throw new ArgumentException("Job must not empty!");
            }

            this.action = action;
            this.owner = rtAction;
            if (interval <= 0)
            {
                throw new ArgumentException("Interval must greater then zero.");
            }
            this.interval = interval;
        }

        #region public methods

        public void Restart()
        {
            if (isDisposed)
            {
                return;
            }
            var stopped = false;
            lock (locker)
            {
                again = true;
                if (!isRunning)
                {
                    stopped = true;
                    isRunning = true;
                }
            }

            if (stopped)
            {
                DoWork();
            }
        }

        public void Stop()
        {
            lock (locker)
            {
                again = false;
            }
        }

        #endregion

        #region protected methods

        protected override void Cleanup()
        {
            Stop();
        }
        #endregion

        #region private methods
        void DoWork()
        {
            Task.Delay(interval)
                .ContinueWith(_ =>
                {
                    lock (locker)
                    {
                        if (!again)
                        {
                            isRunning = false;
                            return;
                        }
                    }
                    if (owner != null)
                    {
                        owner.OnRoutineAction();
                    }
                    else
                    {
                        action();
                    }
                    DoWork();
                });
        }
        #endregion
    }
}
