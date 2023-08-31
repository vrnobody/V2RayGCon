namespace Luna.Interfaces
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
