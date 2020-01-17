namespace V2RayGCon.Model.Data
{
    public class Enum
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
