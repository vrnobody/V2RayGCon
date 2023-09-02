using ImpromptuInterface;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace V2RayGCon.Controllers.CoreServerComponent
{
    public class Wrapper : DynamicObject
    {
        private readonly VgcApis.Interfaces.ICoreServCtrl coreServCtrl;

        #region public exports
        public static VgcApis.Interfaces.IWrappedCoreServCtrl Wrap(
            VgcApis.Interfaces.ICoreServCtrl coreServCtrl
        )
        {
            if (coreServCtrl == null)
            {
                return null;
            }

            return new Wrapper(coreServCtrl).ActLike<VgcApis.Interfaces.IWrappedCoreServCtrl>();
        }

        #endregion

        #region ctor
        private Wrapper(VgcApis.Interfaces.ICoreServCtrl coreServCtrl)
        {
            this.coreServCtrl = coreServCtrl;
        }
        #endregion

        #region override
        public override bool TryInvokeMember(
            InvokeMemberBinder binder,
            object[] args,
            out object result
        )
        {
            result = null;
            try
            {
                var funcName = binder.Name;
                if (
                    !string.IsNullOrEmpty(funcName)
                    && funcLookupTable.TryGetValue(funcName, out var funcsInfo)
                    && funcsInfo != null
                )
                {
                    // 这里用参数个数会有bug，但是参数名称有时会为0，不能用。
                    var pn = binder.CallInfo.ArgumentCount;
                    if (funcsInfo.TryGetValue(pn, out var tup) && tup != null)
                    {
                        var compName = tup.Item1;
                        var comp = compoLookupTable[compName].Invoke(coreServCtrl);
                        var minf = tup.Item2;
                        result = minf.Invoke(comp, args);
                        return true;
                    }
                }
                else if (funcName == "Unwrap")
                {
                    result = coreServCtrl;
                    return true;
                }
            }
            catch { }
            return false;
        }
        #endregion

        #region private methods
        enum CompNames
        {
            GetCoreCtrl,
            GetConfiger,
            GetCoreStates,
            GetLogger,
        }

        static readonly Dictionary<
            CompNames,
            Func<VgcApis.Interfaces.ICoreServCtrl, object>
        > compoLookupTable = new Dictionary<
            CompNames,
            Func<VgcApis.Interfaces.ICoreServCtrl, object>
        >()
        {
            { CompNames.GetConfiger, (inst) => inst.GetConfiger() },
            { CompNames.GetCoreCtrl, (inst) => inst.GetCoreCtrl() },
            { CompNames.GetCoreStates, (inst) => inst.GetCoreStates() },
            { CompNames.GetLogger, (inst) => inst.GetLogger() },
        };

        static readonly Dictionary<
            string,
            Dictionary<int, Tuple<CompNames, MethodInfo>>
        > funcLookupTable = CreateFuncsLookupTableOnlyOnce();

        static Dictionary<
            string,
            Dictionary<int, Tuple<CompNames, MethodInfo>>
        > CreateFuncsLookupTableOnlyOnce()
        {
            var tups = new Tuple<CompNames, Type>[]
            {
                new Tuple<CompNames, Type>(
                    CompNames.GetConfiger,
                    typeof(VgcApis.Interfaces.CoreCtrlComponents.IConfiger)
                ),
                new Tuple<CompNames, Type>(
                    CompNames.GetCoreCtrl,
                    typeof(VgcApis.Interfaces.CoreCtrlComponents.ICoreCtrl)
                ),
                new Tuple<CompNames, Type>(
                    CompNames.GetCoreStates,
                    typeof(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates)
                ),
                new Tuple<CompNames, Type>(
                    CompNames.GetLogger,
                    typeof(VgcApis.Interfaces.CoreCtrlComponents.ILogger)
                ),
            };

            var table = new Dictionary<string, Dictionary<int, Tuple<CompNames, MethodInfo>>>();
            foreach (var tup in tups)
            {
                var minfs = tup.Item2.GetMethods();
                var compNames = tup.Item1;
                foreach (var minf in minfs)
                {
                    var fn = minf.Name;
                    var dict = new Dictionary<int, Tuple<CompNames, MethodInfo>>();
                    if (table.ContainsKey(fn))
                    {
                        dict = table[fn];
                    }
                    else
                    {
                        table.Add(fn, dict);
                    }

                    var pn = minf.GetParameters().Length;
                    if (!dict.ContainsKey(pn))
                    {
                        var v = new Tuple<CompNames, MethodInfo>(compNames, minf);
                        dict.Add(pn, v);
                    }
                    else
                    {
                        // error
                        throw new ArgumentException("duplicated function params");
                    }
                }
            }
            return table;
        }
        #endregion
    }
}
