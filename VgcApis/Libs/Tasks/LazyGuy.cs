using System;
using System.Threading;

namespace VgcApis.Libs.Tasks
{
    public class LazyGuy : BaseClasses.Disposable
    {
        private Action singleTask;
        private Action<Action> chainedTask;
        private readonly int interval;
        private readonly long ticks;
        private readonly int expectedWorkTime;
        readonly AutoResetEvent jobToken = new AutoResetEvent(true);
        readonly AutoResetEvent waitToken = new AutoResetEvent(true);

        bool isCancelled = false;
        Action retry = null;

        public string Name = @"";

        /// <summary>
        ///
        /// </summary>
        /// <param name="chainedTask">(done)=>{... done();}</param>
        /// <param name="interval">millisecond</param>
        /// <param name="expectedWorkTime">millisecond</param>
        public LazyGuy(Action<Action> chainedTask, int interval, int expectedWorkTime)
            : this(interval, expectedWorkTime)
        {
            this.chainedTask = chainedTask;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="singleTask">()=>{ ... }</param>
        /// <param name="interval">millisecond</param>
        /// <param name="expectedWorkTime">millisecond</param>
        public LazyGuy(Action singleTask, int interval, int expectedWorkTime)
            : this(interval, expectedWorkTime)
        {
            this.singleTask = singleTask;
        }

        LazyGuy(int interval, int expectedWorkTime)
        {
            this.interval = interval;
            this.ticks = interval * TimeSpan.TicksPerMillisecond;
            this.expectedWorkTime = expectedWorkTime;
        }

        #region public method

        /// <summary>
        /// ...|...|...|...|
        /// </summary>
        public void Deadline()
        {
            if (isCancelled || !waitToken.WaitOne(0))
            {
                return;
            }

            retry = Deadline;
            Misc.Utils.DoItLater(TryDoTheJob, interval);
        }

        private long postPoneCheckpoint;

        /// <summary>
        /// ...~...~...~...|
        /// </summary>
        public void Postpone()
        {
            postPoneCheckpoint = DateTime.Now.Ticks + ticks;
            if (isCancelled || !waitToken.WaitOne(0))
            {
                return;
            }

            retry = Postpone;
            void next()
            {
                var ms = (postPoneCheckpoint - DateTime.Now.Ticks) / TimeSpan.TicksPerMillisecond;
                if (ms > 50)
                {
                    Misc.Utils.DoItLater(next, ms);
                    return;
                }
                TryDoTheJob();
            }
            Misc.Utils.DoItLater(next, interval);
        }

        /// <summary>
        /// |...|...|...|...|
        /// </summary>
        public void Throttle()
        {
            if (isCancelled || !waitToken.WaitOne(0))
            {
                return;
            }

            retry = Deadline;
            TryDoTheJob();
        }

        /// <summary>
        /// set isCancelled = true only
        /// </summary>
        public void ForgetIt()
        {
            isCancelled = true;
        }

        /// <summary>
        /// set isCancelled = false only
        /// </summary>
        public void PickItUp()
        {
            isCancelled = false;
        }
        #endregion

        #region private method
        void TryDoTheJob()
        {
            Misc.Utils.RunInBackground(TryDoTheJobCore);
        }

        void TryDoTheJobCore()
        {
            var ready = jobToken.WaitOne(expectedWorkTime);
            waitToken.Set();
            if (ready)
            {
                DoTheJob();
            }
            else
            {
                // disabled
                if (false && expectedWorkTime > 1000)
                {
                    if (!string.IsNullOrEmpty(Name))
                    {
                        var text = $"!suspectable deadlock! {Name} - TryDoTheJob()";
                        Sys.FileLogger.Error(text);
                    }
                }
                retry?.Invoke();
            }
        }

        void DoTheJob()
        {
            string errMsg = $"DoTheJob() timeout {Name}\n";

            var ok = false;
            var start = DateTime.Now.Ticks;

            var delay = Math.Max(3000, expectedWorkTime * 3);
            Misc.Utils.DoItLater(
                () =>
                {
                    if (!ok)
                    {
                        // jobToken.Set();
                        Sys.FileLogger.Error(errMsg);
                    }
                },
                delay
            );

            void done()
            {
                ok = true;
                jobToken.Set();
                var end = DateTime.Now.Ticks;

                var workTime = TimeSpan.FromTicks(end - start).TotalMilliseconds;
                if (workTime > 2 * expectedWorkTime)
                {
                    Sys.FileLogger.Warn(
                        $"DoTheJob() overtime {Name}\n"
                            + $"exp: {expectedWorkTime}ms, act: {workTime}ms"
                    );
                }
            }

            if (isCancelled)
            {
                done();
                return;
            }

            // Sys.FileLogger.Info($"{Name} job begin");

            if (chainedTask == null)
            {
                try
                {
                    singleTask?.Invoke();
                }
                catch (Exception ex)
                {
                    Sys.FileLogger.DumpCallStack(
                        $"DoTheJob() {Name} do single task error\n" + $"{ex}"
                    );
                }
                done();
                return;
            }

            try
            {
                Misc.Utils.RunInBackground(() => chainedTask?.Invoke(done));
            }
            catch
            {
                done();
            }
        }

        #endregion

        #region protected method
        protected override void Cleanup()
        {
            isCancelled = true;
            this.singleTask = null;
            this.chainedTask = null;
        }
        #endregion
    }
}
