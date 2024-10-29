using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormWebUI : Form
    {
        #region Sigleton
        static readonly VgcApis.BaseClasses.AuxSiWinForm<FormWebUI> auxSiForm =
            new VgcApis.BaseClasses.AuxSiWinForm<FormWebUI>();

        public static FormWebUI GetForm() => auxSiForm.GetForm();

        public static void ShowForm() => auxSiForm.ShowForm();
        #endregion

        private Services.Settings settings;

        public FormWebUI()
        {
            this.settings = Services.Settings.Instance;
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private async void FormWeb_Load(object sender, EventArgs args)
        {
            settings.RestoreFormRect(this);
            webView2.SourceChanged += (s, a) => this.Text = $"WebUI {webView2.Source}";
            await InitWebView();
            Navigate();
        }

        private void FormWebUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.SaveFormRect(this);
        }

        #region private methods
        void Navigate()
        {
            var url = settings.SystrayLeftClickCommand;
            try
            {
                if (Regex.IsMatch(url, VgcApis.Models.Consts.Patterns.VgcWebUiUrl))
                {
                    var prefix = VgcApis.Models.Consts.Patterns.VgcWebUiUrlPrefix;
                    url = $"http{url.Substring(prefix.Length)}";
                    if (!string.IsNullOrEmpty(url))
                    {
                        this.webView2.Source = new Uri(url);
                    }
                }
            }
            catch
            {
                Close();
            }
        }

        async Task InitWebView()
        {
            var root = Misc.Utils.GetSysAppDataFolder();
            var dir = Path.Combine(root, "webui");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var envs = await CoreWebView2Environment.CreateAsync(null, dir);
            await webView2.EnsureCoreWebView2Async(envs);
        }
        #endregion
    }
}
