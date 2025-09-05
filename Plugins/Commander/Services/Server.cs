using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Commander.Resources.Langs;
using VgcApis.Models.Consts;

namespace Commander.Services
{
    public class Server
    {
        public class ProcInfo
        {
            public string name;
            public Process proc;
        }

        readonly object locker = new object();
        List<ProcInfo> procInfos = new List<ProcInfo>();
        readonly VgcApis.Libs.Sys.QueueLogger logger = new VgcApis.Libs.Sys.QueueLogger(1000);
        private readonly Settings settings;

        public Server(Settings settings)
        {
            this.settings = settings;
        }

        #region public methods

        public void ClearLogs() => logger.Clear();

        public long GetLogTimestamp() => logger.GetTimestamp();

        public string GetLogs() => logger.GetLogAsString(true);

        public List<string> GetNames()
        {
            lock (locker)
            {
                return procInfos.Where(pi => !pi.proc.HasExited).Select(pi => pi.name).ToList();
            }
        }

        public void Stop(string name)
        {
            if (TryGetProcByName(name, out var proc))
            {
                LogWithTag(name, I18N.SendStopSignal);
                VgcApis.Misc.Utils.SendStopSignal(proc);
            }
        }

        public void Cleanup()
        {
            var names = GetNames();
            foreach (var name in names)
            {
                Kill(name);
            }
        }

        public void Kill(string name)
        {
            if (TryGetProcByName(name, out var proc))
            {
                LogWithTag(name, I18N.Kill);
                try
                {
                    proc.Kill();
                }
                catch { }
            }
        }

        public void Start(string name)
        {
            var config = settings.GetCmderParamByName(name);
            if (config != null)
            {
                Start(config);
                return;
            }
            var msg = string.Format(I18N.FindNoConfigWihtName, name);
            LogWithTag(I18N.Error, msg);
        }

        public void Start(Models.Data.CmderParam config)
        {
            if (config == null)
            {
                LogWithTag(I18N.Error, I18N.ConfigIsNull);
                return;
            }
            var name = config.name;
            try
            {
                LogWithTag(name, I18N.Start);
                var proc = CreateProcess(config);
                proc.Exited += (s, a) =>
                {
                    LogWithTag(config.name, I18N.Exited);
                    RemoveClosedProcess();
                };
                if (config.hideWindow)
                {
                    BindLoggerEvents(proc);
                }
                else
                {
                    LogWithTag(name, I18N.DisableLogInWindowMode);
                }

                proc.Start();
                if (config.writeToStdIn)
                {
                    var encIn = VgcApis.Misc.Utils.TranslateEncoding(config.stdInEncoding);
                    WriteToStandardInput(proc, config.stdInContent, encIn);
                }

                lock (locker)
                {
                    procInfos.Add(new ProcInfo() { name = name, proc = proc });
                }

                if (config.hideWindow)
                {
                    proc.BeginErrorReadLine();
                    proc.BeginOutputReadLine();
                }
            }
            catch (Exception ex)
            {
                LogWithTag(I18N.Error, ex.Message);
            }
        }
        #endregion

        #region private methods
        void LogWithTag(string tag, string msg)
        {
            logger.Log($"{DateTime.Now} [{tag}] {msg}");
        }

        bool TryGetProcByName(string name, out Process proc)
        {
            proc = null;
            var procInfo = procInfos.FirstOrDefault(pi => pi.name == name);
            if (procInfo == null)
            {
                var msg = string.Format(I18N.FindNoConfigWihtName, name);
                LogWithTag(I18N.Error, msg);
                return false;
            }
            proc = procInfo.proc;
            return true;
        }

        void RemoveClosedProcess()
        {
            var list = procInfos.Where(pi => !pi.proc.HasExited).ToList();
            lock (locker)
            {
                procInfos = list;
            }
        }

        void SendLogHandler(object sender, DataReceivedEventArgs args)
        {
            logger.Log(args.Data);
        }

        void BindLoggerEvents(Process proc)
        {
            proc.ErrorDataReceived += SendLogHandler;
            proc.OutputDataReceived += SendLogHandler;
            proc.Exited += (s, a) =>
            {
                proc.ErrorDataReceived -= SendLogHandler;
                proc.OutputDataReceived -= SendLogHandler;
            };
        }

        void WriteToStandardInput(Process proc, string content, Encoding encoding)
        {
            using (var s = proc.StandardInput.BaseStream)
            using (var w = new StreamWriter(s, encoding))
            {
                w.Write(content);
            }
        }

        Process CreateProcess(Models.Data.CmderParam config)
        {
            var args = Misc.Utils.ReplaceNewLines(config.args);
            var redirect = config.hideWindow;

            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = config.exe,
                    Arguments = args,
                    CreateNoWindow = redirect,
                    UseShellExecute = config.useShell,
                    RedirectStandardOutput = redirect,
                    RedirectStandardError = redirect,
                    RedirectStandardInput = config.writeToStdIn,
                },
            };

            if (redirect)
            {
                var encOut = VgcApis.Misc.Utils.TranslateEncoding(config.stdOutEncoding);
                p.StartInfo.StandardOutputEncoding = encOut;
                p.StartInfo.StandardErrorEncoding = encOut;
            }

            if (!string.IsNullOrEmpty(config.wrkDir))
            {
                p.StartInfo.WorkingDirectory = config.wrkDir;
            }

            p.EnableRaisingEvents = true;
            if (!string.IsNullOrEmpty(config.envVars))
            {
                var trimed = Misc.Utils.ReplaceNewLines(config.envVars);
                VgcApis.Misc.Utils.SetProcessEnvs(p, trimed);
            }
            return p;
        }
        #endregion
    }
}
