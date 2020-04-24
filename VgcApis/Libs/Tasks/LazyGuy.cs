using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                DebugAutoResetEvent(jobToken, "jobToken");
                waitingToken.Set();
                DoTheJob();
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

                if (tk.IsCancellationRequested || !waitingToken.WaitOne(0))
                {
                    return;
                }

                DebugAutoResetEvent(jobToken, "jobToken");
                waitingToken.Set();
                DoTheJob();
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
                DebugAutoResetEvent(jobToken, "jobToken");
                waitingToken.Set();
                DoTheJob();
            });
        }



        /// <summary>
        /// blocking
        /// </summary>
        public void DoItNow()
        {
            DebugAutoResetEvent(jobToken, "jobToken");
            DoTheJob();
        }

        #endregion

        #region private method

        void DebugAutoResetEvent(AutoResetEvent arEv, string evName = @"")
        {
            while (!arEv.WaitOne(timeout + 2000))
            {
                var title = string.IsNullOrEmpty(evName) ?
                    $"!suspectable deadlock! {Name}" :
                    $"!suspectable deadlock! {Name} - {evName}";

                Sys.FileLogger.DumpCallStack(title);
                Application.DoEvents();
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
                    DebugAutoResetEvent(chainEnd, "chainEnd");
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
