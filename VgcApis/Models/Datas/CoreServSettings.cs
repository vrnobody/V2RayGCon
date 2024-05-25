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
            tag1,
            tag2,
            tag3,
            customCoreName,
            templates;

        public double index;
        public bool isAutorun,
            isUntrack,
            isAcceptInjecttion,
            ignoreSendThrough;

        public CoreServSettings()
        {
            var em = string.Empty;

            serverName = em;
            inboundAddress = em;
            mark = em;
            remark = em;
            tag1 = em;
            tag2 = em;
            tag3 = em;

            customCoreName = em;
            templates = em;

            index = 0;
            isAutorun = false;
            isUntrack = false;
            isAcceptInjecttion = true;
        }

        public CoreServSettings(Interfaces.ICoreServCtrl coreServ)
            : this()
        {
            var cs = coreServ.GetCoreStates();

            index = cs.GetIndex();
            mark = cs.GetMark();
            remark = cs.GetRemark();
            tag1 = cs.GetTag1();
            tag2 = cs.GetTag2();
            tag3 = cs.GetTag3();

            inboundName = cs.GetInboundName();
            customCoreName = coreServ.GetCoreCtrl().GetCustomCoreName();
            templates = cs.GetCustomTemplateNames();

            isAutorun = cs.IsAutoRun();
            isUntrack = cs.IsUntrack();
            isAcceptInjecttion = cs.IsAcceptInjection();
            inboundAddress = cs.GetInboundAddr();
            serverName = cs.GetName();

            ignoreSendThrough = cs.IsIgnoreSendThrough();

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
                || t.templates != templates
                || (int)t.index != (int)index
                || t.inboundAddress != inboundAddress
                || t.mark != mark
                || t.remark != remark
                || t.tag1 != tag1
                || t.tag2 != tag2
                || t.tag3 != tag3
                || t.customCoreName != customCoreName
                || t.inboundName != inboundName
                || t.isAutorun != isAutorun
                || t.isUntrack != isUntrack
                || t.isAcceptInjecttion != isAcceptInjecttion
                || t.ignoreSendThrough != ignoreSendThrough
            )
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
