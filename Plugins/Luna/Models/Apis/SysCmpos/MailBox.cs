using System.Collections.Concurrent;

namespace Luna.Models.Apis.SysCmpos
{
    public class MailBox : VgcApis.Interfaces.Lua.ILuaMailBox
    {
        private readonly string myAddress;
        private readonly PostOffice postOffice;

        ConcurrentQueue<VgcApis.Models.Datas.LuaMail> mails = new ConcurrentQueue<VgcApis.Models.Datas.LuaMail>();

        public MailBox(string myAddress, PostOffice postOffice)
        {
            this.myAddress = myAddress;
            this.postOffice = postOffice;
        }

        #region properties

        #endregion

        #region public methods
        public string GetAddress() => myAddress;

        public bool Reply(VgcApis.Interfaces.Lua.ILuaMail source, string header) =>
            Reply(source, header, null);

        public bool Reply(VgcApis.Interfaces.Lua.ILuaMail source, string header, string body) =>
            Reply(source, header, body, null);

        public bool Reply(VgcApis.Interfaces.Lua.ILuaMail source, string header, string body, string footer) =>
            Send(source.GetAddress(), header, body, footer);

        public bool Send(string address) =>
            Send(address, header: null);

        public bool Send(string address, string header) =>
            Send(address, header, null);

        public bool Send(string address, string header, string body) =>
            Send(address, header, body, null);

        public bool Send(string address, string header, string body, string footer)
        {
            var mail = new VgcApis.Models.Datas.LuaMail
            {
                from = myAddress,
                header = header,
                body = body,
                footer = footer,
            };

            return postOffice.Send(address, mail);
        }

        public int Count() => mails.Count;

        public VgcApis.Models.Datas.LuaMail Check()
        {
            if (mails.TryDequeue(out var mail))
            {
                return mail;
            }
            return null;
        }

        public void Add(VgcApis.Models.Datas.LuaMail mail)
        {
            mails.Enqueue(mail);
        }
        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
