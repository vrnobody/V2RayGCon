namespace VgcApis.Interfaces.Lua
{
    public interface ILuaSignal
    {

        /// <summary>
        /// 检测用户是否按下了"■"停止按钮
        /// </summary>
        /// <returns>true 按下了停止按钮<br/>false 没按 </returns>
        bool Stop();

        /// <summary>
        /// 检测用户是否锁定屏幕
        /// </summary>
        /// <returns>true 屏幕锁定中<br/>false 屏幕未锁定</returns>
        bool ScreenLocked();
    }
}
