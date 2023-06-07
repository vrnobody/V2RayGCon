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
        List<Controllers.CoreServerCtrl> coreServList;

        public QueryHandler(
            ReaderWriterLockSlim locker,
            List<Controllers.CoreServerCtrl> coreServList)
        {
            this.locker = locker;
            this.coreServList = coreServList;
        }

        #region public methods
        public List<ICoreServCtrl> GetRunningServers()
        {
            return AtomicReader(() => coreServList
                .Where(s => s.GetCoreCtrl().IsCoreRunning())
                .OrderBy(s => s.GetCoreStates().GetIndex())
                .Select(s => s as ICoreServCtrl)
                .ToList());
        }

        public List<ICoreServCtrl> GetAllServersOrderByIndex()
        {
            return AtomicReader(() => coreServList
                .OrderBy(s => s.GetCoreStates().GetIndex())
                .Select(s => s as ICoreServCtrl)
                .ToList());
        }

        public List<ICoreServCtrl> GetServersByUids(IEnumerable<string> uids)
        {
            var set = new HashSet<string>(uids);
            return AtomicReader(() => coreServList
                .Where(cs => set.Contains(cs.GetCoreStates().GetUid()))
                .Select(cs => cs as ICoreServCtrl)
                .ToList());
        }

        public List<ICoreServCtrl> GetSelectedServers(
           bool descending = false)
        {
            return AtomicReader(() =>
            {
                var list = coreServList.Where(s => s.GetCoreStates().IsSelected());

                var orderedList = descending ?
                    list.OrderByDescending(s => s.GetCoreStates().GetIndex()) :
                    list.OrderBy(s => s.GetCoreStates().GetIndex());

                return orderedList
                    .Select(s => s as ICoreServCtrl)
                    .ToList();
            });
        }

        public List<ICoreServCtrl> GetTrackableServerList()
        {
            return AtomicReader(
                () => coreServList
                .Where(s => s.GetCoreCtrl().IsCoreRunning() && !s.GetCoreStates().IsUntrack())
                .Select(s => s as ICoreServCtrl)
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
