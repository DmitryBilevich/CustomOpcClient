using CustomOpcClient.Domain.Model;
using System;

namespace CustomOpcClient.Domain
{
    public interface IMotorService
    {
        void Connect();

        void ReadMotorData(Action<Motor> MotorChangedCallBack);

        void StartMotor();

        void StopMotor();

        void AutomationManualSwitch(int value);

        void JogMotor(int value);
    }
}
