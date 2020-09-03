using System;
using System.Collections.Generic;

namespace V2RayGCon.Models.VeeShareLinks
{
    public sealed class Ss1b : BasicSettings
    {
        // ver 1a is optimized for shadowshocks protocol

        const string version = @"1b";
        static public bool IsDecoderFor(string ver) => version == ver;

        static public bool IsEncoderFor(string protocol) => protocol == "shadowsocks";

        public bool isUseOta;
        public string password, method; // 256 bytes

        public Ss1b()
        {
            password = string.Empty;
            method = string.Empty;
            isUseOta = false;
        }

        public Ss1b(BasicSettings source) : this()
        {
            CopyFrom(source);
        }

        public Ss1b(byte[] bytes) :
           this()
        {
            var ver = VgcApis.Libs.Streams.BitStream.ReadVersion(bytes);
            if (ver != version)
            {
                throw new NotSupportedException($"Not supported version ${ver}");
            }

            using (var bs = new VgcApis.Libs.Streams.BitStream(bytes))
            {
                var readString = Utils.GenReadStringHelper(bs, strTable);
                var readMethodString = Utils.GenReadStringHelper(bs, methodTable);

                alias = bs.Read<string>();
                description = readString();
                address = bs.ReadAddress();
                port = bs.Read<int>();
                password = bs.Read<string>();
                method = readMethodString();
                isUseOta = bs.Read<bool>();
                isUseTls = bs.Read<bool>();
                isSecTls = bs.Read<bool>();
                streamType = readString();
                streamParam1 = readString();
                streamParam2 = readString();
                streamParam3 = readString();
            }
        }

        #region string table for compression 
        List<string> methodTable = new List<string>{
            "", "aes-256-cfb", "aes-128-cfb", "chacha20",
            "chacha20-ietf","aes-256-gcm", "aes-128-gcm", "chacha20-poly1305" , "chacha20-ietf-poly1305"
        };

        #endregion

        #region public methods

        public byte[] ToBytes()
        {
            byte[] result;
            using (var bs = new VgcApis.Libs.Streams.BitStream())
            {
                var writeBasicString = Utils.GenWriteStringHelper(bs, strTable);
                var writeMethodString = Utils.GenWriteStringHelper(bs, methodTable);

                bs.Clear();

                bs.Write(alias);
                writeBasicString(description);
                bs.WriteAddress(address);
                bs.Write(port);
                bs.Write(password);
                writeMethodString(method);
                bs.Write(isUseOta);
                bs.Write(isUseTls);
                bs.Write(isSecTls);
                writeBasicString(streamType);
                writeBasicString(streamParam1);
                writeBasicString(streamParam2);
                writeBasicString(streamParam3);

                result = bs.ToBytes(version);
            }
            return result;
        }

        public bool EqTo(Ss1b vee)
        {
            if (!EqTo(vee as BasicSettings)
                || isUseOta != vee.isUseOta
                || password != vee.password
                || method != vee.method)
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
