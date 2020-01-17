using System.Net;
using System.Net.Sockets;
using System.Text;

namespace VgcApis.Libs.Streams.RawBitStream
{
    public sealed class Address :
        Models.BaseClasses.ComponentOf<RawBitStream>
    {
        const int BytesPerIpv4 = Models.Consts.BitStream.BytesPerIpv4;
        const int BytesPerIpv6 = Models.Consts.BitStream.BytesPerIpv6;

        Bytes bytesWriter;

        public Address() { }

        public void Run(Bytes bytesWriter)
        {
            this.bytesWriter = bytesWriter;
        }

        #region properties

        #endregion

        #region public methods
        public void Write(string address)
        {
            var bitStream = GetContainer();
            var ip = ParseIpAddress(address);
            var str = Utils.CutBytes(Encoding.UTF8.GetBytes(address));
            var ipBytes = ip?.GetAddressBytes();
            if (ipBytes != null && ipBytes.Length < str.Length)
            {
                bitStream.Write(true);
                var isIpv6 = ip.AddressFamily == AddressFamily.InterNetworkV6;
                bitStream.Write(isIpv6);
                bytesWriter.Write(ipBytes, ipBytes.Length);
            }
            else
            {
                bitStream.Write(false);
                bytesWriter.Write(str);
            }
        }

        public string Read()
        {
            var isIp = ReadOneBit();
            if (isIp)
            {
                var len = ReadOneBit() ? BytesPerIpv6 : BytesPerIpv4;
                var cache = bytesWriter.Read(len);
                return new IPAddress(cache).ToString();
            }
            else
            {
                var cache = bytesWriter.Read();
                return Encoding.UTF8.GetString(cache);
            }
        }

        #endregion

        #region private methods
        bool ReadOneBit()
        {
            var bitStream = GetContainer();
            return bitStream.Read() == true;
        }

        IPAddress ParseIpAddress(string input)
        {
            // https://stackoverflow.com/questions/799060/how-to-determine-if-a-string-is-a-valid-ipv4-or-ipv6-address-in-c
            IPAddress address;
            if (IPAddress.TryParse(input, out address))
            {
                switch (address.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                    case AddressFamily.InterNetworkV6:
                        return address;
                    default:
                        // umm... yeah... I'm going to need to take your red packet and...
                        break;
                }
            }
            return null;
        }
        #endregion

        #region protected methods

        #endregion
    }
}
