using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VgcApis.Models.Interfaces;

namespace V2RayGCon.Service.ServersComponents
{
    class QueryHandler
    {
        object serverListWriteLock;
        List<Controller.CoreServerCtrl> coreServList;

        public QueryHandler(
            object serverListWriteLock,
            List<Controller.CoreServerCtrl> coreServList)
        {
            this.serverListWriteLock = serverListWriteLock;
            this.coreServList = coreServList;
        }

        #region public methods
        public ReadOnlyCollection<ICoreServCtrl> GetRunningServers()
        {
            return GetAllServersOrderByIndex()
                .Where(s => s.GetCoreCtrl().IsCoreRunning())
                .ToList()
                .AsReadOnly();
        }

        public ReadOnlyCollection<ICoreServCtrl> GetAllServersOrderByIndex() =>
           coreServList
           .Select(s => s as ICoreServCtrl)
           .OrderBy(s => s.GetCoreStates().GetIndex())
           .ToList()
           .AsReadOnly();


        public ReadOnlyCollection<ICoreServCtrl> GetSelectedServers(
           bool descending = false)
        {
            var list = coreServList.Where(s => s.GetCoreStates().IsSelected());

            var orderedList = descending ?
                list.OrderByDescending(s => s.GetCoreStates().GetIndex()) :
                list.OrderBy(s => s.GetCoreStates().GetIndex());

            return orderedList
                .Select(s => s as ICoreServCtrl)
                .ToList()
                .AsReadOnly();
        }

        public ReadOnlyCollection<VgcApis.Models.Interfaces.ICoreServCtrl>
            GetTrackableServerList()
        {
            lock (serverListWriteLock)
            {
                return coreServList
                    .Where(s => s.GetCoreCtrl().IsCoreRunning() && !s.GetCoreStates().IsUntrack())
                    .Select(s => s as VgcApis.Models.Interfaces.ICoreServCtrl)
                    .ToList()
                    .AsReadOnly();
            }
        }

        #endregion

        #region private methods

        #endregion
    }
}
