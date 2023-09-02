using System;
using System.Threading;
using System.Threading.Tasks;

namespace VgcApis.Libs.Tasks
{
    public class LazyGuy : BaseClasses.Disposable
    {
        private Action singleTask;
        private Action<Action> chainedTask;
        private readonly int interval;
        private readonly long ticks;
        private readonly int expectedWorkTime;

        AutoResetEvent jobToken = new AutoResetEvent(true);
        AutoResetEvent waitToken = new AutoResetEvent(true);

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
            Misc.Utils.RunInBackground(() =>
            {
                Misc.Utils.Sleep(interval);
                TryDoTheJob();
            });
        }

        /// <summary>
        /// ...~...~...~...|
        /// </summary>
        public void Postpone()
        {
            checkpoint = DateTime.Now.Ticks + ticks;
            if (isCancelled || !waitToken.WaitOne(0))
            {
                return;
            }

            retry = Postpone;
            Misc.Utils.RunInBackground(() =>
            {
                PostponeWorker();
                TryDoTheJob();
            });
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
            Misc.Utils.RunInBackground(() =>
            {
                TryDoTheJob();
            });
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
        private long checkpoint;

        void PostponeWorker()
        {
            while (true)
            {
                var delay = checkpoint - DateTime.Now.Ticks;
                if (delay < 1)
                {
                    break;
                }
                Misc.Utils.Sleep(TimeSpan.FromTicks(delay));
            }
        }

        void TryDoTheJob()
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

            Task.Run(async () =>
            {
                var delay = Math.Max(3000, expectedWorkTime * 3);
                await Task.Delay(delay);
                if (!ok)
                {
                    // jobToken.Set();
                    Sys.FileLogger.Error(errMsg);
                }
            });

            Action done = () =>
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
            };

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
#if DEBUG
                catch (Exception ex)
                {
                    Sys.FileLogger.DumpCallStack(
                        $"DoTheJob() {Name} do single task error\n" + $"{ex}"
                    );
                }
#else
                catch
                {
                    Sys.FileLogger.Warn($"DoTheJob() {Name} is running in UI thread");
                }
#endif
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
