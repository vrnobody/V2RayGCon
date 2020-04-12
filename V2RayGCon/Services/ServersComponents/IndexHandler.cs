using System;
using System.Collections.Generic;
using System.Linq;

namespace V2RayGCon.Services.ServersComponents
{
    internal sealed class IndexHandler
    {
        readonly object writeLocker;
        List<Controllers.CoreServerCtrl> coreServList;

        public IndexHandler(
            object writeLocker,
            List<Controllers.CoreServerCtrl> coreServList)
        {
            this.writeLocker = writeLocker;
            this.coreServList = coreServList;
        }

        #region properties

        #endregion

        #region public methods
        public void SortCoreServCtrlListBySpeedTestResult(
           ref List<VgcApis.Interfaces.ICoreServCtrl> coreList)
        {
            lock (writeLocker)
            {
                SortServerItemList(ref coreList, SpeedTestComparer);
            }
        }

        public void ReverseCoreservCtrlListByIndex(
            ref List<VgcApis.Interfaces.ICoreServCtrl> coreList)
        {
            lock (writeLocker)
            {
                SortServerItemList(ref coreList, ReverseIndexComparer);
            }
        }

        public void SortCoreServerCtrlListByLastModifyDate(
         ref List<VgcApis.Interfaces.ICoreServCtrl> coreList)
        {
            lock (writeLocker)
            {
                SortServerItemList(ref coreList, UtcTicksDecComparer);
            }
        }

        public void SortCoreServCtrlListBySummary(
           ref List<VgcApis.Interfaces.ICoreServCtrl> coreList)
        {
            lock (writeLocker)
            {
                SortServerItemList(ref coreList, SummaryComparer);
            }
        }


        public void ResetIndex()
        {
            var sortedServers = coreServList
                .OrderBy(c => c.GetCoreStates().GetIndex())
                .ToList();

            lock (writeLocker)
            {
                for (int i = 0; i < sortedServers.Count(); i++)
                {
                    var index = i + 1.0; // closure
                    sortedServers[i]
                        .GetCoreStates()
                        .SetIndex(index);
                }
            }
        }

        public void ResetIndexQuiet()
        {
            var sortedServers = coreServList
                .OrderBy(c => c.GetCoreStates().GetIndex())
                .ToList();

            lock (writeLocker)
            {
                for (int i = 0; i < sortedServers.Count(); i++)
                {
                    var index = i + 1.0; // closure
                    sortedServers[i]
                        .GetCoreStates()
                        .SetIndexQuiet(index);
                }
            }
        }
        #endregion

        #region private methods
        int ReverseIndexComparer(
           VgcApis.Interfaces.ICoreServCtrl a,
           VgcApis.Interfaces.ICoreServCtrl b)
        {
            var idxA = a.GetCoreStates().GetIndex();
            var idxB = b.GetCoreStates().GetIndex();
            return idxB.CompareTo(idxA);
        }

        int SpeedTestComparer(
            VgcApis.Interfaces.ICoreServCtrl a,
            VgcApis.Interfaces.ICoreServCtrl b)
        {
            var spa = a.GetCoreStates().GetSpeedTestResult();
            var spb = b.GetCoreStates().GetSpeedTestResult();
            return spa.CompareTo(spb);
        }

        int UtcTicksDecComparer(
           VgcApis.Interfaces.ICoreServCtrl a,
           VgcApis.Interfaces.ICoreServCtrl b)
        {
            var ticksA = a.GetCoreStates().GetLastModifiedUtcTicks();
            var ticksB = b.GetCoreStates().GetLastModifiedUtcTicks();
            return ticksB.CompareTo(ticksA);
        }

        int SummaryComparer(
            VgcApis.Interfaces.ICoreServCtrl a,
            VgcApis.Interfaces.ICoreServCtrl b)
        {
            var sma = a.GetCoreStates().GetSummary();
            var smb = b.GetCoreStates().GetSummary();

            var rsma = VgcApis.Misc.Utils.ReverseSummary(sma);
            var rsmb = VgcApis.Misc.Utils.ReverseSummary(smb);

            return rsma.CompareTo(rsmb);
        }

        void SortServerItemList(
             ref List<VgcApis.Interfaces.ICoreServCtrl> selectedServers,
             Comparison<VgcApis.Interfaces.ICoreServCtrl> comparer)
        {
            if (selectedServers == null || selectedServers.Count() < 2)
            {
                return;
            }

            selectedServers.Sort(comparer);
            var minIndex = selectedServers.Select(s => s.GetCoreStates().GetIndex()).Min();
            var delta = 1.0 / 2 / selectedServers.Count;
            for (int i = 0; i < selectedServers.Count; i++)
            {
                selectedServers[i].GetCoreStates()
                    .SetIndexQuiet(minIndex + delta * (i + 1));
            }
            ResetIndexQuiet();
        }
        #endregion

        #region protected methods

        #endregion
    }
}
