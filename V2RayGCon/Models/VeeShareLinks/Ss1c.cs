using System;
using System.Collections.Generic;
using V2RayGCon.Models.Datas;

namespace V2RayGCon.Models.VeeShareLinks
{
    public sealed class Ss1c : BasicSettings
    {
        // ver 1a is optimized for shadowshocks protocol

        public const string version = @"1c";
        public const string proto = "shadowsocks";

        public string password,
            method; // 256 bytes

        public Ss1c()
        {
            password = string.Empty;
            method = string.Empty;
        }

        public Ss1c(BasicSettings source)
            : this()
        {
            CopyFrom(source);
        }

        public Ss1c(byte[] bytes)
            : this()
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

                tlsType = bs.Read<string>();
                isSecTls = bs.Read<bool>();
                tlsServName = bs.Read<string>();

                streamType = readString();
                streamParam1 = readString();
                streamParam2 = readString();
                streamParam3 = readString();
            }
        }

        #region string table for compression
        List<string> methodTable = new List<string>
        {
            "",
            "aes-256-cfb",
            "aes-128-cfb",
            "chacha20",
            "chacha20-ietf",
            "aes-256-gcm",
            "aes-128-gcm",
            "chacha20-poly1305",
            "chacha20-ietf-poly1305"
        };

        #endregion

        #region public methods
        public override void CopyFromVeeConfig(VeeConfigs vc)
        {
            base.CopyFromVeeConfig(vc);
            password = vc.auth1;
            method = vc.auth2;
        }

        public override VeeConfigs ToVeeConfigs()
        {
            var vc = base.ToVeeConfigs();
            vc.proto = proto;
            vc.auth1 = password;
            vc.auth2 = method;
            return vc;
        }

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

                bs.Write(tlsType);
                bs.Write(isSecTls);
                bs.Write(tlsServName);

                writeBasicString(streamType);
                writeBasicString(streamParam1);
                writeBasicString(streamParam2);
                writeBasicString(streamParam3);

                result = bs.ToBytes(version);
            }
            return result;
        }

        public bool EqTo(Ss1c vee)
        {
            if (!EqTo(vee as BasicSettings) || password != vee.password || method != vee.method)
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
