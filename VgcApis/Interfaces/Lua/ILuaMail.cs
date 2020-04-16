namespace VgcApis.Interfaces.Lua
{
    public interface ILuaMail
    {
        bool GetState();

        string GetAddress();
        string GetTitle();
        string GetContent();
        double GetCode();
    }
}
