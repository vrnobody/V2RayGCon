using System.Collections.Generic;
using System.Linq;

namespace Luna.Services
{
    public class FormMgr :
        VgcApis.BaseClasses.Disposable
    {
        List<Views.WinForms.FormMain> forms = new List<Views.WinForms.FormMain>();
        readonly object formLocker = new object();

        Settings settings;
        LuaServer luaServer;
        VgcApis.Interfaces.Services.IApiService api;

        public FormMgr() { }

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
        public void CreateNewForm()
        {
            lock (formLocker)
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    var newForm = Views.WinForms.FormMain.CreateForm(api, settings, luaServer, this);

                    newForm.FormClosed += (s, a) =>
                    {
                        var form = newForm; // capture
                        RemoveFormFromList(form);
                    };

                    forms.Add(newForm);
                    newForm.Show();
                });
            }
        }

        public void ShowOrCreateFirstForm()
        {
            if (forms.Count() > 0)
            {
                VgcApis.Misc.UI.Invoke(() => forms[0].Activate());
                return;
            }
            CreateNewForm();
        }

        #endregion

        #region private methods
        void RemoveFormFromList(Views.WinForms.FormMain form)
        {
            lock (formLocker)
            {
                forms.RemoveAll(f => f == form);
            }
        }

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            List<Views.WinForms.FormMain> formList;
            lock (formLocker)
            {
                formList = forms.ToList();
            }

            foreach (var form in formList)
            {
                VgcApis.Misc.UI.CloseFormIgnoreError(form);
            }
        }
        #endregion

    }
}
