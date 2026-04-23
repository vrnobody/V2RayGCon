using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VgcApis.Libs.Infr
{
    public class ImportResultRecorder
    {
        readonly ConcurrentQueue<string[]> results = new ConcurrentQueue<string[]>();
        int cntOk = 0;
        int cntFails = 0;

        // 序列化需要这个ctor, 不要删除！
        public ImportResultRecorder() { }

        #region properties

        private string errorMessage = null;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        #endregion

        #region public methods

        public IEnumerable<string[]> GetResults() => results;

        public int CountFails() => cntFails;

        public int CountOk() => cntOk;

        public void Record(bool success)
        {
            if (success)
            {
                Interlocked.Increment(ref cntOk);
            }
            else
            {
                Interlocked.Increment(ref cntFails);
            }
        }

        public void Record(string mark, string link, bool success, string reason)
        {
            Record(success);

            var symbol = success
                ? Models.Consts.Import.MarkImportSuccess
                : Models.Consts.Import.MarkImportFail;

            var r = new string[]
            {
                string.Empty, // reserved for index
                link,
                mark,
                symbol, // be aware of IsImportResultSuccess()
                reason,
            };
            results.Enqueue(r);
        }
        #endregion
    }
}
