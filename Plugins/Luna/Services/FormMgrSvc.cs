using System.Collections.Generic;
using System.Linq;

namespace Luna.Services
{
    internal class FormMgrSvc :
        VgcApis.BaseClasses.Disposable
    {

        Views.WinForms.FormMain formMain = null;
        List<Views.WinForms.FormEditor> editors = new List<Views.WinForms.FormEditor>();
        readonly object formLocker = new object();

        Settings settings;
        LuaServer luaServer;
        VgcApis.Interfaces.Services.IApiService api;

        public FormMgrSvc() { }

        public void Run(
            Settings settings,
            LuaServer luaServer,
            VgcApis.Interfaces.Services.IApiService api)
        {

            this.api = api;
            this.settings = settings;
            this.luaServer = luaServer;
        }

        #region public methods
        public void CreateNewEditor() => CreateNewEditor(null);

        public void CreateNewEditor(Models.Data.LuaCoreSetting initialCoreSettings)
        {
            Views.WinForms.FormEditor form = null;

            VgcApis.Misc.UI.Invoke(() =>
            {
                form = Views.WinForms.FormEditor.CreateForm(
                    api, settings, luaServer, this,
                    initialCoreSettings);

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
            var form = formMain;
            if (form == null)
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    form = Views.WinForms.FormMain.CreateForm(settings, luaServer, this);
                });
            }

            lock (formLocker)
            {
                if (formMain == null && form != null)
                {
                    formMain = form;
                }
            }

            VgcApis.Misc.UI.Invoke(() =>
            {
                if (formMain == form && form != null)
                {
                    form.FormClosed += (s, a) =>
                    {
                        formMain = null;
                    };
                }
                else
                {
                    form?.Close();
                }

                formMain?.Show();
                formMain?.Activate();
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
