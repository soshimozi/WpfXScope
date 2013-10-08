using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace WpfXScope.Device.API
{
    /// <summary/>
    ///  These declarations are translated from the C declarations in various files
    ///  in the Windows DDK. The files are:
    ///  
    ///  winddk\6001\inc\api\usb.h
    ///  winddk\6001\inc\api\usb100.h
    ///  winddk\6001\inc\api\winusbio.h
    ///  
    ///  (your home directory and release number may vary)
    /// <summary/>

    sealed internal class WinUsbDeviceApi
    {
        internal const UInt32 DeviceSpeed = 1;
        internal const Byte UsbEndpointDirectionMask = 0X80;

        internal enum PolicyType
        {
            ShortPacketTerminate = 1,
            AutoClearStall,
            PipeTransferTimeout,
            IgnoreShortPackets,
            AllowPartialReads,
            AutoFlush,
            RawIo,
        }

        internal enum UsbdPipeType
        {
            UsbdPipeTypeControl,
            UsbdPipeTypeIsochronous,
            UsbdPipeTypeBulk,
            UsbdPipeTypeInterrupt,
        }

        internal enum UsbDeviceSpeed
        {
            UsbLowSpeed = 1,
            UsbFullSpeed,
            UsbHighSpeed,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct UsbConfigurationDescriptor
        {
            internal Byte bLength;
            internal Byte bDescriptorType;
            internal ushort wTotalLength;
            internal Byte bNumInterfaces;
            internal Byte bConfigurationValue;
            internal Byte iConfiguration;
            internal Byte bmAttributes;
            internal Byte MaxPower;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct UsbInterfaceDescriptor
        {
            internal Byte bLength;
            internal Byte bDescriptorType;
            internal Byte bInterfaceNumber;
            internal Byte bAlternateSetting;
            internal Byte bNumEndpoints;
            internal Byte bInterfaceClass;
            internal Byte bInterfaceSubClass;
            internal Byte bInterfaceProtocol;
            internal Byte iInterface;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WinusbPipeInformation
        {
            internal UsbdPipeType PipeType;
            internal Byte PipeId;
            internal ushort MaximumPacketSize;
            internal Byte Interval;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct WinusbSetupPacket
        {
            internal Byte RequestType;
            internal Byte Request;
            internal ushort Value;
            internal ushort Index;
            internal ushort Length;
        }

        [DllImport("winusb.dll", SetLastError = true)]
        internal static extern Boolean WinUsb_ControlTransfer(IntPtr interfaceHandle, WinusbSetupPacket setupPacket, Byte[] buffer, UInt32 bufferLength, ref UInt32 lengthTransferred, IntPtr overlapped);

        [DllImport("winusb.dll", SetLastError = true)]
        internal static extern Boolean WinUsb_Free(IntPtr interfaceHandle);

        [DllImport("winusb.dll", SetLastError = true)]
        internal static extern Boolean WinUsb_AbortPipe(IntPtr interfaceHandle, Byte pipeIndex);

        [DllImport("winusb.dll", SetLastError = true)]
        internal static extern Boolean WinUsb_Initialize(SafeFileHandle deviceHandle, ref IntPtr interfaceHandle);

        //  Use this declaration to retrieve DEVICE_SPEED (the only currently defined InformationType).
        [DllImport("winusb.dll", SetLastError = true)]
        internal static extern Boolean WinUsb_QueryDeviceInformation(IntPtr interfaceHandle, UInt32 informationType, ref UInt32 bufferLength, ref Byte buffer);

        [DllImport("winusb.dll", SetLastError = true)]
        internal static extern Boolean WinUsb_QueryInterfaceSettings(IntPtr interfaceHandle, Byte alternateInterfaceNumber, ref UsbInterfaceDescriptor usbAltInterfaceDescriptor);

        [DllImport("winusb.dll", SetLastError = true)]
        internal static extern Boolean WinUsb_QueryPipe(IntPtr interfaceHandle, Byte alternateInterfaceNumber, Byte pipeIndex, ref WinusbPipeInformation pipeInformation);

        [DllImport("winusb.dll", SetLastError = true)]
        internal static extern Boolean WinUsb_ReadPipe(IntPtr interfaceHandle, Byte pipeId, Byte[] buffer, UInt32 bufferLength, ref UInt32 lengthTransferred, IntPtr overlapped);

        //  Two declarations for WinUsb_SetPipePolicy. 
        //  Use this one when the returned Value is a Byte (all except PIPE_TRANSFER_TIMEOUT):
        [DllImport("winusb.dll", SetLastError = true)]
        internal static extern Boolean WinUsb_SetPipePolicy(IntPtr interfaceHandle, Byte pipeId, UInt32 policyType, UInt32 valueLength, ref Byte value);

        //  Use this alias when the returned Value is a UInt32 (PIPE_TRANSFER_TIMEOUT only):
        [DllImport("winusb.dll", SetLastError = true, EntryPoint = "WinUsb_SetPipePolicy")]
        internal static extern Boolean WinUsb_SetPipePolicy1(IntPtr interfaceHandle, Byte pipeId, UInt32 policyType, UInt32 valueLength, ref UInt32 value);

        [DllImport("winusb.dll", SetLastError = true)]
        internal static extern Boolean WinUsb_WritePipe(IntPtr interfaceHandle, Byte pipeId, Byte[] buffer, UInt32 bufferLength, ref UInt32 lengthTransferred, IntPtr overlapped);
    }
}
