using NeoLuna.Services;

namespace NeoLuna.Models.Apis
{
    internal class LuaSignal : Interfaces.ILuaSignal
    {
        bool signalStop;
        private readonly Settings settings;

        public LuaSignal(Settings settings)
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
