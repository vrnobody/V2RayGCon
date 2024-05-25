using System.Collections.Concurrent;

namespace V2RayGCon.Services
{
    public class PostOffice
        : BaseClasses.SingletonService<PostOffice>,
            VgcApis.Interfaces.Services.IPostOffice
    {
        static readonly ConcurrentDictionary<
            string,
            VgcApis.Interfaces.PostOfficeComponents.ILuaMailBox
        > mailboxes =
            new ConcurrentDictionary<string, VgcApis.Interfaces.PostOfficeComponents.ILuaMailBox>();

        static readonly PostOfficeComponents.SnapCache snapCache =
            new PostOfficeComponents.SnapCache();

        PostOffice() { }

        #region properties

        #endregion

        #region ISnapCache
        public bool SnapCacheRemove(string token) => snapCache.RemoveCache(token);

        public string SnapCacheApply() => snapCache.ApplyNewCache();

        public bool SnapCacheCreate(string token)
        {
            return snapCache.CreateCache(token);
        }

        public object SnapCacheGet(string token, string key) => snapCache.Get(token, key);

        public bool SnapCacheSet(string token, string key, object value) =>
            snapCache.Set(token, key, value);
        #endregion

        #region public methods

        public bool RemoveMailBox(VgcApis.Interfaces.PostOfficeComponents.ILuaMailBox mailbox)
        {
            if (mailboxes.TryRemove(mailbox.GetAddress(), out var box))
            {
                box.Close();
                return true;
            }
            return false;
        }

        public bool ValidateMailBox(VgcApis.Interfaces.PostOfficeComponents.ILuaMailBox mailbox)
        {
            if (mailbox == null)
            {
                return false;
            }

            var addr = mailbox.GetAddress();
            if (
                string.IsNullOrEmpty(addr)
                || !mailboxes.TryGetValue(addr, out var mb)
                || !ReferenceEquals(mb, mailbox)
            )
            {
                return false;
            }
            return true;
        }

        public VgcApis.Interfaces.PostOfficeComponents.ILuaMailBox CreateMailBox(
            string address,
            int capacity
        )
        {
            if (string.IsNullOrEmpty(address) || capacity < 1)
            {
                return null;
            }

            var mailbox = new VgcApis.Models.Datas.LuaMailBox(address, this, capacity);
            if (mailboxes.TryAdd(address, mailbox))
            {
                return mailbox;
            }

            return null;
        }

        public bool SendAndWait(string address, VgcApis.Models.Datas.LuaMail mail)
        {
            if (string.IsNullOrEmpty(address) || mail == null)
            {
                return false;
            }

            if (!mailboxes.TryGetValue(address, out var mailbox))
            {
                return false;
            }

            return mailbox.Add(mail);
        }

        public bool Send(string address, VgcApis.Models.Datas.LuaMail mail)
        {
            if (string.IsNullOrEmpty(address) || mail == null)
            {
                return false;
            }

            if (!mailboxes.TryGetValue(address, out var mailbox))
            {
                return false;
            }
            return mailbox.TryAdd(mail);
        }
        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
