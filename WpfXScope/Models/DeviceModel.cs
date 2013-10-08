using System;
using System.Text;
using WpfXScope.Device;

namespace WpfXScope.Models
{
    public class BulkDataArrivedArgs : EventArgs
    {
        public byte[] Data
        {
            get; set; 
        }
    }

    public static class DeviceModel
    {
        public static event EventHandler<BulkDataArrivedArgs> BulkDataArrived;
        public static event EventHandler DeviceAttached;
        public static event EventHandler DeviceDetached;
 
        private static WinUsbDevice _usbDevice = new WinUsbDevice();
        private static DeviceManagement _deviceManager = new DeviceManagement();

        private static string _devicePathName = string.Empty;

        public static bool USBConnect(ref int version)
        {
            var usbDataIn = new byte[64];

            if (_usbDevice.IsWindowsXpOrLater()) FindMyDevice();
            if (DeviceDetected)
            {
                _usbDevice.DoControlReadTransfer(ref usbDataIn, Convert.ToByte('a'), 0, 0);
                usbDataIn[4] = 0;

                var str = Encoding.UTF8.GetString(usbDataIn, 0, 4);

                str = str.Replace(",", "."); // Different languages use different punctuation
                version = (UInt16)(Convert.ToDecimal(str) * 100);

                // read settings
                var settings = ReadSettings();
                DeviceSettingsManager.FromDeviceData(settings);
                return true;
            }

            return false;
        }

        public static void UpdateFrequency(uint frequency)
        {
            var usbDataIn = new byte[64];
            _usbDevice.DoControlReadTransfer(ref usbDataIn, Convert.ToByte(AWGCommand), (UInt16)(frequency >> 16),
                                             (UInt16)(frequency & 0x0000FFFF));
        }

        public static void WriteByte(byte index, byte data)
        {
            var usbDataIn = new byte[64];

            if (DeviceDetected)
            {
                _usbDevice.DoControlReadTransfer(ref usbDataIn, Convert.ToByte(ByteDataCommand), data, index);
            }
        }

        private static void OnDeviceDetached()
        {
            var handler = DeviceDetached;
            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }

        private static void OnDeviceAttach()
        {
            var handler = DeviceAttached;
            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }

        private static void OnBulkDataArrived(byte[] data)
        {
            var handler = BulkDataArrived;
            if(handler != null)
            {
                handler(null, new BulkDataArrivedArgs { Data = data });
            }
        }

        public static void RegisterForDeviceNotifications(IntPtr handle, ref IntPtr notificationHandle)
        {
            _deviceManager.RegisterForDeviceNotifications(_devicePathName, handle, WinUsbDemoGuid,
                                                          ref notificationHandle);
        }

        private static readonly Guid WinUsbDemoGuid = new Guid("{88BAE032-5A81-49f0-BC3D-A4FF138216D6}");

        private static void FindMyDevice()
        {
            var devicePathName = "";

            if (!(DeviceDetected))
            {
                //  This GUID must match the GUID in the device's INF file.
                //  Convert the device interface GUID String to a GUID object:

                if (_deviceManager.FindDeviceFromGuid(WinUsbDemoGuid, ref devicePathName))
                {
                    var success = _usbDevice.GetDeviceHandle(devicePathName);
                    if (success)
                    {
                        DeviceDetected = true;

                        // Save DevicePathName so OnDeviceChange() knows which name is my device.
                        _devicePathName = devicePathName;
                    }
                    else
                    {
                        // There was a problem in retrieving the information.
                        DeviceDetected = false;
                        _usbDevice.CloseDeviceHandle();
                    }
                }
                if (DeviceDetected)
                {
                    _usbDevice.InitializeDevice();
                }
            }
        }

        private const char ReadSettingsCommand = 'u';
        private const char ByteDataCommand = 'b';
        private const char StartScopeCommand = 'g';
        private const char StopScopeCommand = 'f';
        private const char AWGCommand = 'c';
        private const char AutoScopeCommand = 'i';

