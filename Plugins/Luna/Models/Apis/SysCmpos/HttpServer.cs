using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;

namespace Luna.Models.Apis.SysCmpos
{
    public class HttpServer : VgcApis.BaseClasses.Disposable
    {

        HttpListener serv;


        private readonly VgcApis.Interfaces.Lua.ILuaMailBox inbox;
        private readonly VgcApis.Interfaces.Lua.ILuaMailBox outbox;

        public HttpServer(
            string url,
            VgcApis.Interfaces.Lua.ILuaMailBox inbox,
            VgcApis.Interfaces.Lua.ILuaMailBox outbox)
        {
            this.inbox = inbox;
            this.outbox = outbox;
            serv = new HttpListener();
            serv.Prefixes.Add(url);
        }

        #region public methods
        public bool Start()
        {
            try
            {
                Stop();
                serv.Start();
                HandleConnOut();
                HandleConnIn();
                return true;
            }
            catch { }
            return false;
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
        const int MaxContextLen = 10240;

        ConcurrentDictionary<string, HttpListenerContext> contexts = new ConcurrentDictionary<string, HttpListenerContext>();

        void HandleConnOut()
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
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
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                try
                {
                    var resp = ctx.Response;
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
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                try
                {
                    while (serv.IsListening)
                    {
                        if (contexts.Keys.Count > MaxContextLen)
                        {
                            VgcApis.Misc.Utils.Sleep(100);
                            continue;
                        }

                        var ctx = serv.GetContext();
                        try
                        {
                            HandleOneConnection(ctx);
                        }
                        catch { }
                    }
                }
                catch { };
            });
        }

        int HttpMethodToCode(string method)
        {
            switch (method)
            {
                case "POST":
                    return 1;
                default:
                    return 0;
            }
        }

        void HandleOneConnection(HttpListenerContext ctx)
        {
            var req = ctx.Request;
            var code = HttpMethodToCode(req.HttpMethod);

            string text;
            using (var reader = new StreamReader(req.InputStream, req.ContentEncoding))
            {
                text = reader.ReadToEnd();
            }

            var id = Guid.NewGuid().ToString();
            contexts.TryAdd(id, ctx);
            inbox.Send(inbox.GetAddress(), code, id, true, text ?? "");
        }

        #endregion


    }
}
