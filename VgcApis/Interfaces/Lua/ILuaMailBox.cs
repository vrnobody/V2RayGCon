namespace VgcApis.Interfaces.Lua
{
    /// <summary>
    /// ILuaMailBox定义一个MailBox。它主要用在多个脚本之间收发信息。
    /// 首先通过local mailBox = Sys:CreateMailBox(name)申请一个名为name的邮箱。
    /// 然后可以调用mailbox:Send(addr, title)向名为addr的邮箱发送一封主题为title的邮件。
    /// 接收方可以调用mailbox:Check()检查邮箱内是否有邮件。也可以调用mailbox:Wait()等待邮件到来。
    /// 邮件的组成部分请看ILuaMail.
    /// </summary>
    public interface ILuaMailBox
    {
        bool Add(Models.Datas.LuaMail mail);

        bool TryAdd(VgcApis.Models.Datas.LuaMail mail);

        /// <summary>
        /// 邮箱内未收取的邮件总数
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// 丢弃所有剩下的邮件
        /// </summary>
        /// <returns></returns>
        bool Clear();

        /// <summary>
        /// 邮箱是否已经关闭
        /// </summary>
        /// <returns>
        /// true 邮箱已关闭<br/>
        /// false 邮箱正常
        /// </returns>
        bool IsCompleted();

        /// <summary>
        /// 邮箱是否拒收邮件
        /// </summary>
        /// <returns>
        /// true 邮箱拒收邮件<br/>
        /// false 邮箱正常
        /// </returns>
        bool IsAddingCompleted();

        /// <summary>
        /// 关闭邮件（拒收邮件）
        /// </summary>
        void Close();

        /// <summary>
        /// 轮询邮箱内的邮件
        /// </summary>
        /// <returns>
        /// LuaMail 有邮件时返回一封最老的邮件<br/>
        /// nil 没邮件或邮箱关闭时返回null
        /// </returns>
        Models.Datas.LuaMail Check();

        /// <summary>
        /// 阻塞等待邮件
        /// </summary>
        /// <returns>
        /// LuaMail 有邮件时返回一封最老的邮件<br/>
        /// nil 没邮件或邮箱关闭时返回null
        /// </returns>
        Models.Datas.LuaMail Wait();

        /// <summary>
        /// 阻塞等待邮件指定时间
        /// </summary>
        /// <returns>
        /// LuaMail 有邮件时返回一封最老的邮件<br/>
        /// nil 没邮件或邮箱关闭或超时后返回null
        /// </returns>
        Models.Datas.LuaMail Wait(int ms);

        /// <summary>
        /// 获取当前邮箱名字
        /// </summary>
        /// <returns></returns>
        string GetAddress();

        /// <summary>
        /// 向mail回复只有标题的邮件
        /// </summary>
        /// <param name="mail">来源邮件</param>
        /// <param name="title">回复邮件的标题</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool Reply(ILuaMail mail, string title);

        /// <summary>
        /// 向mail回复带标题和内容的邮件
        /// </summary>
        /// <param name="mail">来源邮件</param>
        /// <param name="title">回复邮件的标题</param>
        /// <param name="content">回复邮件的内容</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool Reply(ILuaMail mail, string title, string content);


        // lua识别参数类型的方式过于灵活
        // 如果用重载写法会导致错误的调用 Reply(ILuaMail mail, STRING title)
        /// <summary>
        /// 向mail回复带只有代码的邮件
        /// </summary>
        /// <param name="mail">来源邮件</param>
        /// <param name="code">代码（浮点数值）</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool ReplyCode(ILuaMail mail, double code);

        /// <summary>
        /// 向mail回复带有代码和内容的邮件
        /// </summary>
        /// <param name="mail">来源邮件</param>
        /// <param name="code">代码（浮点数值）</param>
        /// <param name="content">内容（字符串）</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool ReplyCode(ILuaMail mail, double code, string content);

        /// <summary>
        /// 向mail回复只有状态的邮件
        /// </summary>
        /// <param name="mail">来源邮件</param>
        /// <param name="state">状态（布尔值）</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool ReplyState(ILuaMail mail, bool state);

        /// <summary>
        /// 向mail回复带有状态和内容的邮件
        /// </summary>
        /// <param name="mail">来源邮件</param>
        /// <param name="state">状态（布尔值）</param>
        /// <param name="content">内容（字符串）</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool ReplyState(ILuaMail mail, bool state, string content);

        /// <summary>
        /// 向mail回复一封完整的邮件
        /// </summary>
        /// <param name="mail">来源邮件</param>
        /// <param name="code">代码（浮点数值）</param>
        /// <param name="title">标题（字符串）</param>
        /// <param name="state">状态（布尔值）</param>
        /// <param name="content">内容（实符串）</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool Reply(ILuaMail mail, double code, string title, bool state, string content);

        /// <summary>
        /// 向名为address的邮箱发送只有标题的邮件
        /// </summary>
        /// <param name="address">目标邮箱名</param>
        /// <param name="title">标题（字符串）</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool Send(string address, string title);

        /// <summary>
        /// 向名为address的邮箱发送一封带标题和内容的邮件
        /// </summary>
        /// <param name="address">目标邮箱名</param>
        /// <param name="title">标题（字符串）</param>
        /// <param name="content">内容（实符串）</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool Send(string address, string title, string content);

        /// <summary>
        /// 向名为address的邮箱发送一封只有代码的邮件
        /// </summary>
        /// <param name="address">目标邮箱名</param>
        /// <param name="code">代码（浮点数值）</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool SendCode(string address, double code);

        /// <summary>
        /// 向名为address的邮箱发送一封带有代码和内容的邮件
        /// </summary>
        /// <param name="address">目标邮箱名</param>
        /// <param name="code">代码（浮点数值）</param>
        /// <param name="content">内容（实符串）</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool SendCode(string address, double code, string content);

        /// <summary>
        /// 向名为address的邮箱发送一封只有状态的邮件
        /// </summary>
        /// <param name="address">目标邮箱名</param>
        /// <param name="state">状态（布尔值）</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool SendState(string address, bool state);

        /// <summary>
        /// 向名为address的邮箱发送一封带状态和内容的邮件
        /// </summary>
        /// <param name="address">目标邮箱名</param>
        /// <param name="state">状态（布尔值）</param>
        /// <param name="content">内容（实符串）</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool SendState(string address, bool state, string content);

        /// <summary>
        /// 向名为address的邮箱发送一封完整的邮件
        /// </summary>
        /// <param name="address">目标邮箱名</param>
        /// <param name="code">代码（浮点数值）</param>
        /// <param name="title">标题（字符串）</param>
        /// <param name="state">状态（布尔值）</param>
        /// <param name="content">内容（实符串）</param>
        /// <returns>
        /// true 回复成功<br/>
        /// false 遇到对方邮箱已关闭等各种问题
        /// </returns>
        bool Send(string address, double code, string title, bool state, string content);

        bool SendAndWait(string address, double code, string title, bool state, string content);

        // 跨插件发送attachment时会显示成userdata，猜测是lua实现方式不同导致

        bool Send(string address, double code, string title, bool state, string content, string header, object attachment);

        bool SendAndWait(string address, double code, string title, bool state, string content, string header, object attachment);
    }
}
