using System.Collections.Concurrent;

namespace Luna.Models.Apis.SysCmpos
{
    public class MailBox : VgcApis.Interfaces.Lua.ILuaMailBox
    {
        private readonly string myAddress;
        private readonly PostOffice postOffice;

        BlockingCollection<VgcApis.Models.Datas.LuaMail> mails =
            new BlockingCollection<VgcApis.Models.Datas.LuaMail>();

        public MailBox(string myAddress, PostOffice postOffice)
        {
            this.myAddress = myAddress;
            this.postOffice = postOffice;
        }

        #region properties

        #endregion

        #region public methods
        public string GetAddress() => myAddress;

        public int Count() => mails.Count;

        public VgcApis.Models.Datas.LuaMail Wait()
        {
            try
            {
                return mails.Take();
            }
            catch (System.ObjectDisposedException) { }
            catch (System.InvalidOperationException) { }
            return null;
        }

        public VgcApis.Models.Datas.LuaMail Check()
        {
            try
            {
                if (mails.TryTake(out var mail))
                {
                    return mail;
                }
            }
            catch (System.ObjectDisposedException) { }
            catch (System.InvalidOperationException) { }
            return null;
        }

        public bool Reply(VgcApis.Interfaces.Lua.ILuaMail mail, string title) =>
            Reply(mail, title, null);

        public bool Reply(VgcApis.Interfaces.Lua.ILuaMail mail, string title, string content) =>
            Reply(mail, 0, title, false, content);

        public bool Reply(VgcApis.Interfaces.Lua.ILuaMail mail, double code) =>
            Reply(mail, code, null);

        public bool Reply(VgcApis.Interfaces.Lua.ILuaMail mail, double code, string content) =>
            Reply(mail, code, null, false, content);

        public bool Reply(VgcApis.Interfaces.Lua.ILuaMail mail, bool state) =>
            Reply(mail, state, null);

        public bool Reply(VgcApis.Interfaces.Lua.ILuaMail mail, bool state, string content) =>
            Reply(mail, 0, null, state, content);

        public bool Reply(VgcApis.Interfaces.Lua.ILuaMail mail, double code, string title, bool state, string content) =>
            Send(mail.GetAddress(), code, title, state, content);

        public bool Send(string address, string title) =>
            Send(address, title, null);

        public bool Send(string address, string title, string content) =>
            Send(address, 0, title, false, content);

        public bool Send(string address, double code) =>
            Send(address, code, null);
        public bool Send(string address, double code, string content) =>
            Send(address, code, null, false, content);

        public bool Send(string address, bool state) =>
            Send(address, state, null);

        public bool Send(string address, bool state, string content) =>
            Send(address, 0, null, state, content);

        public bool Send(string address, double code, string title, bool state, string content)
        {
            var mail = new VgcApis.Models.Datas.LuaMail
            {
                from = myAddress,
                title = title,
                content = content,
                code = code,
                state = state,
            };

            return postOffice.Send(address, mail);
        }

        public void Close()
        {
            mails.CompleteAdding();
        }

        public bool TryAdd(VgcApis.Models.Datas.LuaMail mail)
        {
            try
            {
                return mails.TryAdd(mail);
            }
            catch (System.ObjectDisposedException) { }
            catch (System.InvalidOperationException) { }
            return false;
        }
        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
