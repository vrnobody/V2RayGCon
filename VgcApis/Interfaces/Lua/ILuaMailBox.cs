namespace VgcApis.Interfaces.Lua
{
    public interface ILuaMailBox
    {
        int Count();
        bool Clear();

        bool IsCompleted();

        bool IsAddingCompleted();

        void Close();

        Models.Datas.LuaMail Check();

        Models.Datas.LuaMail Wait();

        Models.Datas.LuaMail Wait(int milSecs);

        string GetAddress();

        bool Reply(ILuaMail mail, string title);

        bool Reply(ILuaMail mail, string title, string content);


        // lua识别参数类型的方式过于灵活
        // 如果用重载写法会导致错误的调用 Reply(ILuaMail mail, STRING title)
        bool ReplyCode(ILuaMail mail, double code);

        bool ReplyCode(ILuaMail mail, double code, string content);

        bool ReplyState(ILuaMail mail, bool state);

        bool ReplyState(ILuaMail mail, bool state, string content);

        bool Reply(ILuaMail mail, double code, string title, bool state, string content);


        bool Send(string address, string title);
        bool Send(string address, string title, string content);
        bool SendCode(string address, double code);
        bool SendCode(string address, double code, string content);

        bool SendState(string address, bool state);
        bool SendState(string address, bool state, string content);

        bool Send(string address, double code, string title, bool state, string content);
    }
}
