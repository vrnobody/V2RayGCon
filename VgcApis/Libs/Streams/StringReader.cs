using System;
using System.IO;
using System.Text;

namespace VgcApis.Libs.Streams
{
    public class StringReader
    {
        private readonly Stream stream;

        public StringReader(Stream stream)
        {
            this.stream = stream;
            if (stream.CanTimeout)
            {
                stream.ReadTimeout = Models.Consts.Files.StreamDefaultTimeout;
            }
        }

        #region private methods

        #endregion

        #region public methods
        public string Read()
        {
            var lenBuff = BitConverter.GetBytes((int)0);
            var readed = 0;
            while (readed < lenBuff.Length)
            {
                var c = stream.Read(lenBuff, readed, lenBuff.Length - readed);
                if (c == 0)
                {
                    return null;
                }
                readed += c;
            }
            var len = BitConverter.ToInt32(lenBuff, 0);
            if (len == 0)
            {
                return string.Empty;
            }

            var apms = new ArrayPoolMemoryStream(Encoding.Unicode);
            var buff = Misc.Utils.RentBuffer();
            using (apms)
            {
                readed = 0;
                while (readed < len)
                {
                    var max = Math.Min(buff.Length, len - readed);
                    var c = stream.Read(buff, 0, max);
                    if (c == 0 && readed < len)
                    {
                        break;
                    }
                    else if (c > 0)
                    {
                        readed += c;
                        apms.Write(buff, 0, c);
                    }
                }
            }
            Misc.Utils.ReturnBuffer(buff);
            return apms.GetString() ?? "";
        }
        #endregion
    }
}
