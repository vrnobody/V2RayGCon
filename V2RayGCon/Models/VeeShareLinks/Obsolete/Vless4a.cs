using System;

namespace V2RayGCon.Models.VeeShareLinks.Obsolete
{
    public sealed class Vless4a :
        BasicSettings
    {
        // ver 0a is optimized for vmess protocol 
        public const string version = @"4a";
        public const string proto = "vless";

        public Guid uuid;
        public string encryption;

        public Vless4a() : base()
        {
            uuid = new Guid(); // zeros  
            encryption = "none";
        }

        public Vless4a(BasicSettings source) : this()
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

        public Vless4a(byte[] bytes) :
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
                encryption = bs.Read<string>();
                uuid = bs.Read<Guid>();
                address = bs.ReadAddress();
                streamType = readString();
                streamParam1 = readString();
                streamParam2 = readString();
                streamParam3 = readString();
            }

            if (string.IsNullOrEmpty(encryption))
            {
                encryption = "none";
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
                bs.Write(encryption);
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

        public bool EqTo(Vless4a veeLink)
        {
            if (!EqTo(veeLink as BasicSettings)
                || encryption != veeLink.encryption
                || uuid != veeLink.uuid)
            {
                return false;
            }

            return true;
        }
        #endregion

    }
}
