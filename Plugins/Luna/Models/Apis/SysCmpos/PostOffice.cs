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
        public bool DestoryMailBox(VgcApis.Interfaces.Lua.ILuaMailBox mailBox)
        {
            var address = mailBox.GetAddress();
            return mailboxs.TryRemove(address, out _);
        }

        public VgcApis.Interfaces.Lua.ILuaMailBox CreateMailBox(string address)
        {
            var mailbox = new MailBox(address, this);
            if (mailboxs.TryAdd(address, mailbox))
            {
                return mailbox;
            }

            return null;
        }

        public bool Send(string address, VgcApis.Models.Datas.LuaMail mail)
        {
            if (!mailboxs.TryGetValue(address, out var mailbox))
            {
                return false;
            }

            mailbox.Add(mail);
            return true;
        }
        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
