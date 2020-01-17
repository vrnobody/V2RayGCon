using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Statistics.Services
{
    public class Settings
    {
        VgcApis.Models.IServices.ISettingsService vgcSetting;
        VgcApis.Models.IServices.IServersService vgcServers;

        Models.UserSettings userSettins;
        VgcApis.Libs.Tasks.LazyGuy bookKeeper;
        Timer bgStatsDataUpdateTimer = null;

        #region properties
        public int statsDataUpdateInterval { get; } = 2000;
        public bool isRequireClearStatsData { get; set; } = false;
        const int bgStatsDataUpdateInterval = 5 * 60 * 1000;
        #endregion

        #region public method
        public bool IsShutdown() => vgcSetting.IsClosing();

        public void RequireHistoryStatsDataUpdate()
        {
            if (isUpdating)
            {
                return;
            }
            UpdateHistoryStatsDataWorker();
        }

        void ClearStatsDataOnDemand()
        {
            if (!isRequireClearStatsData)
            {
                return;
            }

            userSettins.statsData =
                new Dictionary<string, Models.StatsResult>();
            isRequireClearStatsData = false;
        }

        public Dictionary<string, Models.StatsResult> GetAllStatsData()
        {
            return userSettins.statsData;
        }

        public void Run(
            VgcApis.Models.IServices.ISettingsService vgcSetting,
            VgcApis.Models.IServices.IServersService vgcServers)
        {
            this.vgcSetting = vgcSetting;
            this.vgcServers = vgcServers;

            userSettins = LoadUserSetting();
            bookKeeper = new VgcApis.Libs.Tasks.LazyGuy(
                SaveUserSetting, VgcApis.Models.Consts.Intervals.LazySaveStatisticsDatadelay);
            StartBgStatsDataUpdateTimer();
            vgcServers.OnCoreClosing += SaveStatDataBeforeCoreClosed;
        }

        public void Cleanup()
        {
            vgcServers.OnCoreClosing -= SaveStatDataBeforeCoreClosed;
            ReleaseBgStatsDataUpdateTimer();

            // Calling v2ctl.exe at shutdown can cause problems.
            // So losing 5 minutes of statistics data is an acceptable loss.
            if (!IsShutdown())
            {
                VgcApis.Libs.Sys.FileLogger.Info("Statistics: save data");
                UpdateHistoryStatsDataWorker();
                bookKeeper.DoItNow();
            }

            bookKeeper.Quit();
            VgcApis.Libs.Sys.FileLogger.Info("Statistics: done!");
        }
        #endregion

        #region private method
        void SaveStatDataBeforeCoreClosed(object sender, EventArgs args)
        {
            var coreCtrl = sender as VgcApis.Models.Interfaces.ICoreServCtrl;
            if (coreCtrl == null)
            {
                return;
            }

            var uid = coreCtrl.GetCoreStates().GetUid();
            var sample = coreCtrl.GetCoreCtrl().TakeStatisticsSample();
            var title = coreCtrl.GetCoreStates().GetTitle();
            VgcApis.Libs.Utils.RunInBackground(
                () => AddToHistoryStatsData(uid, title, sample));
        }

        void AddToHistoryStatsData(
            string uid,
            string title,
            VgcApis.Models.Datas.StatsSample sample)
        {
            if (sample == null)
            {
                return;
            }

            var datas = userSettins.statsData;
            if (datas.ContainsKey(uid))
            {
                datas[uid].totalDown += sample.statsDownlink;
                datas[uid].totalUp += sample.statsUplink;
                return;
            }
            datas[uid] = new Models.StatsResult
            {
                uid = uid,
                title = title,
                totalDown = sample.statsDownlink,
                totalUp = sample.statsUplink,
            };
        }

        void BgStatsDataUpdateHandler(object sender, EventArgs args)
        {
            if (isUpdating)
            {
                return;
            }
            RequireHistoryStatsDataUpdate();
        }

        void StartBgStatsDataUpdateTimer()
        {
            bgStatsDataUpdateTimer = new Timer
            {
                Interval = bgStatsDataUpdateInterval,
            };
            bgStatsDataUpdateTimer.Start();
        }

        void ReleaseBgStatsDataUpdateTimer()
        {
            if (bgStatsDataUpdateTimer == null)
            {
                return;
            }
            bgStatsDataUpdateTimer.Stop();
            bgStatsDataUpdateTimer.Elapsed -= BgStatsDataUpdateHandler;
            bgStatsDataUpdateTimer.Dispose();
        }

        bool isUpdating = false;
        readonly object updateHistoryStatsDataLocker = new object();
        void UpdateHistoryStatsDataWorker()
        {
            lock (updateHistoryStatsDataLocker)
            {
                isUpdating = true;
                var newDatas = vgcServers
                    .GetAllServersOrderByIndex()
                    .Where(s => s.GetCoreCtrl().IsCoreRunning())
                    .Select(s => GetterCoreInfo(s))
                    .ToList();

                ClearStatsDataOnDemand();

                var historyDatas = userSettins.statsData;
                ResetCurSpeed(historyDatas);

                foreach (var d in newDatas)
                {
                    var uid = d.uid;
                    if (!historyDatas.ContainsKey(uid))
                    {
                        historyDatas[uid] = d;
                        continue;
                    }
                    MergeNewDataIntoHistoryData(historyDatas, d, uid);
                }

                bookKeeper.DoItLater();
                isUpdating = false;
            }
        }

        void MergeNewDataIntoHistoryData(
            Dictionary<string, Models.StatsResult> datas,
            Models.StatsResult statsResult,
            string uid)
        {
            var p = datas[uid];

            var elapse = 1.0 * (statsResult.stamp - p.stamp) / TimeSpan.TicksPerSecond;
            if (elapse <= 1)
            {
                elapse = statsDataUpdateInterval / 1000.0;
            }

            var downSpeed = (statsResult.totalDown / elapse) / 1024.0;
            var upSpeed = (statsResult.totalUp / elapse) / 1024.0;
            p.curDownSpeed = Math.Max(0, (int)downSpeed);
            p.curUpSpeed = Math.Max(0, (int)upSpeed);
            p.stamp = statsResult.stamp;
            p.totalDown = p.totalDown + statsResult.totalDown;
            p.totalUp = p.totalUp + statsResult.totalUp;
        }

        void ResetCurSpeed(Dictionary<string, Models.StatsResult> datas)
        {
            foreach (var data in datas)
            {
                data.Value.curDownSpeed = 0;
                data.Value.curUpSpeed = 0;
            }
        }

        Models.StatsResult GetterCoreInfo(VgcApis.Models.Interfaces.ICoreServCtrl coreCtrl)
        {
            var result = new Models.StatsResult();
            result.title = coreCtrl.GetCoreStates().GetTitle();
            result.uid = coreCtrl.GetCoreStates().GetUid();

            var curData = coreCtrl.GetCoreCtrl().TakeStatisticsSample();
            if (curData != null)
            {
                result.stamp = curData.stamp;
                result.totalUp = curData.statsUplink;
                result.totalDown = curData.statsDownlink;
            }
            return result;
        }

        void SaveUserSetting()
        {
            vgcSetting.SavePluginsSetting(
                Properties.Resources.Name,
                VgcApis.Libs.Utils.SerializeObject(userSettins));
        }

        Models.UserSettings LoadUserSetting()
        {
            string uss = vgcSetting.GetPluginsSetting(
                Properties.Resources.Name);
            try
            {
                var us = VgcApis.Libs.Utils
                    .DeserializeObject<Models.UserSettings>(uss);
                if (us != null)
                {
                    return us;
                }
            }
            catch { }
            return new Models.UserSettings();
        }
        #endregion
    }
}
