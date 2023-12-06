namespace V2RayGCon.Services
{
    public class Cache : BaseClasses.SingletonService<Cache>
    {
        public Caches.Template tpl;
        public Caches.CoreCache core;

        Cache()
        {
            tpl = new Caches.Template();
            core = new Caches.CoreCache();
        }

        public void Run(Settings setting)
        {
            core.Run(setting);
        }

        #region public method

        #endregion

        #region private method

        #endregion
    }
}
