using System;

namespace V2RayGCon.Models.VeeShareLinks
{
    public sealed class Vless4c :
        BasicSettingsWithReality
    {
        // ver 0a is optimized for vmess protocol 
        public const string version = @"4c";
        public const string proto = "vless";

        public Guid uuid;
        public string encryption;
        public string flow;

        public Vless4c() : base()
        {
            uuid = new Guid(); // zeros  
            encryption = @"none";
            flow = string.Empty;
        }

        public Vless4c(BasicSettingsWithReality source) : this()
        {
            CopyFrom(source);
        }

        #region public methods
        public override void CopyFromVeeConfig(Datas.VeeConfigsWithReality vc)
        {
            base.CopyFromVeeConfig(vc);
            uuid = Guid.Parse(vc.auth1);
            flow = vc.auth2;
        }

        public new Datas.VeeConfigsWithReality ToVeeConfigs()
        {
            var vc = base.ToVeeConfigs();
            vc.proto = proto;
            vc.auth1 = uuid.ToString();
            vc.auth2 = flow;
            return vc;
        }

        public Vless4c(byte[] bytes) :
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
                tlsServName = bs.Read<string>();
                tlsFingerPrint = bs.Read<string>();
                tlsAlpn = bs.Read<string>();

                tlsParam1 = bs.Read<string>();
                tlsParam2 = bs.Read<string>();
                tlsParam3 = bs.Read<string>();

                port = bs.Read<int>();
                encryption = bs.Read<string>();
                uuid = bs.Read<Guid>();
                flow = bs.Read<string>();
                address = bs.ReadAddress();
                streamType = readString();
                streamParam1 = readString();
                streamParam2 = readString();
                streamParam3 = readString();
            }

            isSecTls = true;
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

                bs.Write(tlsType);
                bs.Write(tlsServName);
                bs.Write(tlsFingerPrint);
                bs.Write(tlsAlpn);

                bs.Write(tlsParam1);
                bs.Write(tlsParam2);
                bs.Write(tlsParam3);

                bs.Write(port);
                bs.Write(encryption);
                bs.Write(uuid);
                bs.Write(flow);
                bs.WriteAddress(address);
                writeString(streamType);
                writeString(streamParam1);
                writeString(streamParam2);
                writeString(streamParam3);

                result = bs.ToBytes(version);
            }

            return result;
        }

        public bool EqTo(Vless4c veeLink)
        {
            if (!base.EqTo(veeLink)
                || encryption != veeLink.encryption
                || uuid != veeLink.uuid
                || flow != veeLink.flow)
            {
                return false;
            }

            return true;
        }
        #endregion

    }
}
