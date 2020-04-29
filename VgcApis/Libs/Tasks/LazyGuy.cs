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
        private readonly int expectedWorkTime;
        AutoResetEvent jobToken = new AutoResetEvent(true);
        AutoResetEvent waitToken = new AutoResetEvent(true);

        bool isCancelled = false;

        public string Name = @"";

        /// <summary>
        ///
        /// </summary>
        /// <param name="chainedTask">(done)=>{... done();}</param>
        /// <param name="interval">millisecond</param>
        /// <param name="expectedWorkTime">millisecond</param>
        public LazyGuy(Action<Action> chainedTask, int interval, int expectedWorkTime)
        {
            this.chainedTask = chainedTask;
            this.interval = interval;
            this.expectedWorkTime = expectedWorkTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="singleTask">()=>{ ... }</param>
        /// <param name="interval">millisecond</param>
        /// <param name="expectedWorkTime">millisecond</param>
        public LazyGuy(Action singleTask, int interval, int expectedWorkTime)
        {
            this.singleTask = singleTask;
            this.interval = interval;
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

            Misc.Utils.RunInBackground(() =>
            {
                Misc.Utils.Sleep(interval);
                TryDoTheJob(Deadline);
            });
        }

        CancellationTokenSource cts = null;
        readonly object cancelLocker = new object();

        /// <summary>
        /// ...~...~...~...|
        /// </summary>
        public void Postpone()
        {
            CancellationToken tk;
            lock (cancelLocker)
            {
                cts?.Cancel();
                cts = new CancellationTokenSource();
                tk = cts.Token;
            }

            Misc.Utils.RunInBackground(async () =>
            {
                try
                {
                    await Task.Delay(interval, tk);
                }
                catch
                {
                    return;
                }

                if (isCancelled || tk.IsCancellationRequested || !waitToken.WaitOne(0))
                {
                    return;
                }

                TryDoTheJob(Postpone);
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

            Misc.Utils.RunInBackground(() =>
            {
                TryDoTheJob(Deadline);
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
        void TryDoTheJob(Action retry)
        {
            var ready = jobToken.WaitOne(expectedWorkTime);
            waitToken.Set();
            if (ready)
            {
                DoTheJob();
            }
            else
            {
                if (expectedWorkTime > 1000)
                {
                    // LogSuspectableDeadLock("TryDoTheJob");
                }
                retry?.Invoke();
            }
        }

        void LogSuspectableDeadLock(string evName)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var text = $"!suspectable deadlock! {Name} - {evName}";
                Sys.FileLogger.Error(text);
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
                    Sys.FileLogger.Warn($"DoTheJob() overtime {Name}\n" +
                        $"exp: {expectedWorkTime}ms, act: {workTime}ms");
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
                catch (Exception ex)
                {
                    Sys.FileLogger.DumpCallStack(
                        $"DoTheJob() {Name} do single task error\n" +
                        $"{ex}");
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
        }
        #endregion

    }
}
