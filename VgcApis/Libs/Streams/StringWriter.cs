using System;
using System.IO;
using System.Text;

namespace VgcApis.Libs.Streams
{
    public class StringWriter
    {
        private readonly Stream stream;

        public StringWriter(Stream stream)
        {
            this.stream = stream;
            if (stream.CanTimeout)
            {
                stream.WriteTimeout = Models.Consts.Files.StreamDefaultTimeout;
            }
        }

        #region private methods

        #endregion

        #region public methods
        public void Write(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                var lenBuff = BitConverter.GetBytes((int)0);
                stream.Write(lenBuff, 0, lenBuff.Length);
                stream.Flush();
                return;
            }

            var encoding = new UnicodeEncoding(false, false);
            using (var apms = new ArrayPoolMemoryStream())
            using (var w = new StreamWriter(apms, encoding))
            {
                w.Write(content);
                w.Flush();
                var lenBuff = BitConverter.GetBytes((int)apms.Length);
                apms.Position = 0;
                stream.Write(lenBuff, 0, lenBuff.Length);
                apms.CopyTo(stream);
            }
        }
        #endregion
    }
}
