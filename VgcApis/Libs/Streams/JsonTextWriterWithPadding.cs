using System;
using System.IO;
using Newtonsoft.Json;

namespace VgcApis.Libs.Streams
{
    internal class JsonTextWriterWithPadding : JsonTextWriter
    {
        readonly TextWriter writer;
        readonly char[] buff;
        readonly int paddingLength;

        public JsonTextWriterWithPadding(TextWriter textWriter)
            : this(textWriter, "") { }

        public JsonTextWriterWithPadding(TextWriter textWriter, string padding)
            : base(textWriter)
        {
            writer = textWriter;
            padding = padding ?? "";
            var nl = writer.NewLine;
            buff = (nl + padding + new string(IndentChar, 12)).ToCharArray();
            paddingLength = nl.Length + padding.Length;
            if (!string.IsNullOrEmpty(padding))
            {
                writer.Write(padding);
            }
        }

        // disable change IndentChar feature
        public new char IndentChar
        {
            get { return base.IndentChar; }
            private set { }
        }

        protected override void WriteIndent()
        {
            int indentLen = Top * Indentation;
            writer.Write(buff, 0, paddingLength + Math.Min(indentLen, 12));
            while ((indentLen -= 12) > 0)
            {
                writer.Write(buff, paddingLength, Math.Min(indentLen, 12));
            }
        }
    }
}
