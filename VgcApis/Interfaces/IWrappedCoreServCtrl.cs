using Newtonsoft.Json.Linq;
using System;

namespace VgcApis.Interfaces
{
    public interface IWrappedCoreServCtrl
        : CoreCtrlComponents.IConfiger,
            CoreCtrlComponents.ICoreCtrl,
            CoreCtrlComponents.ICoreStates,
            CoreCtrlComponents.ILogger
    {
        // 注意 ICore*** 系列接口不可以出现同名而且参数个数相同的方法
        // 不然反射时会匹配成错误的函数

        ICoreServCtrl Unwrap();
    }
}
