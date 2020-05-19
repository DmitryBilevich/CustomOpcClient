using System;
using System.Collections.Generic;
using System.Threading;
using CustomOpcClient.Domain;
using CustomOpcClient.Domain.Model;
using Opc.Da;

namespace OpcDAClient
{
    public class MotorService : IMotorService
    {
        private const int On = 1;
        private const int Off = 0;

        private Server _server;
        private Subscription _groupRead, _groupWrite;
        private SubscriptionState _groupStateRead, _groupStateWrite;

        public Action<Motor> MotorChangedCallBack;

        public void Connect()
        {
            if (_server != null && _server.IsConnected)
                return;

            _server = new Server(new OpcCom.Factory(), new Opc.URL("opcda://localhost/RSLinx OPC Server"));
            _server.Connect();
            _groupStateRead = new SubscriptionState
            {
                Name = "myReadGroup",
                UpdateRate = 200
            };
            _groupRead = (Subscription)_server.CreateSubscription(_groupStateRead);
            _groupRead.DataChanged += GroupReadDataChanged;

            _groupStateWrite = new SubscriptionState
            {
                Name = "myWriteGroup",
                Active = false
            };
            _groupWrite = (Subscription)_server.CreateSubscription(_groupStateWrite);
        }

        public void ReadMotorData(Action<Motor> motorChangedCallBack)
        {
            MotorChangedCallBack = motorChangedCallBack;

            _groupStateRead.Active = true;
            _groupRead.AddItems(new[] {
                new Item { ItemName = "[MYPLC]N7:0" },
                new Item { ItemName = "[MYPLC]O:0/0" },
                new Item { ItemName = "[MYPLC]B3:0/3" }
            });
        }

        public void StartMotor()
        {
            WriteStartOrStop("[MYPLC]B3:0/0");
        }

        public void StopMotor()
        {
            WriteStartOrStop("[MYPLC]B3:0/1");
        }

        public void AutomationManualSwitch(int value)
        {
            WriteData("[MYPLC]B3:0/3", value);
        }

        public void JogMotor(int value)
        {
            WriteData("[MYPLC]B3:0/4", value);
        }

        private void WriteStartOrStop(string itemName)
        {
            WriteData(itemName, On);
            Thread.Sleep(200);
            WriteData(itemName, Off);
        }

        private void WriteData(string itemName, int value)
        {
            _groupWrite.RemoveItems(_groupWrite.Items);
            var writeList = new List<Item>();
            var valueList = new List<ItemValue>();

            var itemToWrite = new Item
            {
                ItemName = itemName
            };
            var itemValue = new ItemValue(itemToWrite)
            {
                Value = value
            };

            writeList.Add(itemToWrite);
            valueList.Add(itemValue);
            //IMPORTANT:
            //#1: assign the item to the group so the items gets a ServerHandle
            _groupWrite.AddItems(writeList.ToArray());
            // #2: assign the server handle to the ItemValue
            for (int i = 0; i < valueList.Count; i++)
                valueList[i].ServerHandle = _groupWrite.Items[i].ServerHandle;
            // #3: write
            _groupWrite.Write(valueList.ToArray());
        }

        private void GroupReadDataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            var motor = new Motor();
            foreach (var itemValue in values)
            {
                switch (itemValue.ItemName)
                {
                    case "[MYPLC]N7:0":
                        motor.Speed = Convert.ToInt32(itemValue.Value);
                        break;

                    case "[MYPLC]O:0/0":
                        motor.IsActive = Convert.ToBoolean(itemValue.Value);
                        break;

                    case "[MYPLC]B3:0/3":
                        motor.IsAutoMode = Convert.ToBoolean(itemValue.Value);
                        break;
                }
            }

            MotorChangedCallBack(motor);
        }
    }
}
