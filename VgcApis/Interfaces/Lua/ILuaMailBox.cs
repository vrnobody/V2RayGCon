namespace VgcApis.Interfaces.Lua
{
    public interface ILuaMailBox
    {
        int Count();

        void Close();

        Models.Datas.LuaMail Check();

        Models.Datas.LuaMail Wait();

        string GetAddress();

        bool Reply(ILuaMail mail, string title);

        bool Reply(ILuaMail mail, string title, string content);

        bool Reply(ILuaMail mail, double code);

        bool Reply(ILuaMail mail, double code, string content);

        bool Reply(ILuaMail mail, bool state);

        bool Reply(ILuaMail mail, bool state, string content);

        bool Reply(ILuaMail mail, double code, string title, bool state, string content);


        bool Send(string address, string title);
        bool Send(string address, string title, string content);
        bool Send(string address, double code);
        bool Send(string address, double code, string content);

        bool Send(string address, bool state);
        bool Send(string address, bool state, string content);

        bool Send(string address, double code, string title, bool state, string content);
    }
}
