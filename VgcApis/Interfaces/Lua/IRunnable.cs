namespace VgcApis.Interfaces.Lua
{
    /// <summary>
    /// 专门用于Sys:CreateHttpServer(...)
    /// </summary>
    public interface IRunnable
    {
        void Start();
        void Stop();
    }
}
