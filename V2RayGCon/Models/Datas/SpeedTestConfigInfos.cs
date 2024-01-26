namespace V2RayGCon.Models.Datas
{
    internal class SpeedTestConfigInfos
    {
        public readonly bool isSocks5;
        public readonly string config;

        public SpeedTestConfigInfos(bool isSocks5, string config)
        {
            this.isSocks5 = isSocks5;
            this.config = config;
        }
    }
}
