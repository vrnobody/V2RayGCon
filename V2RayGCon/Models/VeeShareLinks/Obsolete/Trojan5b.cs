using System;
using V2RayGCon.Models.Datas;

namespace V2RayGCon.Models.VeeShareLinks.Obsolete
{
    public sealed class Trojan5b : BasicSettings
    {
        // ver 5a is optimized for trojan protocol

        public const string version = @"5b";
        public const string proto = @"trojan";

        public string password; // 256 bytes

        public Trojan5b()
        {
            password = string.Empty;

        }

        public Trojan5b(BasicSettings source) : this()
        {
            CopyFrom(source);
        }

        public Trojan5b(byte[] bytes) :
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

                alias = bs.Read<string>();
                description = readString();
                address = bs.ReadAddress();
                port = bs.Read<int>();
                password = bs.Read<string>();

                tlsType = bs.Read<string>();
                isSecTls = bs.Read<bool>();
                tlsServName = bs.Read<string>();

                streamType = readString();
                streamParam1 = readString();
                streamParam2 = readString();
                streamParam3 = readString();
            }
        }

        #region public methods
        public override void CopyFromVeeConfig(VeeConfigs vc)
        {
            base.CopyFromVeeConfig(vc);
            password = vc.auth1;
        }

        public override VeeConfigs ToVeeConfigs()
        {
            var vc = base.ToVeeConfigs();
            vc.proto = proto;
            vc.auth1 = password;
            return vc;
        }

        public byte[] ToBytes()
        {
            byte[] result;
            using (var bs = new VgcApis.Libs.Streams.BitStream())
            {
                var writeBasicString = Utils.GenWriteStringHelper(bs, strTable);

                bs.Clear();

                bs.Write(alias);
                writeBasicString(description);
                bs.WriteAddress(address);
                bs.Write(port);
                bs.Write(password);

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

        public bool EqTo(Trojan5b vee)
        {
            if (!EqTo(vee as BasicSettings)
                || password != vee.password)
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
