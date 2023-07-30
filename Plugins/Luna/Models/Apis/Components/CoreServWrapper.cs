using ImpromptuInterface;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Luna.Models.Apis.Components
{
    public class CoreServWrapper : DynamicObject
    {
        private readonly VgcApis.Interfaces.ICoreServCtrl instance;


        #region public exports
        public static TInterface Wrap<TInterface>(VgcApis.Interfaces.ICoreServCtrl instance)
            where TInterface : class
        {
            if (!typeof(TInterface).IsInterface)
                throw new ArgumentException("TInterface must be an Interface");

            return new CoreServWrapper(instance).ActLike<TInterface>();
        }

        #endregion

        #region ctor
        private CoreServWrapper(VgcApis.Interfaces.ICoreServCtrl instance)
        {
            this.instance = instance;
        }
        #endregion

        #region override
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            InitComposTableOnlyOnce();
            result = null;
            try
            {
                var fn = binder.Name;
                foreach (var comp in comps)
                {
                    var minf = comp.Item1.GetMethod(fn);
                    if (minf != null)
                    {
                        result = minf.Invoke(comp.Item2, args);
                        return true;
                    }
                }
            }
            catch
            { }
            return false;
        }
        #endregion

        #region private methods
        List<Tuple<Type, object>> comps = null;
        readonly object locker = new object();
        void InitComposTableOnlyOnce()
        {
            lock (locker)
            {
                if (comps != null)
                {
                    return;
                }

                comps = new List<Tuple<Type, object>>()
                {
                    new Tuple<Type, object>(typeof(VgcApis.Interfaces.CoreCtrlComponents.IConfiger), instance.GetConfiger()),
                    new Tuple<Type, object>(typeof(VgcApis.Interfaces.CoreCtrlComponents.ICoreCtrl), instance.GetCoreCtrl()),
                    new Tuple<Type, object>(typeof(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates), instance.GetCoreStates()),
                    new Tuple<Type, object>(typeof(VgcApis.Interfaces.CoreCtrlComponents.ILogger), instance.GetLogger()),
                };
            }
        }
        #endregion
    }
}
