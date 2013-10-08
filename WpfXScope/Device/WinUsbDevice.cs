using System;
using Microsoft.Win32.SafeHandles;
using WpfXScope.Device.API;

namespace WpfXScope.Device
{
    /// <summary>
    ///  Routines for the WinUsb driver supported by Windows Vista and Windows XP.
    ///  </summary>
    sealed internal class WinUsbDevice
    {
        internal struct DevInfo
        {
            internal SafeFileHandle DeviceHandle;
            internal IntPtr WinUsbHandle;
            internal Byte BulkInPipe;
            internal Byte BulkOutPipe;
        }

        internal bool Disconnected;

        internal DevInfo DeviceInfo;

        ///  <summary>
        ///  Closes the device handle obtained with CreateFile and frees resources.
        ///  </summary>
        internal void CloseDeviceHandle()
        {
            if (!Disconnected)
            {
                WinUsbDeviceApi.WinUsb_AbortPipe(DeviceInfo.WinUsbHandle, DeviceInfo.BulkInPipe);
                WinUsbDeviceApi.WinUsb_Free(DeviceInfo.WinUsbHandle);
            }

            if (DeviceInfo.DeviceHandle == null) return;
            if (!(DeviceInfo.DeviceHandle.IsInvalid))
            {
                DeviceInfo.DeviceHandle.Close();
            }

        }

        ///  <summary>
        ///  Initiates a Control Read transfer. Data stage is device to host.
        ///  </summary>
        /// 
        /// <param name="dataIn"></param>
        /// <param name="request"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// 
        /// <returns>
        ///  True on success, False on failure.
        ///  </returns>
        internal Boolean DoControlReadTransfer(ref Byte[] dataIn, byte request, UInt16 value, UInt16 index)
        {
            UInt32 bytesReturned = 0;
            WinUsbDeviceApi.WinusbSetupPacket setupPacket;
            setupPacket.RequestType = 0XC0;     //  Vendor-specific request to an interface with device-to-host Data stage.
            setupPacket.Request = request;  //  The request number that identifies the specific request.
            setupPacket.Value = value;    //  Command-specific value to send to the device.
            setupPacket.Index = index;    //  Command-specific value to send to the device.
            setupPacket.Length = Convert.ToUInt16(dataIn.Length); //  Number of bytes in the request's Data stage.
            //  Initiates a control transfer., returns True on success.         
            return WinUsbDeviceApi.WinUsb_ControlTransfer(DeviceInfo.WinUsbHandle, setupPacket, dataIn, Convert.ToUInt16(dataIn.Length), ref bytesReturned, IntPtr.Zero);
        }

        ///  <summary>
        ///  Requests a handle with CreateFile.
        ///  </summary>
        ///  
        ///  <param name="devicePathName"> Returned by SetupDiGetDeviceInterfaceDetail 
        ///  in an SP_DEVICE_INTERFACE_DETAIL_DATA structure. </param>
        ///  
        ///  <returns>
        ///  The handle.
        ///  </returns>
        internal Boolean GetDeviceHandle(String devicePathName)
        {
            // *** API function
            //  summary
            //      Retrieves a handle to a device.
            //  parameters 
            //      Device path name returned by SetupDiGetDeviceInterfaceDetail
            //      Type of access requested (read/write).
            //      FILE_SHARE attributes to allow other processes to access the device while this handle is open.
            //      Security structure. Using Null for this may cause problems under Windows XP.
            //      Creation disposition value. Use OPEN_EXISTING for devices.
            //      Flags and attributes for files. The winsub driver requires FILE_FLAG_OVERLAPPED.
            //      Handle to a template file. Not used.
            //  Returns
            //      A handle or INVALID_HANDLE_VALUE.
            DeviceInfo.DeviceHandle = FileIO.CreateFile(
                devicePathName,
                (FileIO.GenericWrite | FileIO.GenericRead),
                FileIO.FileShareRead | FileIO.FileShareWrite,
                IntPtr.Zero,
                FileIO.OpenExisting,
                FileIO.FileAttributeNormal | FileIO.FileFlagOverlapped,
                0);

            return !(DeviceInfo.DeviceHandle.IsInvalid);
        }

