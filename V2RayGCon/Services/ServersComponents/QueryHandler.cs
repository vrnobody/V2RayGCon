using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VgcApis.Interfaces;

namespace V2RayGCon.Services.ServersComponents
{
    class QueryHandler
    {
        object serverListWriteLock;
        List<Controllers.CoreServerCtrl> coreServList;

        public QueryHandler(
            object serverListWriteLock,
            List<Controllers.CoreServerCtrl> coreServList)
        {
            this.serverListWriteLock = serverListWriteLock;
            this.coreServList = coreServList;
        }

        #region public methods
        public ReadOnlyCollection<ICoreServCtrl> GetRunningServers()
        {
            return AtomOp(() => GetAllServersOrderByIndex()
                .Where(s => s.GetCoreCtrl().IsCoreRunning())
                .ToList());
        }

        public ReadOnlyCollection<ICoreServCtrl> GetAllServersOrderByIndex()
        {
            return AtomOp(() => coreServList.Select(s => s as ICoreServCtrl)
                .OrderBy(s => s.GetCoreStates().GetIndex())
                .ToList());
        }

        public ReadOnlyCollection<ICoreServCtrl> GetSelectedServers(
           bool descending = false)
        {
            return AtomOp(() =>
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

        public ReadOnlyCollection<ICoreServCtrl> GetTrackableServerList()
        {
            return AtomOp(
                () => coreServList
                .Where(s => s.GetCoreCtrl().IsCoreRunning() && !s.GetCoreStates().IsUntrack())
                .Select(s => s as ICoreServCtrl)
                .ToList());
        }

        #endregion

        #region private methods

        ReadOnlyCollection<ICoreServCtrl> AtomOp(Func<List<ICoreServCtrl>> op)
        {
            List<ICoreServCtrl> r = null;
            lock (serverListWriteLock)
            {
                r = op?.Invoke();
            }
            return r.AsReadOnly();
        }

        #endregion
    }
}
