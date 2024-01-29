namespace VgcApis.Models.Datas
{
    public class V2Cfg
    {
        public int version = 2;
        public string name = string.Empty;
        public string hash = string.Empty;
        public string config = string.Empty;

        // 序列化需要这个ctor, 不要删除！
        public V2Cfg() { }

        public V2Cfg(string compressedString)
        {
            try
            {
                var v2cfg =
                    Libs.Infr.ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<V2Cfg>(
                        compressedString
                    );
                if (v2cfg.IsValid())
                {
                    this.name = v2cfg.name;
                    this.config = v2cfg.config;
                    Create(name, config);
                }
            }
            catch { }
        }

        public V2Cfg(string name, string config) => Create(name, config);

        #region public
        public string ToCompressedString()
        {
            try
            {
                UpdateHash();
                return Libs.Infr.ZipExtensions.SerializeObjectToCompressedUnicodeBase64(this);
            }
            catch { }
            return null;
        }

        public bool IsValid()
        {
            if (version != 2)
            {
                return false;
            }

            try
            {
                var hash = Misc.Utils.Sha256Hex(name + config);
                return this.hash == hash;
            }
            catch { }
            return false;
        }

        #endregion
        #region private
        void Create(string name, string config)
        {
            this.name = name;
            this.config = config;
            UpdateHash();
        }

        void UpdateHash()
        {
            this.hash = Misc.Utils.Sha256Hex(name + config);
        }
        #endregion
    }
}
