using System.Collections.Generic;
using System.Linq;

namespace NeoLuna.Services
{
    internal class FormMgrSvc : VgcApis.BaseClasses.Disposable
    {
        Views.WinForms.FormMain formMain = null;
        Views.WinForms.FormLog formLog = null;

        List<Views.WinForms.FormEditor> editors = new List<Views.WinForms.FormEditor>();
        readonly object formLocker = new object();

        public Settings settings;
        public LuaServer luaServer;
        public AstServer astServer;
        public VgcApis.Interfaces.Services.IApiService vgcApi;

        public FormMgrSvc(
            Settings settings,
            LuaServer luaServer,
            AstServer astServer,
            VgcApis.Interfaces.Services.IApiService vgcApi
        )
        {
            this.astServer = astServer;
            this.luaServer = luaServer;
            this.vgcApi = vgcApi;
            this.settings = settings;
        }

        #region public methods
        public void CreateNewEditor() => CreateNewEditor(null);

        public void CreateNewEditor(Models.Data.LuaCoreSetting initialCoreSettings)
        {
            Views.WinForms.FormEditor form = null;

            VgcApis.Misc.UI.Invoke(() =>
            {
                form = Views.WinForms.FormEditor.CreateForm(this, initialCoreSettings);

                form.FormClosing += (s, a) =>
                {
                    var oldForm = s as Views.WinForms.FormEditor;
                    RemoveFormFromList(oldForm);
                };

                form.Show();
            });

            lock (formLocker)
            {
                if (form != null)
                {
                    editors.Add(form);
                }
            }
        }

        public void ShowFormMain()
        {
            Views.WinForms.FormMain form = null;
            if (formMain == null || formMain.IsDisposed)
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    form = Views.WinForms.FormMain.CreateForm(settings, luaServer, this);
                });
            }

            lock (formLocker)
            {
                if (form != null)
                {
                    formMain = form;
                    formMain.FormClosed += (s, a) => formMain = null;
                    form = null;
                }
            }

            VgcApis.Misc.UI.Invoke(() =>
            {
                form?.Close();
                formMain?.Show();
                formMain?.Activate();
            });
        }

        public void ShowFormLog()
        {
            Views.WinForms.FormLog form = null;
            if (formLog == null || formLog.IsDisposed)
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    var logger = settings.GetLogger();
                    form = new Views.WinForms.FormLog(logger);
                });
            }

            lock (formLocker)
            {
                if (form != null)
                {
                    formLog = form;
                    formLog.FormClosed += (s, a) => formLog = null;
                    form = null;
                }
            }

            VgcApis.Misc.UI.Invoke(() =>
            {
                form?.Close();
                formLog?.Show();
                formLog?.Activate();
            });
        }

        public void ShowOrCreateFirstEditor()
        {
            var form = editors.FirstOrDefault();
            if (form == null)
            {
                CreateNewEditor();
            }
            else
            {
                VgcApis.Misc.UI.Invoke(() => form.Activate());
            }
        }

        #endregion

        #region private methods
        void RemoveFormFromList(Views.WinForms.FormEditor form)
        {
            lock (formLocker)
            {
                editors.RemoveAll(f => f == form);
            }
        }

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            VgcApis.Misc.UI.CloseFormIgnoreError(formMain);

            List<Views.WinForms.FormEditor> formList;
            lock (formLocker)
            {
                formList = editors.ToList();
            }

            foreach (var form in formList)
            {
                VgcApis.Misc.UI.CloseFormIgnoreError(form);
            }
        }
        #endregion
    }
}
