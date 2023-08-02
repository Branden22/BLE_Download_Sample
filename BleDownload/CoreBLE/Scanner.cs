using System;
using System.Collections.Generic;
using CoreBluetooth;
using static ObjCRuntime.Dlfcn;
using Foundation;
using CoreFoundation;
using ObjCRuntime;

namespace CoreBLE
{

    public class Scanner
    {
        static CBCentralManager mgr;
        static byte[] cmd;

        public static void StartScan(byte[] command)
        {
            cmd = command;
            BLEManagerDelegate myDel = new BLEManagerDelegate();
            DispatchQueue.Attributes attributes = new DispatchQueue.Attributes();
            attributes.QualityOfService = DispatchQualityOfService.UserInteractive; // Just set to highest priority
            attributes.Concurrent = false;
            DispatchQueue dq = new DispatchQueue("ble_queue", attributes);
            mgr = new CBCentralManager(myDel, dq);
        }

        public class BLEManagerDelegate : CBCentralManagerDelegate
        {

            CBPeripheral currentDevice = null;

            public BLEManagerDelegate() : base() { }

            override public void UpdatedState(CBCentralManager mgr)
            {
                if (mgr.State == CBManagerState.PoweredOn)
                {
                    //Passing in null scans for all peripherals. Peripherals can be targeted by using CBUIIDs
                    CBUUID[] cbuuids = null;
                    mgr.ScanForPeripherals(cbuuids); //Initiates async calls of DiscoveredPeripheral
                }
                else
                {
                    //Invalid state -- Bluetooth powered down, unavailable, etc.
                    System.Console.WriteLine("Bluetooth is not available");
                }
            }

            public override void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
            {
                if (peripheral.Name != null && peripheral.Name.StartsWith("WB") && !peripheral.Equals(currentDevice))
                {
                    // Connect to the peripheral here
                    currentDevice = peripheral;
                    peripheral.Delegate = new CoreBLE.PeripheralDelegate(peripheral, cmd);
                    mgr.ConnectPeripheral(peripheral);
                }
            }

            public override void DisconnectedPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
            {
                Console.WriteLine("Disconnected");
                currentDevice = null;

            }

            public override void ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
            {
                Console.WriteLine("Connected");
                peripheral.DiscoverServices();
            }
        }
    }
}