        ///  <summary>
        ///  Initializes a device interface and obtains information about it.
        ///  Calls these winusb API functions:
        ///    WinUsb_Initialize
        ///    WinUsb_QueryInterfaceSettings
        ///    WinUsb_QueryPipe
        ///  </summary>
        /// 
        /// <returns>
        ///  True on success, False on failure.
        ///  </returns>
        internal Boolean InitializeDevice()
        {
            WinUsbDeviceApi.UsbInterfaceDescriptor ifaceDescriptor;
            WinUsbDeviceApi.WinusbPipeInformation pipeInfo;

            ifaceDescriptor.bLength = 0;
            ifaceDescriptor.bDescriptorType = 0;
            ifaceDescriptor.bInterfaceNumber = 0;
            ifaceDescriptor.bAlternateSetting = 0;
            ifaceDescriptor.bNumEndpoints = 0;
            ifaceDescriptor.bInterfaceClass = 0;
            ifaceDescriptor.bInterfaceSubClass = 0;
            ifaceDescriptor.bInterfaceProtocol = 0;
            ifaceDescriptor.iInterface = 0;

            pipeInfo.PipeType = 0;
            pipeInfo.PipeId = 0;
            pipeInfo.MaximumPacketSize = 0;
            pipeInfo.Interval = 0;

            // *** winusb function 
            //  summary
            //      get a handle for communications with a winusb device        '
            //  parameters
            //      Handle returned by CreateFile.
            //      Device handle to be returned.
            //  returns True on success.
            var success = WinUsbDeviceApi.WinUsb_Initialize(DeviceInfo.DeviceHandle, ref DeviceInfo.WinUsbHandle);
            if (success)
            {
                // *** winusb function 
                //  summary
                //      Get a structure with information about the device interface.
                //  parameters
                //      handle returned by WinUsb_Initialize
                //      alternate interface setting number
                //      USB_INTERFACE_DESCRIPTOR structure to be returned.
                //  returns True on success.
                success = WinUsbDeviceApi.WinUsb_QueryInterfaceSettings(DeviceInfo.WinUsbHandle, 0, ref ifaceDescriptor);

                if (success)
                {
                    //  Get the transfer type, endpoint number, and direction for the interface's
                    //  bulk and interrupt endpoints. Set pipe policies.

                    // *** winusb function 
                    //  summary
                    //      returns information about a USB pipe (endpoint address)
                    //  parameters
                    //      Handle returned by WinUsb_Initialize
                    //      Alternate interface setting number
                    //      Number of an endpoint address associated with the interface. 
                    //      (The values count up from zero and are NOT the same as the endpoint address
                    //      in the endpoint descriptor.)
                    //      WINUSB_PIPE_INFORMATION structure to be returned
                    //  returns True on success
                    for (var i = 0; i <= ifaceDescriptor.bNumEndpoints - 1; i++)
                    {
                        WinUsbDeviceApi.WinUsb_QueryPipe
                            (DeviceInfo.WinUsbHandle,
                             0,
                             Convert.ToByte(i),
                             ref pipeInfo);

                        if (((pipeInfo.PipeType ==
                              WinUsbDeviceApi.UsbdPipeType.UsbdPipeTypeBulk) &
                             UsbEndpointDirectionIn(pipeInfo.PipeId)))
                        {
                            DeviceInfo.BulkInPipe = pipeInfo.PipeId;

                            SetPipePolicy
                                (DeviceInfo.BulkInPipe,
                                 Convert.ToUInt32(WinUsbDeviceApi.PolicyType.IgnoreShortPackets),
                                 Convert.ToByte(false));

                            SetPipePolicy
                                (DeviceInfo.BulkInPipe,
                                 Convert.ToUInt32(WinUsbDeviceApi.PolicyType.PipeTransferTimeout),
                                 0); // Pipe does not timeout

                        }
                        else if (((pipeInfo.PipeType ==
                                   WinUsbDeviceApi.UsbdPipeType.UsbdPipeTypeBulk) &
                                  UsbEndpointDirectionOut(pipeInfo.PipeId)))
                        {

                            DeviceInfo.BulkOutPipe = pipeInfo.PipeId;

                            SetPipePolicy
                                (DeviceInfo.BulkOutPipe,
                                 Convert.ToUInt32(WinUsbDeviceApi.PolicyType.IgnoreShortPackets),
                                 Convert.ToByte(false));

                            SetPipePolicy
                                (DeviceInfo.BulkOutPipe,
                                 Convert.ToUInt32(WinUsbDeviceApi.PolicyType.PipeTransferTimeout),
                                 2000);      // 2 second timeout

                        }
                    }
                }
                else return false;
            }
            return success;
        }

        ///  <summary>
        ///  Is the current operating system Windows XP or later?
        ///  The WinUSB driver requires Windows XP or later.
        ///  </summary>
        /// 
        ///  <returns>
        ///  True if Windows XP or later, False if not.
        ///  </returns>
        internal Boolean IsWindowsXpOrLater()
        {
            var myEnvironment = Environment.OSVersion;

            //  Windows XP is version 5.1.
            var versionXp = new Version(5, 1);

            return myEnvironment.Version >= versionXp;
        }

