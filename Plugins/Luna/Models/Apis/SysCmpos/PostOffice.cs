using System.Collections.Concurrent;

namespace Luna.Models.Apis.SysCmpos
{
    public class PostOffice
    {
        ConcurrentDictionary<string, MailBox> mailboxes = new ConcurrentDictionary<string, MailBox>();

        public PostOffice()
        {

        }

        #region properties

        #endregion

        #region public methods

        public bool RemoveMailBox(VgcApis.Interfaces.Lua.ILuaMailBox mailbox)
        {
            if (mailboxes.TryRemove(mailbox.GetAddress(), out var box))
            {
                box.Close();
                return true;
            }
            return false;
        }

        public bool ValidateMailBox(VgcApis.Interfaces.Lua.ILuaMailBox mailbox)
        {
            if (mailbox == null)
            {
                return false;
            }

            var addr = mailbox.GetAddress();
            if (string.IsNullOrEmpty(addr)
                || !mailboxes.TryGetValue(addr, out var mb)
                || !ReferenceEquals(mb, mailbox))
            {
                return false;
            }
            return true;
        }

        public VgcApis.Interfaces.Lua.ILuaMailBox CreateMailBox(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return null;
            }

            var mailbox = new MailBox(address, this);
            if (mailboxes.TryAdd(address, mailbox))
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
