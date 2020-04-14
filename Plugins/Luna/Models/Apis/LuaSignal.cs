namespace Luna.Models.Apis
{
    public class LuaSignal : VgcApis.Interfaces.Lua.ILuaSignal
    {
        bool signalStop;

        public LuaSignal()
        {
            ResetAllSignals();
        }

        #region interface things
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
