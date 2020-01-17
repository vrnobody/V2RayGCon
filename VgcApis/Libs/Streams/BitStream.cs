using System;
using System.Text;

namespace VgcApis.Libs.Streams
{
    public sealed class BitStream :
        Models.BaseClasses.Disposable
    {
        const int BitsPerInt = Models.Consts.BitStream.BitsPerInt;
        const int Crc8ByteIndex = Models.Consts.BitStream.Crc8ByteIndex;

        readonly object rbsWriteLock = new object();

        RawBitStream.RawBitStream rawBitStream;
        RawBitStream.Numbers numberWriter;
        RawBitStream.Bytes bytesWriter;
        RawBitStream.Uuids uuidsWriter;
        RawBitStream.Address addressWriter;

        public BitStream(byte[] bytes) : this()
        {
            var crc8 = Crc8.ComputeChecksum(bytes, Crc8ByteIndex +1);
            if (bytes[Crc8ByteIndex] != crc8)
            {
                throw new ArgumentException(
                    @"Crc8 checksum not match!");
            }

            rawBitStream.FromBytes(bytes);
        }

        public BitStream()
        {
            rawBitStream = new RawBitStream.RawBitStream();
            rawBitStream.Run();
            numberWriter = rawBitStream.GetComponent<RawBitStream.Numbers>();
            bytesWriter = rawBitStream.GetComponent<RawBitStream.Bytes>();
            uuidsWriter = rawBitStream.GetComponent<RawBitStream.Uuids>();
            addressWriter = rawBitStream.GetComponent<RawBitStream.Address>();
        }

        #region properties

        #endregion

        #region static methods
        public static string ReadVersion(byte[] bytes) =>
            RawBitStream.Utils.ReadVersion(bytes);
        #endregion

        #region public methods
        public void Rewind() =>
            rawBitStream.Rewind();

        public byte[] ToBytes(string version)
        {
            var bytes = rawBitStream.ToBytes();
            RawBitStream.Utils.WriteVersion(version, bytes);
            bytes[Crc8ByteIndex] = Crc8.ComputeChecksum(
                bytes, Crc8ByteIndex + 1);
            return bytes;
        }

        public void Clear()
        {
            lock (rbsWriteLock)
            {
                rawBitStream.Clear();
            }
        }

        public int ReadTinyInt(int len)
        {
            CheckIntLen(len);
            lock (rbsWriteLock)
            {
                return numberWriter.Read(len);
            }
        }

        public string ReadAddress()
        {
            lock (rbsWriteLock)
            {
                return addressWriter.Read();
            }
        }

        public T Read<T>()
        {
            lock (rbsWriteLock)
            {
                var typeName = typeof(T).Name;
                switch (typeof(T).Name)
                {
                    case nameof(Int32):
                        return (T)(numberWriter.Read(BitsPerInt) as object);
                    case nameof(Boolean):
                        return (T)((rawBitStream.Read() == true) as object);
                    case nameof(Guid):
                        return (T)(uuidsWriter.Read() as object);
                    case nameof(String):
                        var cache = bytesWriter.Read();
                        var result = Encoding.UTF8.GetString(cache);
                        return (T)(result as object);
                    default:
                        throw new NotSupportedException($"Not support type {typeName}");
                }
            }
        }

        public void WriteTinyInt(int number, int len)
        {
            CheckIntLen(len);

            lock (rbsWriteLock)
            {
                numberWriter.Write(number, len);
            }
        }

        public void WriteAddress(string address)
        {
            lock (rbsWriteLock)
            {
                addressWriter.Write(address);
            }
        }

        public void Write<T>(T val)
        {
            lock (rbsWriteLock)
            {
                switch (val)
                {
                    case int number:
                        numberWriter.Write(number, BitsPerInt);
                        break;
                    case bool flag:
                        rawBitStream.Write(flag);
                        break;
                    case Guid uuid:
                        uuidsWriter.Write(uuid);
                        break;
                    case string str:
                        var cache = Encoding.UTF8.GetBytes(str);
                        bytesWriter.Write(cache);
                        break;
                    default:
                        throw new NotSupportedException(
                            $"Not supported type {typeof(T)}");
                }
            }
        }
        #endregion

        #region private methods
        private static void CheckIntLen(int len)
        {
            if (len > BitsPerInt)
            {
                throw new OverflowException(
                    $"Int must less then {BitsPerInt} bit");
            }
        }
        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            rawBitStream?.Dispose();
        }
        #endregion
    }
}
