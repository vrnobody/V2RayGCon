using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace V2RayGCon.Services.ServersComponents
{
    internal sealed class IndexHandler
    {
        ReaderWriterLockSlim locker;
        Dictionary<string, Controllers.CoreServerCtrl> coreServCache;

        public event EventHandler OnIndexChanged;

        public IndexHandler(
            ReaderWriterLockSlim locker,
            Dictionary<string, Controllers.CoreServerCtrl> coreServList
        )
        {
            this.locker = locker;
            this.coreServCache = coreServList;
        }

        #region properties

        #endregion

        #region public methods
        public void SortCoreServCtrlListBySpeedTestResult(
            ref List<VgcApis.Interfaces.ICoreServCtrl> coreList
        )
        {
            SortServerItemList(ref coreList, SpeedTestComparer);
        }

        public void ReverseCoreservCtrlListByIndex(
            ref List<VgcApis.Interfaces.ICoreServCtrl> coreList
        )
        {
            SortServerItemList(ref coreList, ReverseIndexComparer);
        }

        public void SortCoreServerCtrlListByDownloadTotal(
            ref List<VgcApis.Interfaces.ICoreServCtrl> coreList
        )
        {
            SortServerItemList(ref coreList, DownloadTotalDecComparer);
        }

        public void SortCoreServerCtrlListByUploadTotal(
            ref List<VgcApis.Interfaces.ICoreServCtrl> coreList
        )
        {
            SortServerItemList(ref coreList, UploadTotalDecComparer);
        }

        public void SortCoreServerCtrlListByLastModifyDate(
            ref List<VgcApis.Interfaces.ICoreServCtrl> coreList
        )
        {
            SortServerItemList(ref coreList, UtcTicksDecComparer);
        }

        public void SortCoreServCtrlListBySummary(
            ref List<VgcApis.Interfaces.ICoreServCtrl> coreList
        )
        {
            SortServerItemList(ref coreList, SummaryComparer);
        }

        public void ResetIndex(bool isQuiet) => ResetIndexWorker(isQuiet);

        #endregion

        #region private methods
        void ResetIndexWorker(bool isQuiet)
        {
            var coreServs = new List<VgcApis.Interfaces.CoreCtrlComponents.ICoreStates>();

            locker.EnterReadLock();
            try
            {
                coreServs = coreServCache
                    .OrderBy(kv => kv.Value)
                    .Select(kv => kv.Value.GetCoreStates())
                    .ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }

            // 不要RunInBackground!! 2023-09-09
            double idx = 0;
            if (isQuiet)
            {
                foreach (var coreServ in coreServs)
                {
                    coreServ.SetIndexQuiet(++idx);
                }
            }
            else
            {
                foreach (var coreServ in coreServs)
                {
                    coreServ.SetIndex(++idx);
                }
            }

            InvokeOnIndexChangedHandlerIgnoreError();
        }

        void InvokeOnIndexChangedHandlerIgnoreError()
        {
            try
            {
                OnIndexChanged?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }

        int ReverseIndexComparer(
            VgcApis.Interfaces.ICoreServCtrl a,
            VgcApis.Interfaces.ICoreServCtrl b
        )
        {
            var idxA = a.GetCoreStates().GetIndex();
            var idxB = b.GetCoreStates().GetIndex();
            return idxB.CompareTo(idxA);
        }

        int SpeedTestComparer(
            VgcApis.Interfaces.ICoreServCtrl a,
            VgcApis.Interfaces.ICoreServCtrl b
        )
        {
            var spa = a.GetCoreStates().GetSpeedTestResult();
            var spb = b.GetCoreStates().GetSpeedTestResult();
            return spa.CompareTo(spb);
        }

        int DownloadTotalDecComparer(
            VgcApis.Interfaces.ICoreServCtrl a,
            VgcApis.Interfaces.ICoreServCtrl b
        )
        {
            var ticksA = a.GetCoreStates().GetDownlinkTotalInBytes();
            var ticksB = b.GetCoreStates().GetDownlinkTotalInBytes();
            return ticksB.CompareTo(ticksA);
        }

        int UploadTotalDecComparer(
            VgcApis.Interfaces.ICoreServCtrl a,
            VgcApis.Interfaces.ICoreServCtrl b
        )
        {
            var ticksA = a.GetCoreStates().GetUplinkTotalInBytes();
            var ticksB = b.GetCoreStates().GetUplinkTotalInBytes();
            return ticksB.CompareTo(ticksA);
        }

        int UtcTicksDecComparer(
            VgcApis.Interfaces.ICoreServCtrl a,
            VgcApis.Interfaces.ICoreServCtrl b
        )
        {
            var ticksA = a.GetCoreStates().GetLastModifiedUtcTicks();
            var ticksB = b.GetCoreStates().GetLastModifiedUtcTicks();
            return ticksB.CompareTo(ticksA);
        }

        int SummaryComparer(VgcApis.Interfaces.ICoreServCtrl a, VgcApis.Interfaces.ICoreServCtrl b)
        {
            var sma = a.GetCoreStates().GetSummary();
            var smb = b.GetCoreStates().GetSummary();

            var rsma = VgcApis.Misc.Utils.ReverseSummary(sma);
            var rsmb = VgcApis.Misc.Utils.ReverseSummary(smb);

            return rsma.CompareTo(rsmb);
        }

        void SortServerItemList(
            ref List<VgcApis.Interfaces.ICoreServCtrl> selectedServers,
            Comparison<VgcApis.Interfaces.ICoreServCtrl> comparer
        )
        {
            if (selectedServers == null || selectedServers.Count() < 2)
            {
                return;
            }
            locker.EnterWriteLock();
            try
            {
                selectedServers.Sort(comparer);
                var minIndex = selectedServers.Min(s => s.GetCoreStates().GetIndex());
                var delta = 1.0 / 2 / selectedServers.Count;
                for (int i = 0; i < selectedServers.Count; i++)
                {
                    selectedServers[i].GetCoreStates().SetIndexQuiet(minIndex + delta * (i + 1));
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }
            ResetIndexWorker(true);
        }
        #endregion

        #region protected methods

        #endregion
    }
}
