namespace VgcApis.Interfaces.Lua
{
    public interface ILuaMail
    {
        string GetAddress();
        string GetTitle();
        string GetContent();
        double GetCode();
    }
}
