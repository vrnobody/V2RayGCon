using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;

namespace NeoLuna.Models.Apis.SysCmpos
{
    internal class PipedProcess : VgcApis.BaseClasses.Disposable
    {
        readonly Process proc;
        readonly AnonymousPipeServerStream pipeIn, pipeOut;
        readonly VgcApis.Libs.Streams.StringReader reader;
        readonly VgcApis.Libs.Streams.StringWriter writer;

        public PipedProcess(bool hasWindow, string workingDir, string exe, string args)
        {
            pipeIn = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
            pipeOut = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);

            var handleIn = pipeIn.GetClientHandleAsString();
            var handleOut = pipeOut.GetClientHandleAsString();

            proc = CreateProcessWithAnonymousPipe(handleIn, handleOut, hasWindow, workingDir, exe, args);
            reader = new VgcApis.Libs.Streams.StringReader(pipeIn);
            writer = new VgcApis.Libs.Streams.StringWriter(pipeOut);
            proc.Start();
            pipeIn.DisposeLocalCopyOfClientHandle();
            pipeOut.DisposeLocalCopyOfClientHandle();
        }

        #region properties

        #endregion

        #region public methods
        public string Read()
        {
            try
            {
                return reader.Read();
            }
            catch { }
            return null;
        }

        public bool Write(string content)
        {
            try
            {
                writer.Write(content);
                return true;
            }
            catch { }
            return false;
        }
        #endregion

        #region private methods
        Process CreateProcessWithAnonymousPipe(
           string handleIn,
           string handleOut,
           bool hasWindow,
           string workingDir,
           string exe,
           string args)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = exe,

                // in out ·´¹ýÀ´
                Arguments = $"-pipein={handleOut} -pipeout={handleIn} {args}",

                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = !hasWindow,
            };

            if (!string.IsNullOrEmpty(workingDir))
            {
                startInfo.WorkingDirectory = workingDir;
            }

            return new Process
            {
                StartInfo = startInfo,
            };
        }

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            pipeOut.Dispose();
            pipeIn.Dispose();
        }
        #endregion
    }
}