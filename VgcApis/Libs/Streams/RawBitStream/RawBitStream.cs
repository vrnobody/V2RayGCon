using System.Collections.Generic;

namespace VgcApis.Libs.Streams.RawBitStream
{
    public sealed class RawBitStream :
        Models.BaseClasses.ComponentOf<RawBitStream>
    {
        int readPos = 0;
        List<bool> bitStream = new List<bool>();

        public RawBitStream() { }

        public void Run()
        {
            var numbers = new Numbers();
            var bytes = new Bytes();
            var uuids = new Uuids();
            var address = new Address();

            Plug(this, numbers);
            Plug(this, bytes);
            Plug(this, uuids);
            Plug(this, address);

            bytes.Run(numbers);
            uuids.Run(bytes);
            address.Run(bytes);
        }

        #region public methods
        public void FromBytes(byte[] bytes)
        {
            Clear();
            bitStream = Utils.Bytes2BoolList(bytes);
        }

        public byte[] ToBytes() =>
            Utils.BoolList2Bytes(bitStream);

        public int Count() => bitStream.Count;

        public int GetIndex() => readPos;

        public void Clear()
        {
            readPos = 0;
            bitStream = new List<bool>();
        }

        public void Rewind() => readPos = 0;

        public List<bool> Read(int len)
        {
            var result = new List<bool>();
            for (int i = 0; i < len && readPos < bitStream.Count; i++)
            {
                var val = bitStream[readPos++];
                result.Add(val);
            }
            return result;
        }

        public void Write(IEnumerable<bool> values)
        {
            foreach (var val in values)
            {
                bitStream.Add(val);
            }
        }

        // for unit test only
        public void Write(bool val)
        {
            bitStream.Add(val);
        }

        // for unit test only
        public bool? Read()
        {
            if (readPos >= bitStream.Count)
            {
                return null;
            }
            var result = bitStream[readPos++];
            return result;
        }
        #endregion

        #region private methods
        #endregion

    }
}
