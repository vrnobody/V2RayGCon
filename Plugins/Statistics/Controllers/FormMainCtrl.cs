using Statistics.Resources.Langs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Statistics.Controllers
{
    public class FormMainCtrl
    {
        Services.Settings settings;

        ListView dataView;
        ToolStripMenuItem miReset, miResizeByTitle, miResizeByContent;

        Timer updateDataViewTimer = null;

        bool[] sortFlags = new bool[] { false, false, false, false, false };
        string[] dataViewHeaders = null;

        public FormMainCtrl(
            Services.Settings settings,

            ListView dataView,

            ToolStripMenuItem miReset,
            ToolStripMenuItem miResizeByTitle,
            ToolStripMenuItem miResizeByContent)
        {
            this.settings = settings;

            this.dataView = dataView;

            this.miReset = miReset;
            this.miResizeByContent = miResizeByContent;
            this.miResizeByTitle = miResizeByTitle;

            dataViewHeaders = GetDataViewHeaders();
        }

        #region public methods
        public void Cleanup()
        {
            ReleaseUpdateTimer();
        }

        public void Run()
        {
            ResizeDataViewByTitle();
            BindControlEvent();
            ShowStatsDataOnDataView();
            StartUpdateTimer();
        }
        #endregion

        #region private methods
        string[] GetDataViewHeaders()
        {
            var headers = new List<string>();
            for (int i = 0; i < dataView.Columns.Count; i++)
            {
                headers.Add(dataView.Columns[i].Text);
            }
            return headers.ToArray();
        }

        void StartUpdateTimer()
        {
            updateDataViewTimer = new Timer
            {
                Interval = settings.statsDataUpdateInterval,
            };
            updateDataViewTimer.Tick += UpdateDataViewHandler;
            updateDataViewTimer.Start();
        }

        private void BindControlEvent()
        {
            miReset.Click += (s, a) =>
            {
                VgcApis.Libs.Utils.RunInBackground(() =>
                {
                    if (VgcApis.Libs.UI.Confirm(I18N.ConfirmResetStatsData))
                    {
                        settings.isRequireClearStatsData = true;
                    }
                });
            };

            miResizeByContent.Click += (s, a) => ResizeDataViewByContent();

            miResizeByTitle.Click += (s, a) => ResizeDataViewByTitle();

            dataView.ColumnClick += ColumnClickHandler;
        }

        void ColumnClickHandler(object sender, ColumnClickEventArgs args)
        {
            var index = args.Column;
            if (index < 0 || index > 4)
            {
                return;
            }
            sortFlags[index] = !sortFlags[index];
            dataViewOrderKeyIndex = index;
            UpdateColumnHeaders();
            ResizeDataViewByTitle();
            ShowStatsDataOnDataView();
        }

        void UpdateColumnHeaders()
        {
            for (var i = 1; i < 5; i++)
            {
                var title = dataViewHeaders[i];
                if (i == dataViewOrderKeyIndex)
                {
                    title += " " + (sortFlags[i] ? "▲" : "▼");
                }
                UpdateColumnHeaderTextOnDemand(i, title);
            }
        }

        void UpdateColumnHeaderTextOnDemand(int index, string text)
        {
            if (dataView.Columns[index].Text == text)
            {
                return;
            }
            dataView.Columns[index].Text = text;
        }

        void UpdateDataViewHandler(object sender, EventArgs args)
        {
            VgcApis.Libs.Utils.RunInBackground(UpdateDataViewWorker);
        }

        void ReleaseUpdateTimer()
        {
            if (updateDataViewTimer == null)
            {
                return;
            }

            updateDataViewTimer.Stop();
            updateDataViewTimer.Tick -= UpdateDataViewHandler;
            updateDataViewTimer.Dispose();
        }

        bool isUpdating = false;
        readonly object updateDataViewLocker = new object();
        void UpdateDataViewWorker()
        {
            lock (updateDataViewLocker)
            {
                if (isUpdating)
                {
                    return;
                }
                isUpdating = true;
            }

            settings.RequireHistoryStatsDataUpdate();
            ShowStatsDataOnDataView();

            lock (updateDataViewLocker)
            {
                isUpdating = false;
            }
        }

        int dataViewOrderKeyIndex = 0;
        void ShowStatsDataOnDataView()
        {
            var lvContent = GetSortedHistoryData()
                .Select(e => new ListViewItem(e))
                .ToArray();

            BatchUpdateDataView(() =>
            {
                dataView.Items.Clear();
                dataView.Items.AddRange(lvContent);
            });
        }

        IEnumerable<string[]> GetSortedHistoryData()
        {
            const int MiB = 1024 * 1024;
            var contents = settings.GetAllStatsData()
                .Select(d =>
                {
                    var v = d.Value;
                    return new string[] {
                        v.title,
                        v.curDownSpeed.ToString(),
                        v.curUpSpeed.ToString(),
                        (v.totalDown/MiB).ToString(),
                        (v.totalUp/MiB).ToString(),
                    };
                });

            var index = dataViewOrderKeyIndex;
            if (index == 0)
            {
                return contents;
            }

            return sortFlags[index] ?
                contents.OrderBy(e => VgcApis.Libs.Utils.Str2Int(e[index])) :
                contents.OrderByDescending(e => VgcApis.Libs.Utils.Str2Int(e[index]));
        }

        void BatchUpdateDataView(Action action)
        {
            VgcApis.Libs.UI.RunInUiThread(dataView, () =>
            {
                dataView.BeginUpdate();
                try
                {
                    action?.Invoke();
                }
                catch { }
                finally
                {
                    dataView.EndUpdate();
                }
            });
        }

        void ResizeDataViewColumn(bool byTitle)
        {
            var mode = byTitle ?
                ColumnHeaderAutoResizeStyle.HeaderSize :
                ColumnHeaderAutoResizeStyle.ColumnContent;

            var count = dataView.Columns.Count;
            for (int i = 1; i < count; i++)
            {
                dataView.Columns[i].AutoResize(mode);
            }
        }

        void ResizeDataViewByContent()
        {
            BatchUpdateDataView(
                () => ResizeDataViewColumn(false));
        }

        void ResizeDataViewByTitle()
        {
            BatchUpdateDataView(
                () => ResizeDataViewColumn(true));
        }

        #endregion
    }
}
