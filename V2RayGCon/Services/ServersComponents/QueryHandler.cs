using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VgcApis.Interfaces;

namespace V2RayGCon.Services.ServersComponents
{
    class QueryHandler
    {
        ReaderWriterLockSlim locker;
        Dictionary<string, Controllers.CoreServerCtrl> coreServCache;

        public QueryHandler(
            ReaderWriterLockSlim locker,
            Dictionary<string, Controllers.CoreServerCtrl> coreServList)
        {
            this.locker = locker;
            this.coreServCache = coreServList;
        }

        #region public methods
        public List<ICoreServCtrl> GetRunningServers()
        {
            return AtomicReader(() => coreServCache
                .Where(kv => kv.Value.GetCoreCtrl().IsCoreRunning())
                .Select(kv => kv.Value as ICoreServCtrl)
                .OrderBy(s => s.GetCoreStates().GetIndex())
                .ToList());
        }

        public List<ICoreServCtrl> GetAllServersOrderByIndex()
        {
            return AtomicReader(() => coreServCache
                .Select(kv => kv.Value as ICoreServCtrl)
                .OrderBy(s => s.GetCoreStates().GetIndex())
                .ToList());
        }

        public List<ICoreServCtrl> GetServersByUids(IEnumerable<string> uids)
        {
            var r = new List<ICoreServCtrl>();
            locker.EnterReadLock();
            try
            {
                foreach (var uid in uids)
                {
                    if (!string.IsNullOrEmpty(uid) && coreServCache.TryGetValue(uid, out var coreServ))
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

        public List<ICoreServCtrl> GetSelectedServers(
           bool descending = false)
        {
            return AtomicReader(() =>
            {
                var list = coreServCache.Where(kv => kv.Value.GetCoreStates().IsSelected());

                var orderedList = descending ?
                    list.OrderByDescending(kv => kv.Value.GetCoreStates().GetIndex()) :
                    list.OrderBy(kv => kv.Value.GetCoreStates().GetIndex());

                return orderedList
                    .Select(kv => kv.Value as ICoreServCtrl)
                    .ToList();
            });
        }

        public List<ICoreServCtrl> GetTrackableServerList()
        {
            return AtomicReader(
                () => coreServCache
                .Where(kv => kv.Value.GetCoreCtrl().IsCoreRunning() && !kv.Value.GetCoreStates().IsUntrack())
                .Select(kv => kv.Value as ICoreServCtrl)
                .ToList());
        }

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
