using System;

namespace VgcApis.Interfaces.CoreCtrlComponents
{
    public interface ICoreCtrl
    {
        bool IsCoreRunning();

        void RunSpeedTest();

        void StopCore();
        void StopCoreQuiet(); // for cleanup do not use this in lua script
        void StopCoreThen();
        void StopCoreThen(Action next);

        void RestartCore();
        void RestartCoreThen();
        void RestartCoreThen(Action next);

        Models.Datas.StatsSample TakeStatisticsSample();
    }
}
