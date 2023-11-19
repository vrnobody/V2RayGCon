namespace VgcApis.Interfaces.CoreCtrlComponents
{
    public interface ICoreStates
    {
        /// <summary>
        /// 获取自定义模板名
        /// </summary>
        /// <returns>逗号分隔的模板名</returns>
        string GetCustomTemplateNames();

        /// <summary>
        /// 设置自定义模板名
        /// </summary>
        /// <param name="tpls">逗号分隔的模板名</param>
        void SetCustomTemplateNames(string tpls);

        /// <summary>
        /// 内部使用，脚本一般用不到
        /// </summary>
        /// <returns></returns>
        Models.Datas.CoreInfo GetAllRawCoreInfo();

        /// <summary>
        /// 获取流量统计上传总量
        /// </summary>
        /// <returns>上传总量</returns>
        long GetUplinkTotalInBytes();

        /// <summary>
        /// 获取流量统计下载总量
        /// </summary>
        /// <returns>下载总量</returns>
        long GetDownlinkTotalInBytes();

        /// <summary>
        /// 修改流量统计上传总量（用于定期重置流量统计）
        /// </summary>
        /// <param name="sizeInBytes">任意数值</param>
        void SetUplinkTotal(long sizeInBytes);

        /// <summary>
        /// 修改流量统计下载总量（用于定期重置流量统计）
        /// </summary>
        /// <param name="sizeInBytes">任意数值</param>
        void SetDownlinkTotal(long sizeInBytes);

        /// <summary>
        /// 获取inbound地址（IP + Port)
        /// </summary>
        /// <returns>inbound地址</returns>
        string GetInboundAddr();

        /// <summary>
        /// 获取inbound IP
        /// </summary>
        /// <returns>inbound ip</returns>
        string GetInboundIp();

        /// <summary>
        /// 获取inbound Port
        /// </summary>
        /// <returns>inbound port</returns>
        int GetInboundPort();

        /// <summary>
        /// 获取自定义inbound名
        /// </summary>
        /// <returns>inbound名</returns>
        string GetInboundName();

        /// <summary>
        /// 获取服务器序号
        /// </summary>
        /// <returns>序号</returns>
        double GetIndex();

        /// <summary>
        /// 获取服务器上次修改日期
        /// </summary>
        /// <returns>上次修改日期</returns>
        long GetLastModifiedUtcTicks();

        /// <summary>
        /// 获取服务器上次延迟测试日期
        /// </summary>
        /// <returns>次延迟测试日期</returns>
        long GetLastSpeedTestUtcTicks();

        /// <summary>
        /// 获取服务器标记
        /// </summary>
        /// <returns>标记</returns>
        string GetMark();

        /// <summary>
        /// 获取服务器备注
        /// </summary>
        /// <returns>备注</returns>
        string GetRemark();

        /// <summary>
        /// 获取服务器流量统计API端口号
        /// </summary>
        /// <returns>统计API端口号</returns>
        int GetStatPort();

        /// <summary>
        /// 获取服务器短名
        /// </summary>
        /// <returns>短名</returns>
        string GetShortName();

        /// <summary>
        /// 获取服务器长名
        /// </summary>
        /// <returns>长名</returns>
        string GetLongName();

        /// <summary>
        /// 获取服务器全名
        /// </summary>
        /// <returns>全名</returns>
        string GetName();

        /// <summary>
        /// 设置服务器全名
        /// </summary>
        /// <returns></returns>
        void SetName(string name);

        /// <summary>
        /// 获取服务器原始GUID（可能为空）
        /// </summary>
        /// <returns>GUID</returns>
        string GetRawUid();

        /// <summary>
        /// 获取服务器延迟测试数值
        /// </summary>
        /// <returns>延迟数值</returns>
        long GetSpeedTestResult();

        /// <summary>
        /// 获取服务器延迟标签内容
        /// </summary>
        /// <returns>延迟标签内容</returns>
        string GetStatus();

        /// <summary>
        /// 获取服务器摘要
        /// </summary>
        /// <returns>摘要</returns>
        string GetSummary();

        /// <summary>
        /// 获取自定义标签1的内容
        /// </summary>
        /// <returns>标签内容</returns>
        string GetTag1();

        /// <summary>
        /// 获取自定义标签2的内容
        /// </summary>
        /// <returns>标签内容</returns>
        string GetTag2();

        /// <summary>
        /// 获取自定义标签3的内容
        /// </summary>
        /// <returns>标签内容</returns>
        string GetTag3();

        /// <summary>
        /// 获取服务器标题（序号+短名+摘要）
        /// </summary>
        /// <returns>标题</returns>
        string GetTitle();

        /// <summary>
        /// 获取服务器GUID（为空时自动生成一个）
        /// </summary>
        /// <returns>GUID</returns>
        string GetUid();

        /// <summary>
        /// 当前服务器是否设置为自启动
        /// </summary>
        /// <returns>自启动</returns>
        bool IsAutoRun();

        /// <summary>
        /// 当前服务器是否设置为不追踪
        /// </summary>
        /// <returns>不追踪</returns>
        bool IsUntrack();

        /// <summary>
        /// 当前服务器是否设置为不注入
        /// </summary>
        /// <returns>不注入</returns>
        bool IsAcceptInjection();

        bool IsIgnoreSendThrough();

        void SetIgnoreSendThrough(bool isIgnored);

        /// <summary>
        /// 当前服务器是否已选中
        /// </summary>
        /// <returns>选中</returns>
        bool IsSelected();

        /// <summary>
        /// 设置服务器序号并刷新主窗口
        /// </summary>
        /// <param name="index">序号</param>
        void SetIndex(double index);

        /// <summary>
        /// 仅设置服务器序号不刷新主窗口
        /// </summary>
        /// <param name="index">序号</param>
        void SetIndexQuiet(double index);

        /// <summary>
        /// 设置服务器选中状态
        /// </summary>
        /// <param name="selected">是否选中</param>
        void SetIsSelected(bool selected);

        void SetInboundIp(string ip);
        void SetInboundPort(int port);

        /// <summary>
        /// 设置服务器inbound地址
        /// </summary>
        /// <param name="ip">ip</param>
        /// <param name="port">port</param>
        void SetInboundAddr(string ip, int port);

        /// <summary>
        /// 设置自定义inbound名
        /// </summary>
        /// <param name="type">inbound名</param>
        void SetInboundName(string name);

        /// <summary>
        /// 设置服务器上次修改日期
        /// </summary>
        /// <param name="utcTicks">修改日期</param>
        void SetLastModifiedUtcTicks(long utcTicks);

        /// <summary>
        /// 设置服务器标记
        /// </summary>
        /// <param name="mark">标记</param>
        void SetMark(string mark);

        /// <summary>
        /// 设置服务器备注
        /// </summary>
        /// <param name="remark">备注</param>
        void SetRemark(string remark);

        /// <summary>
        /// 设置服务器延迟测试结果
        /// </summary>
        /// <param name="latency">延迟</param>
        void SetSpeedTestResult(long latency);

        /// <summary>
        /// 设置服务器延迟标签内容
        /// </summary>
        /// <returns>延迟标签内容</returns>
        void SetStatus(string content);

        /// <summary>
        /// 设置自定义标签1的内容
        /// </summary>
        /// <param name="tag">标签内容</param>
        void SetTag1(string tag);

        /// <summary>
        /// 设置自定义标签2的内容
        /// </summary>
        /// <param name="tag">标签内容</param>
        void SetTag2(string tag);

        /// <summary>
        /// 设置自定义标签3的内容
        /// </summary>
        /// <param name="tag">标签内容</param>
        void SetTag3(string tag);

        /// <summary>
        /// 设置服务器自启动选项
        /// </summary>
        /// <param name="isAutoRun">是否自启动</param>
        void SetIsAutoRun(bool isAutoRun);

        void SetIsAcceptInjection(bool isEnabled);

        /// <summary>
        /// 设置服务器不追踪选项
        /// </summary>
        /// <param name="isUntrack">是否不追踪</param>
        void SetIsUntrack(bool isUntrack);

        /// <summary>
        /// 添加流量统计数据
        /// </summary>
        /// <param name="sample">流量统计数据</param>
        void AddStatSample(Models.Datas.StatsSample sample);
    }
}
