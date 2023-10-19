using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace NeoLuna.Models.Apis.SysCmpos
{
    public class HttpServer : VgcApis.BaseClasses.Disposable, Interfaces.IRunnable
    {
        enum SourceType
        {
            None,
            HTML,
            File,
            Folder,
        }

        readonly HttpListener serv;

        private readonly VgcApis.Interfaces.PostOfficeComponents.ILuaMailBox inbox;
        private readonly VgcApis.Interfaces.PostOfficeComponents.ILuaMailBox outbox;
        private readonly string source;
        private readonly SourceType sourceType;
        private readonly bool allowCORS;

        public HttpServer(
            string url,
            VgcApis.Interfaces.PostOfficeComponents.ILuaMailBox inbox,
            VgcApis.Interfaces.PostOfficeComponents.ILuaMailBox outbox,
            string source,
            bool allowCORS
        )
        {
            this.inbox = inbox;
            this.outbox = outbox;
            this.source = source;
            this.allowCORS = allowCORS;

            this.sourceType = SourceType.None;
            if (File.Exists(source))
            {
                sourceType = SourceType.File;
            }
            else if (Directory.Exists(source))
            {
                sourceType = SourceType.Folder;
            }
            else if (!string.IsNullOrEmpty(source) && source.Length > 0)
            {
                sourceType = SourceType.HTML;
            }

            serv = new HttpListener();
            serv.Prefixes.Add(url);
        }

        #region static public folder

        static readonly string[] defaultDocuments =
        {
            "index.html",
            "index.htm",
            "default.html",
            "default.htm"
        };

        static readonly Dictionary<string, string> mimeTypeMaps = new Dictionary<string, string>(
            StringComparer.InvariantCultureIgnoreCase
        )
        {
            #region extension to MIME type list
            { ".asf", "video/x-ms-asf" },
            { ".asx", "video/x-ms-asf" },
            { ".avi", "video/x-msvideo" },
            { ".bin", "application/octet-stream" },
            { ".cco", "application/x-cocoa" },
            { ".crt", "application/x-x509-ca-cert" },
            { ".css", "text/css" },
            { ".deb", "application/octet-stream" },
            { ".der", "application/x-x509-ca-cert" },
            { ".dll", "application/octet-stream" },
            { ".dmg", "application/octet-stream" },
            { ".ear", "application/java-archive" },
            { ".eot", "application/octet-stream" },
            { ".exe", "application/octet-stream" },
            { ".flv", "video/x-flv" },
            { ".gif", "image/gif" },
            { ".hqx", "application/mac-binhex40" },
            { ".htc", "text/x-component" },
            { ".htm", "text/html" },
            { ".html", "text/html" },
            { ".ico", "image/x-icon" },
            { ".img", "application/octet-stream" },
            { ".iso", "application/octet-stream" },
            { ".jar", "application/java-archive" },
            { ".jardiff", "application/x-java-archive-diff" },
            { ".jng", "image/x-jng" },
            { ".jnlp", "application/x-java-jnlp-file" },
            { ".jpeg", "image/jpeg" },
            { ".jpg", "image/jpeg" },
            { ".js", "application/x-javascript" },
            { ".mml", "text/mathml" },
            { ".mng", "video/x-mng" },
            { ".mov", "video/quicktime" },
            { ".mp3", "audio/mpeg" },
            { ".mpeg", "video/mpeg" },
            { ".mpg", "video/mpeg" },
            { ".msi", "application/octet-stream" },
            { ".msm", "application/octet-stream" },
            { ".msp", "application/octet-stream" },
            { ".pdb", "application/x-pilot" },
            { ".pdf", "application/pdf" },
            { ".pem", "application/x-x509-ca-cert" },
            { ".pl", "application/x-perl" },
            { ".pm", "application/x-perl" },
            { ".png", "image/png" },
            { ".prc", "application/x-pilot" },
            { ".ra", "audio/x-realaudio" },
            { ".rar", "application/x-rar-compressed" },
            { ".rpm", "application/x-redhat-package-manager" },
            { ".rss", "text/xml" },
            { ".run", "application/x-makeself" },
            { ".sea", "application/x-sea" },
            { ".shtml", "text/html" },
            { ".sit", "application/x-stuffit" },
            { ".swf", "application/x-shockwave-flash" },
            { ".tcl", "application/x-tcl" },
            { ".tk", "application/x-tcl" },
            { ".txt", "text/plain" },
            { ".war", "application/java-archive" },
            { ".wbmp", "image/vnd.wap.wbmp" },
            { ".wmv", "video/x-ms-wmv" },
            { ".xml", "text/xml" },
            { ".xpi", "application/x-xpinstall" },
            { ".zip", "application/zip" },

            #endregion
        };

        #endregion

        #region public methods
        public void Start()
        {
            try
            {
                Stop();
                serv.Start();
                HandleConnOut();
                HandleConnIn();
            }
            catch { }
        }

        public void Stop()
        {
            try
            {
                serv.Stop();
            }
            catch { }
        }
        #endregion

        #region protected
        protected override void Cleanup()
        {
            Stop();
        }
        #endregion

        #region private methods
        const int FinalStageConnInLimit = 1024 * 30;
        const int FirstStageConnInLimit = 1024 * 15;
        readonly ConcurrentDictionary<string, HttpListenerContext> contexts =
            new ConcurrentDictionary<string, HttpListenerContext>();

        void HandleConnOut()
        {
            VgcApis.Misc.Utils.RunInBgSlim(() =>
            {
                try
                {
                    while (true)
                    {
                        var mail = outbox.Wait();
                        if (mail == null)
                        {
                            break;
                        }

                        if (!contexts.TryRemove(mail.title, out var ctx))
                        {
                            continue;
                        }

                        HandleOneConnOut(ctx, mail.GetContent());
                    }
                }
                catch { }
            });
        }

        void HandleOneConnOut(HttpListenerContext ctx, string content)
        {
            VgcApis.Misc.Utils.RunInBgSlim(() =>
            {
                try
                {
                    var resp = ctx.Response;
                    if (allowCORS)
                    {
                        resp.AppendHeader("Access-Control-Allow-Origin", "*");
                    }
                    var encoding = ctx.Request.ContentEncoding;
                    var buff = encoding.GetBytes(content ?? "");
                    resp.ContentLength64 = buff.Length;
                    using (var s = resp.OutputStream)
                    {
                        s.Write(buff, 0, buff.Length);
                    }
                    resp.Close();
                }
                catch { }
            });
        }

        void HandleConnIn()
        {
            VgcApis.Misc.Utils.RunInBgSlim(() =>
            {
                try
                {
                    while (serv.IsListening)
                    {
                        var ctx = serv.GetContext();

                        if (contexts.Keys.Count > FirstStageConnInLimit)
                        {
                            VgcApis.Misc.Utils.Sleep(100);
                        }

                        if (contexts.Keys.Count > FinalStageConnInLimit)
                        {
                            VgcApis.Misc.Utils.RunInBgSlim(() =>
                            {
                                try
                                {
                                    ctx.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                                    ctx.Response.OutputStream.Close();
                                }
                                catch { }
                            });
                            continue;
                        }

                        try
                        {
                            HandleOneConnection(ctx);
                        }
                        catch { }
                    }
                }
                catch { }
                ;
            });
        }

        HttpMethods GetHttpMethodFromContext(HttpListenerContext ctx)
        {
            var method = ctx.Request.HttpMethod?.ToUpper();
            switch (method)
            {
                case "GET":
                    return HttpMethods.Get;
                case "POST":
                    return HttpMethods.Post;
                default:
                    return HttpMethods.Others;
            }
        }

        enum HttpMethods
        {
            Others = 0,
            Post = 1,
            Get = 2,
        }

        void HandleOneConnection(HttpListenerContext ctx)
        {
            var method = GetHttpMethodFromContext(ctx);
            if (method != HttpMethods.Get)
            {
                ConnInDefaultHandler(ctx);
                return;
            }

            switch (sourceType)
            {
                case SourceType.Folder:
                    ConnInFolderHandler(ctx);
                    return;
                case SourceType.File:
                    ConnInFileHandler(ctx);
                    return;
                case SourceType.HTML:
                    ConnInHtmlHandler(ctx);
                    return;
                case SourceType.None:
                default:
                    break;
            }

            ConnInDefaultHandler(ctx);
        }

        void ConnInFileHandler(HttpListenerContext context)
        {
            ResponseWithFile(context, source);
            context.Response.OutputStream.Close();
        }

        void ConnInHtmlHandler(HttpListenerContext context)
        {
            try
            {
                var input = new MemoryStream();
                var w = new StreamWriter(input);
                w.Write(source);
                w.Flush();
                input.Position = 0;

                context.Response.ContentType = "text/html";
                ResponseWithStream(context, input, DateTime.Today);
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            context.Response.OutputStream.Close();
        }

        void ConnInFolderHandler(HttpListenerContext context)
        {
            string filename = context.Request.Url.AbsolutePath;
            filename = filename.Substring(1);

            if (string.IsNullOrEmpty(filename))
            {
                foreach (string indexFile in defaultDocuments)
                {
                    if (File.Exists(Path.Combine(source, indexFile)))
                    {
                        filename = indexFile;
                        break;
                    }
                }
            }

            filename = Path.Combine(source, filename);

            ResponseWithFile(context, filename);
            context.Response.OutputStream.Close();
        }

        void ResponseWithFile(HttpListenerContext context, string filename)
        {
            if (!File.Exists(filename))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            try
            {
                Stream input = new FileStream(filename, FileMode.Open);
                string mime;
                context.Response.ContentType = mimeTypeMaps.TryGetValue(
                    Path.GetExtension(filename),
                    out mime
                )
                    ? mime
                    : "application/octet-stream";
                var lastModified = File.GetLastWriteTime(filename);
                ResponseWithStream(context, input, lastModified);
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        void ResponseWithStream(HttpListenerContext context, Stream input, DateTime lastModified)
        {
            context.Response.ContentLength64 = input.Length;
            context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
            context.Response.AddHeader("Last-Modified", lastModified.ToString("r"));

            byte[] buffer = new byte[1024 * 32];
            int nbytes;
            while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                context.Response.OutputStream.Write(buffer, 0, nbytes);
            input.Close();
            context.Response.OutputStream.Flush();
            context.Response.StatusCode = (int)HttpStatusCode.OK;
        }

        void ConnInDefaultHandler(HttpListenerContext context)
        {
            var req = context.Request;
            string text;

            using (var reader = new StreamReader(req.InputStream, req.ContentEncoding))
            {
                text = reader.ReadToEnd();
            }

            var id = Guid.NewGuid().ToString();
            contexts.TryAdd(id, context);
            var code = GetHttpMethodFromContext(context);
            inbox.Send(inbox.GetAddress(), (int)code, id, true, text ?? "");
        }

        #endregion
    }
}
