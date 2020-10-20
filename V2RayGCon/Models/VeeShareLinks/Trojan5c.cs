using System;
using System.Collections.Generic;
using V2RayGCon.Models.Datas;

namespace V2RayGCon.Models.VeeShareLinks
{
    public sealed class Trojan5c : BasicSettings
    {
        // ver 5a is optimized for trojan protocol

        public const string version = @"5c";
        public const string proto = @"trojan";

        public string password; // 256 bytes
        public string flow; // v2fly PR #334

        public Trojan5c()
        {
            password = string.Empty;
            flow = string.Empty;
        }

        public Trojan5c(BasicSettings source) : this()
        {
            CopyFrom(source);
        }

        List<string> flowTypesTable = new List<string>
        {
            "",
            "xtls-rprx-origin",
            "xtls-rprx-origin-udp443",
            "xtls-rprx-direct",
            "xtls-rprx-direct-udp443",
        };

        public Trojan5c(byte[] bytes) :
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
                var readFlowType = Utils.GenReadStringHelper(bs, flowTypesTable);

                alias = bs.Read<string>();
                description = readString();
                address = bs.ReadAddress();
                port = bs.Read<int>();
                password = bs.Read<string>();
                flow = readFlowType();

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
            flow = vc.auth2;
        }

        public override VeeConfigs ToVeeConfigs()
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
                var writeBasicString = Utils.GenWriteStringHelper(bs, strTable);
                var writeFlowType = Utils.GenWriteStringHelper(bs, flowTypesTable);

                bs.Clear();

                bs.Write(alias);
                writeBasicString(description);
                bs.WriteAddress(address);
                bs.Write(port);
                bs.Write(password);
                writeFlowType(flow);

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

        public bool EqTo(Trojan5c vee)
        {
            if (!EqTo(vee as BasicSettings)
                || password != vee.password
                || flow != vee.flow)
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
