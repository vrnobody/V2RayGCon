using System;

namespace VgcApis.Libs.Streams.RawBitStream
{
    public sealed class Uuids :
        Models.BaseClasses.ComponentOf<RawBitStream>
    {
        const int BytesPerUuid = Models.Consts.BitStream.BytesPerUuid;
        Bytes bytesWriter;
        public Uuids() { }

        public void Run(Bytes bytes)
        {
            this.bytesWriter = bytes;
        }

        #region properties

        #endregion

        #region public methods
        public void Write(Guid uuid) =>
            bytesWriter.Write(uuid.ToByteArray(), BytesPerUuid);

        public Guid Read()
        {
            var uuid = bytesWriter.Read(BytesPerUuid);
            return new Guid(uuid);
        }

        #endregion

        #region private methods
        #endregion

        #region protected methods

        #endregion
    }
}
