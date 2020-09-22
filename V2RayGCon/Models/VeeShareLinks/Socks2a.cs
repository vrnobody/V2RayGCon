using System;

namespace V2RayGCon.Models.VeeShareLinks
{
    public class Socks2a : BasicSettings
    {
        // ver 2a is optimized for socks protocol 
        const string version = @"2a";
        const string proto = "socks";

        public string userName, userPassword;

        static public bool IsDecoderFor(string ver) => version == ver;

        static public bool IsEncoderFor(string protocol) => protocol == proto;
        public Socks2a() : base()
        {
            userName = string.Empty;
            userPassword = string.Empty;
        }

        public Socks2a(BasicSettings source) : this()
        {
            CopyFrom(source);
        }

        #region public methods
        public override void CopyFromVeeConfig(Models.Datas.VeeConfigs vc)
        {
            base.CopyFromVeeConfig(vc);
            userName = vc.auth1;
            userPassword = vc.auth2;
        }

        public override Datas.VeeConfigs ToVeeConfigs()
        {
            var vc = base.ToVeeConfigs();
            vc.proto = proto;
            vc.auth1 = userName;
            vc.auth2 = userPassword;
            return vc;
        }

        public Socks2a(byte[] bytes) :
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
                isUseTls = bs.Read<bool>();
                isSecTls = bs.Read<bool>();
                port = bs.Read<int>();
                address = bs.ReadAddress();
                userName = readString();
                userPassword = readString();
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
                bs.Write(isSecTls);
                bs.Write(port);
                bs.WriteAddress(address);
                writeString(userName);
                writeString(userPassword);
                writeString(streamType);
                writeString(streamParam1);
                writeString(streamParam2);
                writeString(streamParam3);

                result = bs.ToBytes(version);
            }

            return result;
        }

        public bool EqTo(Socks2a target)
        {
            if (!EqTo(target as BasicSettings)
                || userName != target.userName
                || userPassword != target.userPassword)
            {
                return false;
            }
            return true;
        }
        #endregion

    }
}
