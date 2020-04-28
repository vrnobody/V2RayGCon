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
        /// blocking
        /// </summary>
        public void DoItNow()
        {
            LogJobTokenWaitTime(jobToken, nameof(jobToken));
            DoTheJob();
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
            // Console.WriteLine($"ready; {ready}");
            waitToken.Set();
            if (ready)
            {
                DoTheJob();
            }
            else
            {
                LogSuspectableDeadLock("TryDoTheJob");
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

        void LogJobTokenWaitTime(AutoResetEvent arEv, string evName)
        {
            while (!arEv.WaitOne(expectedWorkTime))
            {
                LogSuspectableDeadLock(evName);
                Misc.Utils.Sleep(100);
            }
        }

        void Done()
        {
            jobToken.Set();
            Sys.FileLogger.Info($"{Name} job finished");
        }

        void DoTheJob()
        {
            if (isCancelled)
            {
                Done();
                return;
            }

            Sys.FileLogger.Info($"{Name} job begin");

            if (chainedTask == null)
            {
                try
                {
                    singleTask?.Invoke();
                }
                finally
                {
                    Done();
                }
                return;
            }

            try
            {
                Misc.Utils.RunInBackground(() => chainedTask?.Invoke(Done));
            }
            catch
            {
                Done();
            }
        }

        #endregion

        #region protected method
        protected override void Cleanup()
        {
            isCancelled = true;
            singleTask = null;
            chainedTask = null;
        }
        #endregion

    }
}