        ///  <summary>
        ///  Attempts to read data from a bulk IN endpoint.
        ///  </summary>
        /// 
        /// <param name="pipeId"></param>
        /// <param name="bytesToRead"> Number of bytes to read. </param>
        /// <param name="buffer"></param>
        /// <param name="bytesRead"> Number of bytes read. </param>
        ///  <param name="success"> Success or failure status. </param>
        ///  
        internal void ReadViaBulkTransfer(Byte pipeId, UInt32 bytesToRead, ref Byte[] buffer, ref UInt32 bytesRead, out Boolean success)
        {
            // ***  winusb function 
            //  summary
            //      Attempts to read data from a device interface.
            //  parameters
            //      Device handle returned by WinUsb_Initialize.
            //      Endpoint address.
            //      Buffer to store the data.
            //      Maximum number of bytes to return.
            //      Number of bytes read.
            //      Null pointer for non-overlapped.
            //  Returns True on success.
            // ***
            success = WinUsbDeviceApi.WinUsb_ReadPipe(DeviceInfo.WinUsbHandle, pipeId, buffer, bytesToRead, ref bytesRead, IntPtr.Zero);
            //if (!(success)) CloseDeviceHandle();
        }

        ///  <summary>
        ///  Attempts to send data via a bulk OUT endpoint.
        ///  </summary>
        ///  
        ///  <param name="buffer"> Buffer containing the bytes to write. </param>
        ///  <param name="bytesToWrite"> Number of bytes to write. </param>
        ///  
        ///  <returns>
        ///  True on success, False on failure.
        ///  </returns>
        internal Boolean SendViaBulkTransfer(ref Byte[] buffer, UInt32 bytesToWrite)
        {
            UInt32 bytesWritten = 0;
            // *** winusb function 
            //  summary
            //      Attempts to write data to a device interface.
            //  parameters
            //      Device handle returned by WinUsb_Initialize.
            //      Endpoint address.
            //      Buffer with data to write.
            //      Number of bytes to write.
            //      Number of bytes written.
            //      IntPtr.Zero for non-overlapped I/O.
            //  Returns True on success.
            var success = WinUsbDeviceApi.WinUsb_WritePipe(
                DeviceInfo.WinUsbHandle,
                DeviceInfo.BulkOutPipe,
                buffer,
                bytesToWrite,
                ref bytesWritten,
                IntPtr.Zero);

            if (!(success)) CloseDeviceHandle();
            return success;
        }

        ///  <summary>
        ///  Sets pipe policy.
        ///  Used when the value parameter is a Byte (all except PIPE_TRANSFER_TIMEOUT).
        ///  </summary>
        /// 
        /// <param name="pipeId"> Pipe to set a policy for. </param>
        /// <param name="policyType"> POLICY_TYPE member. </param>
        /// <param name="value"> Policy value. </param>
        /// 
        /// <returns>
        ///  True on success, False on failure.
        ///  </returns>
        private void SetPipePolicy(byte pipeId, uint policyType, byte value)
        {
            // *** winusb function 
            //  summary
            //      sets a pipe policy 
            //  parameters
            //      handle returned by WinUsb_Initialize
            //      identifies the pipe
            //      POLICY_TYPE member.
            //      length of value in bytes
            //      value to set for the policy.
            //  returns True on success
            WinUsbDeviceApi.WinUsb_SetPipePolicy(
                DeviceInfo.WinUsbHandle,
                pipeId,
                policyType,
                1,
                ref value);
        }

        ///  <summary>
        ///  Sets pipe policy.
        ///  Used when the value parameter is a UInt32 (PIPE_TRANSFER_TIMEOUT only).
        ///  </summary>
        /// 
        /// <param name="pipeId"> Pipe to set a policy for. </param>
        /// <param name="policyType"> POLICY_TYPE member. </param>
        /// <param name="value"> Policy value. </param>
        /// 
        /// <returns>
        ///  True on success, False on failure.
        ///  </returns>
        private void SetPipePolicy(byte pipeId, uint policyType, uint value)
        {
            // *** winusb function 
            //  summary
            //      sets a pipe policy 
            //  parameters
            //      handle returned by WinUsb_Initialize
            //      identifies the pipe
            //      POLICY_TYPE member.
            //      length of value in bytes
            //      value to set for the policy.
            //  returns True on success 
            WinUsbDeviceApi.WinUsb_SetPipePolicy1
                (DeviceInfo.WinUsbHandle,
                 pipeId,
                 policyType,
                 4,
                 ref value);
        }

        ///  <summary>
        ///  Is the endpoint's direction IN (device to host)?
        ///  </summary>
        ///  
        ///  <param name="addr"> The endpoint address. </param>
        ///  <returns>
        ///  True if IN (device to host), False if OUT (host to device)
        ///  </returns> 
        private static Boolean UsbEndpointDirectionIn(Int32 addr)
        {
            return ((addr & 0X80) == 0X80);
        }

        ///  <summary>
        ///  Is the endpoint's direction OUT (host to device)?
        ///  </summary>
        ///  
        ///  <param name="addr"> The endpoint address. </param>
        ///  
        ///  <returns>
        ///  True if OUT (host to device, False if IN (device to host)
        ///  </returns>
        private static Boolean UsbEndpointDirectionOut(Int32 addr)
        {
            return ((addr & 0X80) == 0);
        }
    }

}
