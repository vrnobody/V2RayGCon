using System.Collections.Concurrent;

namespace VgcApis.Models.Datas
{
    public class LuaMailBox : Interfaces.PostOfficeComponents.ILuaMailBox
    {
        private readonly string myAddress;
        private readonly Interfaces.Services.IPostOffice postOffice;

        readonly BlockingCollection<LuaMail> mails;

        public LuaMailBox(
            string myAddress,
            Interfaces.Services.IPostOffice postOffice,
            int capacity
        )
        {
            this.myAddress = myAddress;
            this.postOffice = postOffice;
            mails = new BlockingCollection<LuaMail>(capacity);
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

        public LuaMail Wait()
        {
            do
            {
                if (TryTakeIgnoreError(mails, 3000, out var mail))
                {
                    return mail;
                }
                Misc.Utils.Sleep(100);
            } while (!mails.IsCompleted);
            return null;
        }

        public LuaMail Wait(int ms)
        {
            if (TryTakeIgnoreError(mails, ms, out var mail))
            {
                return mail;
            }
            return null;
        }

        public LuaMail Check()
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

        public bool Reply(Interfaces.PostOfficeComponents.ILuaMail mail, string title) =>
            Reply(mail, title, null);

        public bool Reply(
            Interfaces.PostOfficeComponents.ILuaMail mail,
            string title,
            string content
        ) => Reply(mail, 0, title, false, content);

        public bool ReplyCode(Interfaces.PostOfficeComponents.ILuaMail mail, double code) =>
            ReplyCode(mail, code, null);

        public bool ReplyCode(
            Interfaces.PostOfficeComponents.ILuaMail mail,
            double code,
            string content
        ) => Reply(mail, code, null, false, content);

        public bool ReplyState(Interfaces.PostOfficeComponents.ILuaMail mail, bool state) =>
            ReplyState(mail, state, null);

        public bool ReplyState(
            Interfaces.PostOfficeComponents.ILuaMail mail,
            bool state,
            string content
        ) => Reply(mail, 0, null, state, content);

        public bool Reply(
            Interfaces.PostOfficeComponents.ILuaMail mail,
            double code,
            string title,
            bool state,
            string content
        ) => Send(mail.GetAddress(), code, title, state, content);

        public bool Send(string address, string title) => Send(address, title, null);

        public bool Send(string address, string title, string content) =>
            Send(address, 0, title, false, content);

        public bool SendCode(string address, double code) => SendCode(address, code, null);

        public bool SendCode(string address, double code, string content) =>
            Send(address, code, null, false, content);

        public bool SendState(string address, bool state) => SendState(address, state, null);

        public bool SendState(string address, bool state, string content) =>
            Send(address, 0, null, state, content);

        // backward compactable
        public bool Send(string address, double code, string title, bool state, string content) =>
            Send(address, code, title, state, content, null, null);

        public bool Send(
            string address,
            double code,
            string title,
            bool state,
            string content,
            string header,
            object attachment
        )
        {
            var mail = new LuaMail
            {
                from = myAddress,
                title = title,
                content = content,
                code = code,
                state = state,
                header = header,
                attachment = attachment,
            };

            return postOffice.Send(address, mail);
        }

        public bool SendAndWait(
            string address,
            double code,
            string title,
            bool state,
            string content
        ) => SendAndWait(address, code, title, state, content);

        public bool SendAndWait(
            string address,
            double code,
            string title,
            bool state,
            string content,
            string header,
            object attachment
        )
        {
            var mail = new LuaMail
            {
                from = myAddress,
                title = title,
                content = content,
                code = code,
                state = state,
                header = header,
                attachment = attachment,
            };

            return postOffice.SendAndWait(address, mail);
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

        public bool Add(LuaMail mail)
        {
            try
            {
                mails.Add(mail);
                return true;
            }
            catch (System.ObjectDisposedException) { }
            catch (System.InvalidOperationException) { }
            return false;
        }

        public bool TryAdd(LuaMail mail)
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
