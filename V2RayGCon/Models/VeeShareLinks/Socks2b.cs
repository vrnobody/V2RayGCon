using System;

namespace V2RayGCon.Models.VeeShareLinks
{
    public class Socks2b : BasicSettings
    {
        // ver 2a is optimized for socks protocol 
        public const string version = @"2b";
        public const string proto = "socks";

        public string userName, userPassword;

        public Socks2b() : base()
        {
            userName = string.Empty;
            userPassword = string.Empty;
        }

        public Socks2b(BasicSettings source) : this()
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

        public Socks2b(byte[] bytes) :
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

                tlsType = bs.Read<string>();
                isSecTls = bs.Read<bool>();
                tlsServName = bs.Read<string>();

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

                bs.Write(tlsType);
                bs.Write(isSecTls);
                bs.Write(tlsServName);

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

        public bool EqTo(Socks2b target)
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
