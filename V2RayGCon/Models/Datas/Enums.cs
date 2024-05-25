namespace V2RayGCon.Models.Datas
{
    public class Enums
    {
        public enum Cultures
        {
            auto = 0,
            enUS = 1,
            zhCN = 2,
        }

        /// <summary>
        /// Inbound types
        /// </summary>
        public enum ProxyTypes
        {
            Config = 0,
            HTTP = 1,
            SOCKS = 2,
            Custom = 3,
        }

        public enum FormLocations
        {
            TopLeft,
            BottomLeft,
            TopRight,
            BottomRight,
        }
    }
}
