using Luna.Services;

namespace Luna.Models.Apis
{
    internal class LuaSignal : VgcApis.Interfaces.Lua.ILuaSignal
    {
        bool signalStop;
        private readonly Settings settings;

        public LuaSignal(Services.Settings settings)
        {
            ResetAllSignals();
            this.settings = settings;
        }

        #region interface things
        public bool ScreenLocked() => settings.isScreenLocked;

        public bool Stop()
        {
            return signalStop;
        }
        #endregion

        #region public methods
        public void SetStopSignal(bool isStop)
        {
            signalStop = isStop;
        }

        public void ResetAllSignals()
        {
            signalStop = false;
        }
        #endregion
    }
}
