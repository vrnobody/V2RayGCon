﻿using System;
using System.IO.Pipes;
using Newtonsoft.Json;

namespace DyFetch.Comps
{
    internal class Plumber : IDisposable
    {
        readonly AnonymousPipeClientStream pipeIn,
            pipeOut;
        private bool disposedValue;

        public Plumber(string pipeIn, string pipeOut)
        {
            this.pipeIn = new AnonymousPipeClientStream(PipeDirection.In, pipeIn);
            this.pipeOut = new AnonymousPipeClientStream(PipeDirection.Out, pipeOut);
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    Utils.DisposeObject(pipeIn);
                    Utils.DisposeObject(pipeOut);
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Plumber()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region properties

        #endregion

        #region public methods
        public void Work(Fetcher fetcher)
        {
            var reader = new StringReader(pipeIn);
            var writer = new StringWriter(pipeOut);

            string str;
            do
            {
                // 从输入管道读取命令，格式详见Models.Message.cs
                str = reader.Read();
                if (str != null)
                {
                    var msg = JsonConvert.DeserializeObject<Models.Message>(str);
                    Console.WriteLine($"Fetch:{msg.url}");

                    // 调用selenium访问命令指定的url
                    var html = fetcher.Fetch(msg.url, msg.csses, msg.timeout, msg.wait);
                    Console.WriteLine($"Send:{html.Length}");

                    // 把结果写到输出管道
                    writer.Write(html);
                }
                // 如果父进程结束StringReader.Read()将得到null
            } while (str != null);
        }

        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
