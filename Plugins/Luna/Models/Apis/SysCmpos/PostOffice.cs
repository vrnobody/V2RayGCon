using System;
using System.Collections.Concurrent;

namespace Luna.Models.Apis.SysCmpos
{
    public class PostOffice
    {
        ConcurrentDictionary<string, MailBox> mailboxs = new ConcurrentDictionary<string, MailBox>();

        public PostOffice()
        { }

        #region properties

        #endregion

        #region public methods
        public VgcApis.Interfaces.Lua.ILuaMailBox ApplyRandomMailBox()
        {
            for (int failsafe = 0; failsafe < 10000; failsafe++)
            {
                var name = Guid.NewGuid().ToString();
                var mailbox = CreateMailBox(name);
                if (mailbox != null)
                {
                    return mailbox;
                }
            }

            // highly unlikely
            return null;
        }

        public bool RemoveMailBox(VgcApis.Interfaces.Lua.ILuaMailBox mailBox)
        {
            if (mailBox == null)
            {
                return false;
            }

            var address = mailBox.GetAddress();
            if (mailboxs.TryRemove(address, out var box))
            {
                box.Close();
                return true;
            }
            return false;
        }

        public VgcApis.Interfaces.Lua.ILuaMailBox CreateMailBox(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return null;
            }

            var mailbox = new MailBox(address, this);
            if (mailboxs.TryAdd(address, mailbox))
            {
                return mailbox;
            }

            return null;
        }

        public bool Send(string address, VgcApis.Models.Datas.LuaMail mail)
        {
            if (string.IsNullOrEmpty(address) || mail == null)
            {
                return false;
            }

            if (!mailboxs.TryGetValue(address, out var mailbox))
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
