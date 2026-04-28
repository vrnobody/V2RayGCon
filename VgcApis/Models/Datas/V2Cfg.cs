using Newtonsoft.Json;

namespace VgcApis.Models.Datas
{
    public class V2Cfg
    {
        static readonly byte[] zdict = Properties.Resources.zstd_dict_v2cfg_ver3;

        public int version = 3;
        public string name = string.Empty;
        public string hash = string.Empty;
        public string config = string.Empty;

        // 序列化需要这个ctor, 不要删除！
        public V2Cfg() { }

        public V2Cfg(string name, string config) => Create(name, config);

        #region public
        public static V2Cfg FromVer3Body(string body)
        {
            try
            {
                var json = VgcApis.Libs.Infr.ZipExtensions.ZstdDictFromBase64Core(zdict, body);
                var v = JsonConvert.DeserializeObject<V2Cfg>(json);
                if (v.IsValid(3))
                {
                    return v;
                }
            }
            catch { }
            return null;
        }

        public static V2Cfg FromVer2Body(string body)
        {
            try
            {
                var v =
                    VgcApis.Libs.Infr.ZipExtensions.DeserializeObjectFromCompressedUnicodeBase64<V2Cfg>(
                        body
                    );
                if (v.IsValid(2))
                {
                    return v;
                }
            }
            catch { }
            return null;
        }

        public string ToCompressedString()
        {
            try
            {
                UpdateHash();
                var json = JsonConvert.SerializeObject(this);
                return Libs.Infr.ZipExtensions.ZstdDictToBase64Core(zdict, json);
            }
            catch { }
            return null;
        }

        #endregion

        #region private
        bool IsValid(int expVer)
        {
            if (expVer != this.version)
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
