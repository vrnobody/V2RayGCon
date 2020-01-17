namespace ProxySetter.Model.Data
{
    public class Enum
    {
        /// <summary>
        /// Determine if the two ranges overlap
        /// </summary>
        public enum Overlaps
        {
            None,
            All,
            Left,
            Middle,
            Right,
        }

        public enum PacListModes
        {
            WhiteList = 0,
            BlackList = 1,
        }

        public enum PacProtocols
        {
            HTTP = 0,
            SOCKS = 1,
        }

        public enum SystemProxyModes
        {
            None = 0,  // keep current system proxy settings
            PAC = 1,
            Global = 2,
            Direct = 3,
        }

        public enum OnOff
        {
            OFF = 0,
            ON = 1,
        }
    }
}
