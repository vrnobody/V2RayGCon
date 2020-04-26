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

        AutoResetEvent jobToken = new AutoResetEvent(true);
        AutoResetEvent waitingToken = new AutoResetEvent(true);

        bool isCancelled = false;

        public string Name = @"";


        /// <summary>
        ///
        /// </summary>
        /// <param name="chainedTask">(done)=>{... done();}</param>
        /// <param name="timeout">millisecond</param>
        public LazyGuy(Action<Action> chainedTask, int timeout)
        {
            this.chainedTask = chainedTask;
            this.timeout = timeout;
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
                Task.Delay(timeout).Wait();
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

            Misc.Utils.RunInBackground(() =>
            {
                try
                {
                    Task.Delay(timeout, tk).Wait();
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
                TryDoTheJob(Throttle);
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
            var ready = jobToken.WaitOne(timeout);
            waitingToken.Set();
            if (ready)
            {
                DoTheJob();
            }
            else
            {
                retry?.Invoke();
            }
        }

        void DebugAutoResetEvent(AutoResetEvent arEv, string evName)
        {
            while (!arEv.WaitOne(timeout + 2000))
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    var title = $"!suspectable deadlock! {Name} - {evName}";
                    Sys.FileLogger.DumpCallStack(title);
                }
                Task.Delay(100).Wait();
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

            try
            {
                if (chainedTask != null)
                {
                    AutoResetEvent chainEnd = new AutoResetEvent(false);
                    chainedTask?.Invoke(() => chainEnd.Set());
                    DebugAutoResetEvent(chainEnd, nameof(chainEnd));
                }
                else
                {
                    singleTask?.Invoke();
                }
            }
            finally
            {
                Done();
            }
            return;
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
