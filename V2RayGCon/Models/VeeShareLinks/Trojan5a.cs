using System;

namespace V2RayGCon.Models.VeeShareLinks
{
    public sealed class Trojan5a : BasicSettings
    {
        // ver 1a is optimized for shadowshocks protocol

        const string version = @"5a";
        static public bool IsDecoderFor(string ver) => version == ver;

        static public bool IsEncoderFor(string protocol) => protocol == "trojan";

        public string password; // 256 bytes

        public Trojan5a()
        {
            password = string.Empty;

        }

        public Trojan5a(BasicSettings source) : this()
        {
            CopyFrom(source);
        }

        public Trojan5a(byte[] bytes) :
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
                isUseTls = bs.Read<bool>();
                isSecTls = bs.Read<bool>();
                streamType = readString();
                streamParam1 = readString();
                streamParam2 = readString();
                streamParam3 = readString();
            }
        }

        #region string table for compression 

        #endregion

        #region public methods

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
