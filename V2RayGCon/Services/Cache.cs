﻿namespace V2RayGCon.Services
{
    public class Cache : BaseClasses.SingletonService<Cache>
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
