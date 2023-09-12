using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2RayGCon.Controllers.Comm
{
    internal interface IDyMenuHelper
    {
        void LoadConfigByUid(string uid);
        void ReplaceServer(string uid);
    }
}
