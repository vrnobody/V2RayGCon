using System;

namespace VgcApis.Libs.Tasks
{
    public class LazyGuy
    {
        Action task = null;
        int timeout = -1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task">()=>{ ... }</param>
        /// <param name="timeout">millisecond</param>
        public LazyGuy(Action task, int timeout)
        {
            if (task == null || timeout < 1)
            {
                throw new ArgumentException("I am not that lazy!");
            }

            this.task = task;
            this.timeout = timeout;

        }

        #region properties
        readonly object taskLocker = new object();

        CancelableTimeout _lazyTimer = null;
        CancelableTimeout lazyTimer
        {
            get
            {
                if (_lazyTimer == null)
                {
                    _lazyTimer = new CancelableTimeout(task, timeout);
                }
                return _lazyTimer;
            }
        }
        #endregion

        #region public method
        public void ForgetIt()
        {
            lazyTimer.Cancel();
        }

        public void Remember(Action task)
        {
            this.task = task;
        }

        public void DoItLater()
        {
            lazyTimer.Start();
        }

        bool cancel = false;
        public void DoItNow()
        {
            lazyTimer.Cancel();

            lock (taskLocker)
            {
                if (cancel)
                {
                    return;
                }
                task?.Invoke();
            }
        }

        public void Quit()
        {
            cancel = true;
            lock (taskLocker)
            {
                lazyTimer.Cancel();
                lazyTimer.Release();
                task = null;
            }
        }
        #endregion

        #region private method

        #endregion

    }
}
