using System;
using System.Threading.Tasks.Dataflow;
using CoreBluetooth;
using Foundation;
using Microsoft.Maui.Controls;
using static CoreFoundation.DispatchSource;

namespace CoreBLE
{
    public class PeripheralDelegate : CBPeripheralDelegate
    {
        public static CBUUID SERVICE_UUID = CBUUID.FromString("a1e8abba-6e54-fff3-7744-b3881c4b623d");
        public static CBUUID CHARACTERISTIC_UUID = CBUUID.FromString("a1e8f00f-6e54-fff3-7744-b3881c4b623d");

        CBCharacteristic readCharacteristic;
        CBPeripheral peripheralDevice;
        PeripheralDelegate peripheralDelegate;
        byte[] cmd;
        
        int respRecordCount = 0;

        public PeripheralDelegate(CBPeripheral peripheral, byte[] command) : base()
        {
            peripheralDevice = peripheral;
            cmd = command;
        }

        public override void DiscoveredService(CBPeripheral peripheral, NSError error)
        {
            Console.WriteLine("Discovered services");

            foreach (CBService service in peripheral.Services)
            {
                Console.WriteLine("Discovered Service: " + service.UUID);
                if (service.UUID.Equals(SERVICE_UUID))
                {
                    Console.WriteLine("Discovered Read Service");
                    peripheral.DiscoverCharacteristics(service);
                }
            }
        }

        public override void DiscoveredCharacteristics(CBPeripheral peripheral, CBService service, NSError error)
        {
            Console.WriteLine("Discovered characteristics");
            foreach (CBCharacteristic characteristic in service.Characteristics)
            {
                Console.WriteLine("Discovered characteristic: " + characteristic.UUID);
                if (characteristic.UUID.Equals(CHARACTERISTIC_UUID))
                {
                    Console.WriteLine("Discovered Read characteristic, fetching records");
                    readCharacteristic = characteristic;
                    NSData fetchRecordCommand = NSData.FromArray(cmd);
                    peripheralDevice.SetNotifyValue(true, readCharacteristic);
                    peripheralDevice.WriteValue(fetchRecordCommand, readCharacteristic, CBCharacteristicWriteType.WithoutResponse);

                }
            }
        }

        public override async void UpdatedCharacterteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            respRecordCount++;
            if (respRecordCount % 10 == 0)
            {
                Console.WriteLine("Fetched " + respRecordCount + " records");
            }
        }

        public void Disconnect()
        {
            peripheralDevice.SetNotifyValue(false, readCharacteristic);
        }
    }
}

