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

            Task.Run(async () =>
            {
                await Task.Delay(timeout);
                jobToken.WaitOne();
                waitingToken.Set();
                DoTheJob();
            }).ConfigureAwait(false);
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

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(timeout, tk);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                if (tk.IsCancellationRequested || !waitingToken.WaitOne(0))
                {
                    return;
                }

                jobToken.WaitOne();
                waitingToken.Set();
                DoTheJob();

            }).ConfigureAwait(false);
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

            Task.Run(() =>
            {
                jobToken.WaitOne();
                waitingToken.Set();
                DoTheJob();
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// blocking
        /// </summary>
        public void DoItNow()
        {
            jobToken.WaitOne();
            DoTheJob();
        }

        #endregion

        #region private method

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
                    chainEnd.WaitOne();
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
