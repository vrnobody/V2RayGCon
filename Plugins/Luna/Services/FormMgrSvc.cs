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
        public void CreateNewEditor() => CreateNewEditor("");

        public void CreateNewEditor(string scriptName)
        {
            lock (formLocker)
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    var newForm = Views.WinForms.FormEditor.CreateForm(
                        api, settings, luaServer, this, scriptName);
                    newForm.FormClosed += (s, a) =>
                    {
                        var form = newForm; // capture
                        RemoveFormFromList(form);
                    };

                    editors.Add(newForm);
                    newForm.Show();
                });
            }
        }

        public void ShowFormMain()
        {
            lock (formLocker)
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    if (formMain == null)
                    {
                        var form = Views.WinForms.FormMain.CreateForm(settings, luaServer, this);
                        form.FormClosed += (s, a) =>
                        {
                            formMain = null;
                        };
                        formMain = form;
                        formMain.Show();
                    }
                    formMain.Activate();
                });
            }
        }
        public void ShowOrCreateFirstEditor()
        {
            if (editors.Count() > 0)
            {
                VgcApis.Misc.UI.Invoke(() => editors[0].Activate());
                return;
            }
            CreateNewEditor();
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
            if (settings.IsClosing())
            {
                return;
            }

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
