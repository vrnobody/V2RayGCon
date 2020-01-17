namespace V2RayGCon.Service
{
    public class Cache : Model.BaseClass.SingletonService<Cache>
    {
        // special
        public Caches.HTML html;
        public Caches.Template tpl;
        public Caches.CoreCache core;

        Cache()
        {
            html = new Caches.HTML();
            tpl = new Caches.Template();
            core = new Caches.CoreCache();
        }

        public void Run(Setting setting)
        {
            core.Run(setting);
        }

        Caches.JsonAcm _jsonAcm = null;
        public Caches.JsonAcm GetJsonAcm()
        {
            if (_jsonAcm == null)
            {
                _jsonAcm = new Caches.JsonAcm();
            }
            return _jsonAcm;
        }

        #region public method

        #endregion

        #region private method

        #endregion
    }
}
