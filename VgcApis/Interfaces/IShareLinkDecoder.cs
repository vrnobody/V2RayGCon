﻿using System;
using System.Collections.Generic;

namespace VgcApis.Interfaces
{
    public interface IShareLinkDecoder : IDisposable
    {
        /// <summary>
        /// Return empty list if fail.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        List<string> ExtractLinksFromText(string text);

        /// <summary>
        /// Return null if decode fail!
        /// </summary>
        Models.Datas.DecodeResult Decode(string shareLink);

        /// <summary>
        /// Return null if encode fail!
        /// </summary>
        string Encode(string name, string config);
    }
}
