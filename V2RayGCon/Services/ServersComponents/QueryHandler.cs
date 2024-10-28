using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using VgcApis.Interfaces;

namespace V2RayGCon.Services.ServersComponents
{
    class QueryHandler
    {
        readonly ReaderWriterLockSlim locker;
        readonly Dictionary<string, Controllers.CoreServerCtrl> coreServCache;

        public QueryHandler(
            ReaderWriterLockSlim locker,
            Dictionary<string, Controllers.CoreServerCtrl> coreServList
        )
        {
            this.locker = locker;
            this.coreServCache = coreServList;
        }

        #region public methods

        public List<ICoreServCtrl> GetServers(
            Func<KeyValuePair<string, Controllers.CoreServerCtrl>, bool> predicate,
            bool? isDescending = null
        )
        {
            return AtomicReader(() =>
            {
                var list = coreServCache.AsEnumerable();

                if (predicate != null)
                {
                    list = list.Where(predicate);
                }

                if (isDescending != null)
                {
                    list =
                        isDescending == false
                            ? list.OrderBy(kv => kv.Value)
                            : list.OrderByDescending(kv => kv.Value);
                }
                return list.Select(kv => kv.Value as ICoreServCtrl).ToList();
            });
        }

        public List<ICoreServCtrl> GetRunningServers() =>
            GetServers(kv => kv.Value.GetCoreCtrl().IsCoreRunning());

        public List<ICoreServCtrl> GetAllServers(bool? isDescending = null) =>
            GetServers(null, isDescending);

        public ICoreServCtrl GetServersByUid(string uid)
        {
            locker.EnterReadLock();
            try
            {
                if (coreServCache.TryGetValue(uid, out var coreServ))
                {
                    return coreServ;
                }
            }
            finally
            {
                locker.ExitReadLock();
            }
            return null;
        }

        public List<ICoreServCtrl> GetServersByUids(IEnumerable<string> uids)
        {
            var r = new List<ICoreServCtrl>();
            locker.EnterReadLock();
            try
            {
                foreach (var uid in uids)
                {
                    if (
                        !string.IsNullOrEmpty(uid)
                        && coreServCache.TryGetValue(uid, out var coreServ)
                        && coreServ != null
                    )
                    {
                        r.Add(coreServ);
                    }
                }
            }
            finally
            {
                locker.ExitReadLock();
            }
            return r;
        }

        public List<ICoreServCtrl> GetSelectedServers() =>
            GetServers(kv => kv.Value.GetCoreStates().IsSelected());

        public List<ICoreServCtrl> GetTrackableServerList() =>
            GetServers(kv =>
                kv.Value.GetCoreCtrl().IsCoreRunning() && !kv.Value.GetCoreStates().IsUntrack()
            );

        #endregion

        #region private methods
        List<ICoreServCtrl> AtomicReader(Func<List<ICoreServCtrl>> op)
        {
            List<ICoreServCtrl> r = null;
            locker.EnterReadLock();
            try
            {
                r = op?.Invoke();
            }
            finally
            {
                locker.ExitReadLock();
            }
            return r;
        }

        #endregion
    }
}
