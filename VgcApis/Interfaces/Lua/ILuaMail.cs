namespace VgcApis.Interfaces.Lua
{
    public interface ILuaMail
    {
        string GetAddress();
        string GetHeader();
        string GetBody();
        string GetFooter();
    }
}
