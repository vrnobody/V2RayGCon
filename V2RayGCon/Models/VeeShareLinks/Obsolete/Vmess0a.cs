using System;

namespace V2RayGCon.Models.VeeShareLinks.Obsolete
{
    public sealed class Vmess0a :
        BasicSettings
    {
        // ver 0a is optimized for vmess protocol 
        public const string version = @"0a";
        public const string proto = "vmess";

        public int alterId; // 16 bit each
        public Guid uuid;

        public Vmess0a() : base()
        {
            alterId = 0;
            uuid = new Guid(); // zeros   
        }

        public Vmess0a(BasicSettings source) : this()
        {
            CopyFrom(source);
        }
        #region public methods
        public override void CopyFromVeeConfig(Models.Datas.VeeConfigs vc)
        {
            base.CopyFromVeeConfig(vc);
            uuid = Guid.Parse(vc.auth1);
        }

        public override Datas.VeeConfigs ToVeeConfigs()
        {
            var vc = base.ToVeeConfigs();
            vc.proto = proto;
            vc.auth1 = uuid.ToString();
            return vc;
        }

        public Vmess0a(byte[] bytes) :
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
                bs.Write(isSecTls);
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

        public bool EqTo(Vmess0a veeLink)
        {
            if (!EqTo(veeLink as BasicSettings)
                || alterId != veeLink.alterId
                || uuid != veeLink.uuid)
            {
                return false;
            }

            return true;
        }
        #endregion

    }
}
