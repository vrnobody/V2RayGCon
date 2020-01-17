namespace VgcApis.BaseClasses
{
    public class LuaSignal : Interfaces.Lua.ILuaSignal
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
