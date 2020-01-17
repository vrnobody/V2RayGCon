using System;

namespace V2RayGCon.Model.Data
{
    public class Shadowsocks
    {
        public string method, pass, addr;

        public Shadowsocks()
        {
            method = String.Empty;
            pass = String.Empty;
            addr = String.Empty;
        }
    }
}
