using System;
using System.Collections.Generic;

namespace V2RayGCon.Model.VeeShareLinks
{
    public sealed class Ver0a
    {
        // ver 0a is optimized for vmess protocol 

        const string version = @"0a";
        static public string SupportedVersion() => version;

        public string alias, description; // 256 bytes each
        public bool isUseTls;
        public int port, alterId; // 16 bit each
        public Guid uuid;
        public string address; // 256 bytes
        public string streamType, streamParam1, streamParam2, streamParam3; // 256 bytes each

        public Ver0a()
        {
            alias = string.Empty;
            description = string.Empty;
            isUseTls = false;
            port = 0;
            uuid = new Guid(); // zeros
            address = string.Empty;
            streamType = string.Empty;
            streamParam1 = string.Empty;
            streamParam2 = string.Empty;
            streamParam3 = string.Empty;
        }

        #region string table for compression 
        const int tableLenInBits = 4;
        List<string> strTable = new List<string>{
            "ws", "tcp", "kcp", "h2", "quic",
            "none", "srtp", "utp", "wechat-video",
            "dtls", "wireguard", "",
        };

        #endregion

        #region public methods

        public Ver0a(byte[] bytes) :
            this()
        {
            var ver = VgcApis.Libs.Streams.BitStream.ReadVersion(bytes);
            if (ver != version)
            {
                throw new NotSupportedException(
                    $"Not supported version ${ver}");
            }

            using (var bs = new VgcApis.Libs.Streams.BitStream(bytes))
            {
                var readString = Utils.GenReadStringHelper(bs, strTable);

                alias = bs.Read<string>();
                description = readString();
                isUseTls = bs.Read<bool>();
                port = bs.Read<int>();
                alterId = bs.Read<int>();
                uuid = bs.Read<Guid>();
                address = bs.ReadAddress();
                streamType = readString();
                streamParam1 = readString();
                streamParam2 = readString();
                streamParam3 = readString();
            }
        }

        public byte[] ToBytes()
        {
            byte[] result;
            using (var bs = new VgcApis.Libs.Streams.BitStream())
            {
                bs.Clear();

                var writeString = Utils.GenWriteStringHelper(bs, strTable);

                bs.Write(alias);
                writeString(description);
                bs.Write(isUseTls);
                bs.Write(port);
                bs.Write(alterId);
                bs.Write(uuid);
                bs.WriteAddress(address);
                writeString(streamType);
                writeString(streamParam1);
                writeString(streamParam2);
                writeString(streamParam3);

                result = bs.ToBytes(version);
            }
            
            return result;
        }

        public bool EqTo(Ver0a veeLink)
        {
            if (isUseTls != veeLink.isUseTls
                || port != veeLink.port
                || uuid != veeLink.uuid
                || address != veeLink.address
                || streamType != veeLink.streamType
                || streamParam1 != veeLink.streamParam1
                || streamParam2 != veeLink.streamParam2
                || streamParam3 != veeLink.streamParam3
                || alias != veeLink.alias
                || description != veeLink.description)
            {
                return false;
            }
            return true;
        }
        #endregion

    }
}
