using System;
using System.Threading;
using System.Threading.Tasks;

namespace VgcApis.Libs.Tasks
{
    public class CancelableTask
    {
        // https://codereview.stackexchange.com/questions/117743/background-task-with-instant-abort-capability-in-c
        // Declare cancellation token and Cancellation token sources:
        private Thread _executionThread = null;
        private readonly Action _worker = null;

        // During initialization of the class assign token:
        public CancelableTask(Action worker)
        {
            _worker = worker;
        }

        #region public method
        //During the task initialization pass the token and specify TaskCreationOption.LongRunning:
        public void Start()
        {
            if (_executionThread != null)
            {
                return;
            }
            RunQuietly(this.LongRunningTaskWrapper);
        }
        #endregion

        #region private method
        public static void RunQuietly(Action worker)
        {
            void job()
            {
                try
                {
                    worker?.Invoke();
                }
                catch (ThreadAbortException) { }
                catch (Exception e)
                {
                    Sys.FileLogger.Error($"CancelableTask error:\n{e}");
                }
            }

            try
            {
                var t = new Task(job, TaskCreationOptions.LongRunning);
                t.ConfigureAwait(false);
                t.Start();
                return;
            }
            catch (Exception e)
            {
                Sys.FileLogger.Error($"Create CancelableTask error:\n{e}");
            }
        }

        // Suppress the exception if required, depends on the business use case:
        private void LongRunningTaskWrapper()
        {
            _executionThread = Thread.CurrentThread;
            try
            {
                _worker?.Invoke();
                // do long running job here
            }
            catch (ThreadAbortException)
            {
                // stop when aborted
            }
            _executionThread = null;
        }

        public void Stop()
        {
            try
            {
                _executionThread?.Abort();
            }
            catch { }
        }
        #endregion
    }
}
