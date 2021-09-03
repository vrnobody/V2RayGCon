namespace VgcApis.Interfaces.Lua
{
    /// <summary>
    /// ILuaMail定义了一个邮件，
    /// 它由content（字符串），title（字符串），state（布尔值），address（来源地址，自动填入)，code（浮点数值）组成。
    /// 这样拆分主要是为了方便传输不同类型的数据时不用进行类型转换。
    /// 邮件用法见ILuaMailBox.
    /// </summary>
    public interface ILuaMail
    {
        /// <summary>
        /// 获取邮件的状态值
        /// </summary>
        /// <returns></returns>
        bool GetState();

        /// <summary>
        /// 获取邮件的来源地址
        /// </summary>
        /// <returns></returns>
        string GetAddress();

        /// <summary>
        /// 获取邮件的标题
        /// </summary>
        /// <returns></returns>
        string GetTitle();

        /// <summary>
        /// 获取邮件的内容
        /// </summary>
        /// <returns></returns>
        string GetContent();

        /// <summary>
        /// 获取邮件的代码
        /// </summary>
        /// <returns></returns>
        double GetCode();
    }
}
