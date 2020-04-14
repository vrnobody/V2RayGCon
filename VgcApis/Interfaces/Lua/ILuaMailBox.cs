namespace VgcApis.Interfaces.Lua
{
    public interface ILuaMailBox
    {
        int Count();

        Models.Datas.LuaMail Check();

        string GetAddress();

        bool Reply(ILuaMail source, string header);

        bool Reply(ILuaMail source, string header, string body);

        bool Reply(ILuaMail source, string header, string body, string footer);

        bool Send(string address);

        bool Send(string address, string header);

        bool Send(string address, string header, string body);

        bool Send(string address, string header, string body, string footer);
    }
}
