using System;

namespace VgcApis.Libs.Tasks
{
    public class LazyGuy
    {
        Action task = null;
        CancelableTimeout lazyTimer = null;

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
            lazyTimer = new CancelableTimeout(DoItNow, timeout);
        }

        #region public method
        public void ForgetIt()
        {
            lazyTimer.Cancel();
        }

        public void DoItLater()
        {
            lazyTimer.Start();
        }

        public void DoItNow()
        {
            lazyTimer.Cancel();
            task?.Invoke();
        }

        public void Quit()
        {
            task = null;
            lazyTimer.Cancel();
            lazyTimer.Release();
        }
        #endregion

        #region private method

        #endregion

    }
}
