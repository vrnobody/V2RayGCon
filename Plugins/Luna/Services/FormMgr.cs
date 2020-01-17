using System.Collections.Generic;
using System.Linq;

namespace Luna.Services
{
    public class FormMgr :
        VgcApis.Models.BaseClasses.Disposable
    {
        List<Views.WinForms.FormMain> forms = new List<Views.WinForms.FormMain>();
        readonly object formLocker = new object();

        Settings settings;
        LuaServer luaServer;
        VgcApis.Models.IServices.IApiService api;

        public FormMgr() { }

        public void Run(
            Settings settings,
            LuaServer luaServer,
            VgcApis.Models.IServices.IApiService api)
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
                var newForm = new Views.WinForms.FormMain(
                    api, settings, luaServer, this);

                newForm.FormClosed += (s, a) =>
                {
                    var form = newForm; // capture
                    RemoveFormFromList(form);
                };

                forms.Add(newForm);
                newForm.Show();
            }
        }

        public void ShowOrCreateFirstForm()
        {
            if (forms.Count() > 0)
            {
                forms[0].Activate();
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
            lock (formLocker)
            {
                var formList = forms.ToList();
                foreach (var form in formList)
                {
                    form.Close();
                }
            }
        }
        #endregion

    }
}
