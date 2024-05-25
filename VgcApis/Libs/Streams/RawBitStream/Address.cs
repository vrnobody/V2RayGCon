using System.Net;
using System.Net.Sockets;
using System.Text;

namespace VgcApis.Libs.Streams.RawBitStream
{
    public sealed class Address : BaseClasses.ComponentOf<RawBitStream>
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
            var bitStream = GetParent();
            Misc.Utils.TryParseIp(address, out var ip);
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
            var bitStream = GetParent();
            return bitStream.Read() == true;
        }

        #endregion

        #region protected methods

        #endregion
    }
}
