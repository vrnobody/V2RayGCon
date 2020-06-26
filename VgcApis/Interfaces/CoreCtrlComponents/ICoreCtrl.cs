using System;

namespace VgcApis.Interfaces.CoreCtrlComponents
{
    public interface ICoreCtrl
    {
        bool IsCoreRunning();

        void RunSpeedTest();

        void StopCore();

        void StopCoreThen();
        void StopCoreThen(Action next);

        void RestartCore();
        void RestartCoreThen();
        void RestartCoreThen(Action next);

        // Models.Datas.StatsSample TakeStatisticsSample();
    }
}
