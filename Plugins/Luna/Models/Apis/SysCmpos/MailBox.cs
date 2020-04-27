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
        public bool Clear()
        {
            var r = false;
            try
            {
                while (mails.TryTake(out var _))
                {
                    r = true;
                }
            }
            catch (System.ObjectDisposedException) { }
            catch (System.InvalidOperationException) { }
            return r;
        }

        public string GetAddress() => myAddress;

        public int Count() => mails.Count;

        public VgcApis.Models.Datas.LuaMail Wait()
        {
            do
            {
                if (TryTakeIgnoreError(mails, 3000, out var mail))
                {
                    return mail;
                }
                VgcApis.Misc.Utils.Sleep(100);
            } while (!mails.IsCompleted);
            return null;
        }

        public VgcApis.Models.Datas.LuaMail Wait(int milSecs)
        {
            if (TryTakeIgnoreError(mails, milSecs, out var mail))
            {
                return mail;
            }
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

        public bool ReplyCode(VgcApis.Interfaces.Lua.ILuaMail mail, double code) =>
            ReplyCode(mail, code, null);

        public bool ReplyCode(VgcApis.Interfaces.Lua.ILuaMail mail, double code, string content) =>
            Reply(mail, code, null, false, content);

        public bool ReplyState(VgcApis.Interfaces.Lua.ILuaMail mail, bool state) =>
            ReplyState(mail, state, null);

        public bool ReplyState(VgcApis.Interfaces.Lua.ILuaMail mail, bool state, string content) =>
            Reply(mail, 0, null, state, content);

        public bool Reply(VgcApis.Interfaces.Lua.ILuaMail mail, double code, string title, bool state, string content) =>
            Send(mail.GetAddress(), code, title, state, content);

        public bool Send(string address, string title) =>
            Send(address, title, null);

        public bool Send(string address, string title, string content) =>
            Send(address, 0, title, false, content);

        public bool SendCode(string address, double code) =>
            SendCode(address, code, null);
        public bool SendCode(string address, double code, string content) =>
            Send(address, code, null, false, content);

        public bool SendState(string address, bool state) =>
            SendState(address, state, null);

        public bool SendState(string address, bool state, string content) =>
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

        public bool IsCompleted()
        {
            try
            {
                return mails.IsCompleted;
            }
            catch { }
            return true;
        }

        public bool IsAddingCompleted()
        {
            try
            {
                return mails.IsAddingCompleted;
            }
            catch { }
            return true;
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
        bool TryTakeIgnoreError<T>(BlockingCollection<T> collection, int timeout, out T item)
        {
            try
            {
                return collection.TryTake(out item, timeout);
            }
            catch (System.ObjectDisposedException) { }
            catch (System.InvalidOperationException) { }
            catch (System.ArgumentOutOfRangeException) { }

            item = default;
            return false;
        }
        #endregion

        #region protected methods

        #endregion
    }
}
