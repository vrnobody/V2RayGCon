using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VgcApis.Interfaces.CoreCtrlComponents
{
    public interface IV2RayCoreOwner
    {
        void OnV2RayCoreLog(string message);
        void OnV2RayCoreStart();
        void OnV2RayCoreStop();
    }
}
