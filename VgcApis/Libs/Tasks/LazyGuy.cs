using System;
using System.Threading;
using System.Threading.Tasks;

namespace VgcApis.Libs.Tasks
{
    public class LazyGuy : BaseClasses.Disposable
    {
        private Action singleTask;
        private Action<Action> chainedTask;
        private readonly int timeout;
        private readonly int expectedWorkTime;
        AutoResetEvent jobToken = new AutoResetEvent(true);
        AutoResetEvent waitingToken = new AutoResetEvent(true);

        bool isCancelled = false;

        public string Name = @"";

        /// <summary>
        ///
        /// </summary>
        /// <param name="chainedTask">(done)=>{... done();}</param>
        /// <param name="timeout">millisecond</param>
        public LazyGuy(Action<Action> chainedTask, int timeout, int expectedWorkTime)
        {
            this.chainedTask = chainedTask;
            this.timeout = timeout;
            this.expectedWorkTime = expectedWorkTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="singleTask">()=>{ ... }</param>
        /// <param name="timeout">millisecond</param>
        public LazyGuy(Action singleTask, int timeout)
        {
            this.singleTask = singleTask;
            this.timeout = timeout;
        }

        #region public method

        /// <summary>
        /// ...|...|...|...|
        /// </summary>
        public void Deadline()
        {
            if (isCancelled || !waitingToken.WaitOne(0))
            {
                return;
            }

            Misc.Utils.RunInBackground(() =>
            {
                Misc.Utils.Sleep(timeout);
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
                    await Task.Delay(timeout, tk);
                }
                catch
                {
                    return;
                }

                if (isCancelled || tk.IsCancellationRequested || !waitingToken.WaitOne(0))
                {
                    return;
                }

                TryDoTheJob(Postpone);
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

        /// <summary>
        /// |...|...|...|...|
        /// </summary>
        public void Throttle()
        {
            if (isCancelled || !waitingToken.WaitOne(0))
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
            DebugAutoResetEvent(jobToken, nameof(jobToken));
            DoTheJob();
        }

        #endregion

        #region private method
        void TryDoTheJob(Action retry)
        {
            var ready = jobToken.WaitOne(timeout + expectedWorkTime);
            // Console.WriteLine($"ready; {ready}");
            waitingToken.Set();
            if (ready)
            {
                DoTheJob();
            }
            else
            {
                DumpCurCallStack("TryDoTheJob");
                retry?.Invoke();
            }
        }

        void DumpCurCallStack(string evName)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var title = $"!suspectable deadlock! {Name} - {evName}";
                Sys.FileLogger.DumpCallStack(title);
            }
        }

        void DebugAutoResetEvent(AutoResetEvent arEv, string evName)
        {
            while (!arEv.WaitOne(timeout + expectedWorkTime))
            {
                DumpCurCallStack(evName);
                VgcApis.Misc.Utils.Sleep(100);
            }
        }

        void Done() => jobToken.Set();

        void DoTheJob()
        {
            if (isCancelled)
            {
                Done();
                return;
            }

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