        public static byte[] ReadSettings()
        {
            var usbDataIn = new byte[64];

            // Request settings from Xprotolab
            if (DeviceDetected)
            {
                _usbDevice.DoControlReadTransfer(ref usbDataIn, Convert.ToByte(ReadSettingsCommand), 0, 14);
            }
            else return new byte[] { };

            return usbDataIn;
        }

        public static bool DeviceDetected { get; private set; }

        private delegate void ReadFromDeviceDelegate
            (Byte pipeId,
            UInt32 bufferLength,
            ref Byte[] buffer,
            ref UInt32 lengthTransferred,
            out Boolean success);

        private const int BulkBufferSize = (256*3) + 2;
        ///  <summary>
        ///  Initiates a read operation from a bulk IN endpoint.
        ///  To enable reading without blocking the main thread, uses an asynchronous delegate.
        ///  </summary>
        ///  
        ///  <remarks>
        ///  To enable reading more than 64 bytes (with device firmware support), increase bytesToRead.
        ///  </remarks> 
        public static void ReadBulkData()
        {
            if (DeviceDetected)
            {
                const uint bytesToRead = BulkBufferSize; // CH1+CH2+CHD+frame number

                //  Define a delegate for the ReadViaBulkTransfer method of WinUsbDevice.
                ReadFromDeviceDelegate myReadFromDeviceDelegate =
                    _usbDevice.ReadViaBulkTransfer;

                //  The BeginInvoke method calls MyWinUsbDevice.ReadViaBulkTransfer to attempt 
                //  to read data. The method has the same parameters as ReadViaBulkTransfer,
                //  plus two additional parameters:
                //  GetReceivedBulkData is the callback routine that executes when 
                //  ReadViaBulkTransfer returns.
                //  MyReadFromDeviceDelegate is the asynchronous delegate object.

                var readState = new AsyncReadState {Success = false, BytesRead = 0, Data = new byte[BulkBufferSize]};


                myReadFromDeviceDelegate.BeginInvoke(
                    Convert.ToByte(_usbDevice.DeviceInfo.BulkInPipe),
                    bytesToRead,
                    ref readState.Data,
                    ref readState.BytesRead,
                    out readState.Success,
                    GetReceivedBulkData,
                    readState
                    );
            }
        }

        private static void GetReceivedBulkData(IAsyncResult ar)
        {
            var state = (AsyncReadState)ar.AsyncState;
            OnBulkDataArrived(state.Data);
        }

        public struct AsyncReadState
        {
            public byte[] Data;
            public uint BytesRead;
            public bool Success;
        }

        internal static void ForceDisconnect()
        {
            _usbDevice.Disconnected = true;
            DeviceDetected = false;

            _usbDevice = new WinUsbDevice();
            _deviceManager = new DeviceManagement();

            _devicePathName = string.Empty;
        }

        public static void ForceTrigger()
        {
            if (!DeviceDetected) return;

            var usbDataIn = new byte[64]; 
            _usbDevice.DoControlReadTransfer(ref usbDataIn, Convert.ToByte('h'), 0, 0);
        }

        public static void ScopeActive(bool start)
        {
            var usbDataIn = new byte[64];
            if (start)
                _usbDevice.DoControlReadTransfer(ref usbDataIn, Convert.ToByte(StartScopeCommand), 0, 0);
            else
                _usbDevice.DoControlReadTransfer(ref usbDataIn, Convert.ToByte(StopScopeCommand), 0, 0);
        }

        public static void ProcessDeviceMessage(int wparam, int lparam)
        {
            if(wparam == DeviceManagement.DbtDeviceremovecomplete)
            {
                // we need to force disconnect
                OnDeviceDetached();
            }
            else if(wparam == DeviceManagement.DbtDevicearrival)
            {
                OnDeviceAttach();
            }
        }

        public static void AutoSettings()
        {
            var usbDataIn = new byte[64];
            _usbDevice.DoControlReadTransfer(ref usbDataIn, Convert.ToByte(AutoScopeCommand), 0, 0);
        }
    }
}
