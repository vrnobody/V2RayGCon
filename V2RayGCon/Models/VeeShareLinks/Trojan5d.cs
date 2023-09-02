using System;
using V2RayGCon.Models.Datas;

namespace V2RayGCon.Models.VeeShareLinks
{
    public sealed class Trojan5d : BasicSettingsWithReality
    {
        // ver 5a is optimized for trojan protocol

        public const string version = @"5d";
        public const string proto = @"trojan";

        public string password; // 256 bytes
        public string flow; // v2fly PR #334

        public Trojan5d()
        {
            password = string.Empty;
            flow = string.Empty;
        }

        public Trojan5d(BasicSettingsWithReality source)
            : this()
        {
            CopyFrom(source);
        }

        public Trojan5d(byte[] bytes)
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

                alias = bs.Read<string>();
                description = readString();

                tlsType = readString();
                tlsServName = bs.Read<string>();
                tlsFingerPrint = readString();
                tlsAlpn = bs.Read<string>();

                tlsParam1 = bs.Read<string>();
                tlsParam2 = bs.Read<string>();
                tlsParam3 = bs.Read<string>();

                port = bs.Read<int>();
                password = bs.Read<string>();
                flow = readString();
                address = bs.ReadAddress();

                streamType = readString();
                streamParam1 = readString();
                streamParam2 = readString();
                streamParam3 = readString();
            }
        }

        #region public methods
        public override void CopyFromVeeConfig(VeeConfigsWithReality vc)
        {
            base.CopyFromVeeConfig(vc);
            password = vc.auth1;
            flow = vc.auth2;
        }

        public new VeeConfigsWithReality ToVeeConfigs()
        {
            var vc = base.ToVeeConfigs();
            vc.proto = proto;
            vc.auth1 = password;
            vc.auth2 = flow;
            return vc;
        }

        public byte[] ToBytes()
        {
            byte[] result;
            using (var bs = new VgcApis.Libs.Streams.BitStream())
            {
                var writeString = Utils.GenWriteStringHelper(bs, strTable);

                bs.Clear();

                bs.Write(alias);
                writeString(description);

                writeString(tlsType);
                bs.Write(tlsServName);
                writeString(tlsFingerPrint);
                bs.Write(tlsAlpn);

                bs.Write(tlsParam1);
                bs.Write(tlsParam2);
                bs.Write(tlsParam3);

                bs.Write(port);
                bs.Write(password);
                writeString(flow);
                bs.WriteAddress(address);

                writeString(streamType);
                writeString(streamParam1);
                writeString(streamParam2);
                writeString(streamParam3);

                result = bs.ToBytes(version);
            }
            return result;
        }

        public bool EqTo(Trojan5d vee)
        {
            if (!EqTo(vee as BasicSettings) || password != vee.password || flow != vee.flow)
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
