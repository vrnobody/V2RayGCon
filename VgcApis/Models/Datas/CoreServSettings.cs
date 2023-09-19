using Newtonsoft.Json.Linq;

namespace VgcApis.Models.Datas
{
    public class CoreServSettings
    {
        public string serverName,
            inboundName,
            inboundAddress,
            mark,
            remark,
            customCoreName;

        public double index;
        public bool isAutorun,
            isUntrack;

        public CoreServSettings()
        {
            var em = string.Empty;

            serverName = em;
            inboundAddress = em;
            mark = em;
            remark = em;
            customCoreName = em;

            index = 0;
            isAutorun = false;
            isUntrack = false;
        }

        public CoreServSettings(Interfaces.ICoreServCtrl coreServ)
            : this()
        {
            var cs = coreServ.GetCoreStates();

            index = cs.GetIndex();
            mark = cs.GetMark();
            remark = cs.GetRemark();
            inboundName = cs.GetInboundName();
            customCoreName = coreServ.GetCoreCtrl().GetCustomCoreName();

            isAutorun = cs.IsAutoRun();
            isUntrack = cs.IsUntrack();
            inboundAddress = cs.GetInboundAddr();
            serverName = cs.GetName();

            try
            {
                var ccfg = coreServ.GetConfiger();
                var cfg = ccfg.GetConfig();
                var json = JObject.Parse(cfg);
                var GetStr = Misc.Utils.GetStringByKeyHelper(json);
            }
            catch { }
        }

        public override bool Equals(object target)
        {
            if (target == null || !(target is CoreServSettings))
            {
                return false;
            }

            var t = target as CoreServSettings;
            if (
                t.serverName != serverName
                || (int)t.index != (int)index
                || t.inboundAddress != inboundAddress
                || t.mark != mark
                || t.remark != remark
                || t.customCoreName != customCoreName
                || t.inboundName != inboundName
                || t.isAutorun != isAutorun
                || t.isUntrack != isUntrack
            )
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
