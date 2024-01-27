namespace V2RayGCon.Services
{
    public class Cache : BaseClasses.SingletonService<Cache>
    {
        public Caches.Template tpl;

        Cache()
        {
            tpl = new Caches.Template();
        }

        #region public method

        #endregion

        #region private method

        #endregion
    }
}
