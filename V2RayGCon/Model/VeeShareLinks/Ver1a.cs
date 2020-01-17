using System;
using System.Collections.Generic;

namespace V2RayGCon.Model.VeeShareLinks
{
    public sealed class Ver1a
    {
        // ver 0b is optimized for shadowshocks protocol

        const string version = @"1a";
        static public string SupportedVersion() => version;

        public string alias, description; // 256 bytes each
        public bool isUseOta, isUseTls;
        public int port; // 16 bit 

        const int networkTypeLenInBits = 2;
        int networkType; // 2 bit "tcp" , "udp" , "tcp,udp"

        public string address, password, method; // 256 bytes
        public string streamType, streamParam1, streamParam2, streamParam3; // 256 bytes each


        public Ver1a()
        {
            // config
            alias = string.Empty;
            description = string.Empty;

            // ss 
            address = string.Empty;
            port = 0;
            password = string.Empty;
            method = string.Empty;
            networkType = 0;
            isUseOta = false;

            // stream
            isUseTls = false;
            streamType = string.Empty;
            streamParam1 = string.Empty;
            streamParam2 = string.Empty;
            streamParam3 = string.Empty;
        }

        public Ver1a(byte[] bytes) :
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

                address = bs.ReadAddress();
                port = bs.Read<int>();
                password = bs.Read<string>();
                method = readString();
                networkType = bs.ReadTinyInt(networkTypeLenInBits);
                isUseOta = bs.Read<bool>();

                isUseTls = bs.Read<bool>();
                streamType = readString();
                streamParam1 = readString();
                streamParam2 = readString();
                streamParam3 = readString();
            }
        }

        #region string table for compression 
        List<string> strTable = new List<string>{
            "","ws", "tcp", "kcp", "h2",
            "quic", "none", "srtp", "utp", "wechat-video",
            "dtls", "wireguard", "aes-256-cfb", "aes-128-cfb", "chacha20",
            "chacha20-ietf","aes-256-gcm", "aes-128-gcm", "chacha20-poly1305" , "chacha20-ietf-poly1305"
        };

        string[] networkTable = new string[] {
            "tcp" , "udp" , "tcp,udp"
        };

        string GetNetworkTypeNameByIndex(int index) =>
            networkTable[
                VgcApis.Libs.Utils.Clamp(
                    index, 0, networkTable.Length)];

        int GetIndexByNetworkTypeName(string networkTypeName)
        {
            var nw = networkTypeName.ToLower();
            for (int i = 0; i < networkTable.Length; i++)
            {
                if (networkTable[i] == nw)
                {
                    return i;
                }
            }
            return 0;
        }

        #endregion

        #region public methods
        public void SetNetworkType(string networkTypeName) =>
            networkType = GetIndexByNetworkTypeName(networkTypeName);

        public int GetCurNetworkTypeIndex() =>
            networkType;

        public string GetCurNetworkTypeName() =>
            GetNetworkTypeNameByIndex(networkType);

        public byte[] ToBytes()
        {
            byte[] result;
            using (var bs = new VgcApis.Libs.Streams.BitStream())
            {
                var writeString = Utils.GenWriteStringHelper(bs, strTable);

                bs.Clear();

                bs.Write(alias);
                writeString(description);

                bs.WriteAddress(address);
                bs.Write(port);
                bs.Write(password);
                writeString(method);
                bs.WriteTinyInt(networkType, networkTypeLenInBits);
                bs.Write(isUseOta);

                bs.Write(isUseTls);
                writeString(streamType);
                writeString(streamParam1);
                writeString(streamParam2);
                writeString(streamParam3);

                result = bs.ToBytes(version);
            }
            return result;
        }

        public bool EqTo(Ver1a vee)
        {
            if (isUseOta != vee.isUseOta
                || port != vee.port
                || password != vee.password
                || method != vee.method
                || networkType != vee.networkType
                || address != vee.address
                || streamType != vee.streamType
                || streamParam1 != vee.streamParam1
                || streamParam2 != vee.streamParam2
                || streamParam3 != vee.streamParam3
                || alias != vee.alias
                || description != vee.description)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region private methods



        #endregion

    }
}
