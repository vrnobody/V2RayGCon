using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace VgcApis.Libs.Infr
{
    public class ImportResultRecorder
    {
        readonly ConcurrentQueue<string[]> results = new ConcurrentQueue<string[]>();
        readonly Dictionary<string, int> reasons = new Dictionary<string, int>();

        int cntOk = 0;
        int cntFails = 0;
        private string errorMessage = null;

        public ImportResultRecorder() { }

        #region public methods

        public string GetErrorMessage() => errorMessage;

        public void SetErrorMessage(string msg)
        {
            if (errorMessage == null)
            {
                errorMessage = msg;
            }
        }

        public List<string[]> GetResults() => results.ToList();

        public string GetReasonsSubTotal()
        {
            lock (this.reasons)
            {
                return string.Join(", ", reasons.Select(kv => $"{kv.Key}: {kv.Value}"));
            }
        }

        public int GetTotalCount() => cntFails + cntOk;

        public int GetSuccessCount() => cntOk;

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
            CountReason(reason);

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

        #region private methods
        void CountReason(string reason)
        {
            lock (reasons)
            {
                if (reasons.ContainsKey(reason))
                {
                    reasons[reason]++;
                }
                else
                {
                    reasons[reason] = 1;
                }
            }
        }
        #endregion
    }
}
