using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2RayGCon.Services.ImportComponents
{
    internal interface IDownloadTask
    {
        void Fetch(
            BlockingCollection<string[]> queue,
            bool isSocks5,
            int proxyPort,
            int timeout
        );
    }
}
