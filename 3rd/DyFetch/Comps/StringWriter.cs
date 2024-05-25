﻿using System;
using System.IO;
using System.Text;

namespace DyFetch.Comps
{
    public class StringWriter
    {
        private readonly Stream stream;

        public StringWriter(Stream stream)
        {
            this.stream = stream;
            if (stream.CanTimeout)
            {
                stream.WriteTimeout = 10 * 60 * 1000;
            }
        }

        #region private methods

        #endregion

        #region public methods
        public void Write(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                var lenBuff = BitConverter.GetBytes(0);
                stream.Write(lenBuff, 0, lenBuff.Length);
            }
            else
            {
                var buff = Encoding.Unicode.GetBytes(content);
                var lenBuff = BitConverter.GetBytes(buff.Length);
                stream.Write(lenBuff, 0, lenBuff.Length);
                stream.Write(buff, 0, buff.Length);
            }
            stream.Flush();
        }
        #endregion
    }
}
