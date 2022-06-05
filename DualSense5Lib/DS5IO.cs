// MIT License

// Copyright (c) 2022, Martin Mayr, Sebastian Fragner

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

/**
 * @project DualSense5Lib
 * @file DS5IO.cs
 * @author Martin Mayr
 * @date 05.06.2022
 * @brief Manages communication with the DualSense5 as a HID Device.
 * 
 * DISCLAIMER: This file is a C# port of
 *  https://github.com/Ohjurot/DualSense-Windows,
 *  which provides a C++ API for the DualSense.
 */


using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace DualSense5Library
{
	/// <summary>
	/// Manages communication with the DualSense5 as a HID Device.
	/// </summary>
	internal class DS5IO
	{
        #region Private Members
        private const uint GENERIC_READ = 0x80000000;
		private const uint GENERIC_WRITE = 0x40000000;
		private const uint FILE_SHARE_READ = 0x00000001;
		private const uint FILE_SHARE_WRITE = 0x00000002;
		private const uint CREATE_NEW = 1;
		private const uint CREATE_ALWAYS = 2;
		private const uint OPEN_EXISTING = 3;
        #endregion

        #region DLL Imports
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SetupDiGetClassDevs(           // 1st form using a ClassGUID only, with null Enumerator
		   ref Guid ClassGuid,
		   IntPtr Enumerator,
		   IntPtr hwndParent,
		   int Flags
		);

		[DllImport("setupapi.dll", SetLastError = true)]
		static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

		[DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern Boolean SetupDiEnumDeviceInterfaces(
		   IntPtr hDevInfo,
		   ref SP_DEVINFO_DATA devInfo,
		   ref Guid interfaceClassGuid,
		   UInt32 memberIndex,
		   ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
		);

		[DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern Boolean SetupDiGetDeviceInterfaceDetail(
		   IntPtr hDevInfo,
		   ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
		   ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
		   UInt32 deviceInterfaceDetailDataSize,
		   out UInt32 requiredSize,
		   ref SP_DEVINFO_DATA deviceInfoData
		);


		[DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern Boolean SetupDiGetDeviceInterfaceDetailW(
		   IntPtr hDevInfo,
		   ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
		   IntPtr deviceInterfaceDetailData,
		   UInt32 deviceInterfaceDetailDataSize,
		   ref UInt32 requiredSize,
		   IntPtr deviceInfoData
		);


		[DllImport("setupapi.dll", SetLastError = true)]
		private static extern bool SetupDiDestroyDeviceInfoList
		(
			 IntPtr DeviceInfoSet
		);

		[DllImport("hid.dll", SetLastError = true)]
		private static extern Boolean HidD_GetAttributes(SafeFileHandle hObject, ref HIDD_ATTRIBUTES Attributes);

		[DllImport("hid.dll", SetLastError = true)]
		private static extern bool HidD_GetPreparsedData(
			SafeFileHandle hObject,
			ref IntPtr PreparsedData);

		[DllImport("hid.dll", SetLastError = true)]
		private static extern int HidP_GetCaps(
		  IntPtr pPHIDP_PREPARSED_DATA,                 // IN PHIDP_PREPARSED_DATA  PreparsedData,
		  ref HIDP_CAPS myPHIDP_CAPS);              // OUT PHIDP_CAPS  Capabilities


		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
			uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
			uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		[DllImport("hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool HidD_GetFeature(
		   SafeFileHandle hDevice,
		   IntPtr hReportBuffer,
		   uint ReportBufferLength);

		[DllImport("hid.dll", SetLastError = true)]
		static extern Boolean HidD_FlushQueue(SafeFileHandle HidDeviceObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool ReadFile(SafeFileHandle hFile, byte[] lpBuffer,
			uint nNumberOfBytesToRead, ref uint lpNumberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer,
		   uint nNumberOfBytesToWrite, ref uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

        #endregion

        #region Private Types
        [StructLayout(LayoutKind.Sequential)]
		private struct HIDD_ATTRIBUTES
		{
			public Int32 Size;
			public Int16 VendorID;
			public Int16 ProductID;
			public Int16 VersionNumber;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SP_DEVICE_INTERFACE_DATA
		{
			public uint cbSize;
			public Guid InterfaceClassGuid;
			public uint Flags;
			public IntPtr Reserved;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct SP_DEVICE_INTERFACE_DETAIL_DATA
		{
			public int cbSize;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string DevicePath;
		}


		// HIDP_CAPS
		[StructLayout(LayoutKind.Sequential)]
		private struct HIDP_CAPS
		{
			public System.UInt16 Usage;                 // USHORT
			public System.UInt16 UsagePage;             // USHORT
			public System.UInt16 InputReportByteLength;
			public System.UInt16 OutputReportByteLength;
			public System.UInt16 FeatureReportByteLength;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
			public System.UInt16[] Reserved;                // USHORT  Reserved[17];			
			public System.UInt16 NumberLinkCollectionNodes;
			public System.UInt16 NumberInputButtonCaps;
			public System.UInt16 NumberInputValueCaps;
			public System.UInt16 NumberInputDataIndices;
			public System.UInt16 NumberOutputButtonCaps;
			public System.UInt16 NumberOutputValueCaps;
			public System.UInt16 NumberOutputDataIndices;
			public System.UInt16 NumberFeatureButtonCaps;
			public System.UInt16 NumberFeatureValueCaps;
			public System.UInt16 NumberFeatureDataIndices;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SP_DEVINFO_DATA
		{
			public uint cbSize;
			public Guid ClassGuid;
			public uint DevInst;
			public IntPtr Reserved;
		}
        #endregion

        #region Public Methods
        /// <summary>
        /// Enumerate all DS5 devices connected to the computer.
        /// </summary>
        /// <returns></returns>
        public DS5ReturnValue EnumDevices(ref DeviceEnumInfo[] buffer, int bufferLen, ref uint controllerCount)
		{
			Guid GUID_DEVINTERFACE_HID = new Guid(0x4D1E55B2, 0xF16F, 0x11CF, 0x88, 0xCB, 0x00, 0x11, 0x11, 0x00, 0x00, 0x30);
			const int DIGCF_DEVICEINTERFACE = 0x10;
			const int DIGCF_PRESENT = 0x2;
			IntPtr hidDiHandle = SetupDiGetClassDevs(ref GUID_DEVINTERFACE_HID, IntPtr.Zero, IntPtr.Zero, DIGCF_DEVICEINTERFACE | DIGCF_PRESENT);

			if (hidDiHandle == IntPtr.Zero)
			{
				return DS5ReturnValue.ErrorExternalWinapi;
			}

			uint inputArrIndex = 0;

			SP_DEVINFO_DATA devInfo = new SP_DEVINFO_DATA();
			devInfo.cbSize = (uint)Marshal.SizeOf(devInfo);

			SP_DEVICE_INTERFACE_DETAIL_DATA didd = new SP_DEVICE_INTERFACE_DETAIL_DATA();
			if (IntPtr.Size == 8) // for 64 bit operating systems
				didd.cbSize = 8;
			else
				didd.cbSize = 4 + Marshal.SystemDefaultCharSize; // for 32 bit systems


			/// Enumerate over hid device
			uint devIndex = 0;
			SP_DEVINFO_DATA hidDiInfo = new SP_DEVINFO_DATA();
			hidDiInfo.cbSize = (uint)Marshal.SizeOf(hidDiInfo);
			while (SetupDiEnumDeviceInfo(hidDiHandle, devIndex, ref hidDiInfo))
			{
				/// Enumerate over all hid device interfaces
				uint ifIndex = 0;
				SP_DEVICE_INTERFACE_DATA ifDiInfo = new SP_DEVICE_INTERFACE_DATA();
				ifDiInfo.cbSize = (uint)Marshal.SizeOf(ifDiInfo);
				while (SetupDiEnumDeviceInterfaces(hidDiHandle, ref hidDiInfo, ref GUID_DEVINTERFACE_HID, ifIndex, ref ifDiInfo))
				{
					/// Query device path size.
					uint requiredSize = 0;

					bool ret = SetupDiGetDeviceInterfaceDetail(hidDiHandle, ref ifDiInfo, ref didd, (uint)Marshal.SizeOf(didd), out requiredSize, ref devInfo);
					int error = Marshal.GetLastWin32Error();

					string devicePathh = didd.DevicePath;


					/// Check size.
					if (requiredSize > (260 * sizeof(char)))
					{
						SetupDiDestroyDeviceInfoList(hidDiHandle);
						return DS5ReturnValue.ErrorExternalWinapi;
					}

					/// Allocate memory for path
					SP_DEVICE_INTERFACE_DETAIL_DATA devicePath = new SP_DEVICE_INTERFACE_DETAIL_DATA();
					devicePath.cbSize = 8; // TODO ?

					//SetupDiGetDeviceInterfaceDetail(hidDiHandle, ref ifDiInfo, ref devicePath, requiredSize, ref requiredSize, ref deviceInfoData);


					using (SafeFileHandle deviceHandle = CreateFile(devicePathh, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero))
					{
						error = Marshal.GetLastWin32Error();
						/// check if device is reachable
						short vendorId = 0;
						short productId = 0;


						HIDD_ATTRIBUTES deviceAttributes = new HIDD_ATTRIBUTES();
						if (HidD_GetAttributes(deviceHandle, ref deviceAttributes))
						{
							vendorId = deviceAttributes.VendorID;
							productId = deviceAttributes.ProductID;
						}

						/// Check if ids match
						if (vendorId == 0x054C && productId == 0x0CE6)
						{
							DeviceEnumInfo deviceEnumInfo = new DeviceEnumInfo();

							/// copy path
							deviceEnumInfo.Path = String.Copy(devicePathh);

							/// Get preparsed data
							IntPtr preParsedData = new IntPtr();
							if (HidD_GetPreparsedData(deviceHandle, ref preParsedData))
							{
								HIDP_CAPS deviceCaps = new HIDP_CAPS();
								int hidCapsSuccess = HidP_GetCaps(preParsedData, ref deviceCaps);
								/// check for device connection type
								/// check if controller matches USB specification
								if (deviceCaps.InputReportByteLength == 64)
								{
									deviceEnumInfo.Connection = DeviceConnection.USB;

									/// device found and valid
								}
								/// check if controller matches bluetooth specification
								else if (deviceCaps.InputReportByteLength == 78)
								{
									deviceEnumInfo.Connection = DeviceConnection.Bluetooth;

									/// device found and valid
								}
								if (inputArrIndex < bufferLen)
								{
									buffer[inputArrIndex] = deviceEnumInfo;
									inputArrIndex++;
								}
							}
						}
					}
					ifIndex++;
					Console.WriteLine(buffer[0]);
				}
				devIndex++;
			}
			/// SetupDiDestroyDeviceInfoLIst(hidDiHandle);
			controllerCount = inputArrIndex;
			if (controllerCount > 0)
			{
				return DS5ReturnValue.OK;
			}
			else
            {
				return DS5ReturnValue.ErrorNoDeviceFound;
            }
		}

		/// <summary>
		/// Initializes a DeviceContext from its enum infos.
		/// </summary>
		/// <returns></returns>
		public DS5ReturnValue InitDeviceContext(ref DeviceEnumInfo enumInfo, ref DeviceContext deviceContext)
		{
			if (enumInfo.Path.Length == 0)
			{
				return DS5ReturnValue.ErrorInvalidArgs;
			}

			/// Connect to device.
			SafeFileHandle deviceHandle = CreateFile(enumInfo.Path, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
			if (deviceHandle == null || deviceHandle.IsInvalid)
			{
				return DS5ReturnValue.ErrorDeviceRemoved;
			}

			/// Write the context.
			deviceContext.Connected = true;
			deviceContext.DeviceConnection = enumInfo.Connection;
			deviceContext.DeviceHandle = deviceHandle;
			deviceContext.DevicePath = String.Copy(enumInfo.Path);


			if (deviceContext.DeviceConnection == DeviceConnection.Bluetooth)
			{
				IntPtr fBuffer = Marshal.AllocHGlobal(64);
				Marshal.WriteInt32(fBuffer, 0x05);
				if (!HidD_GetFeature(deviceHandle, fBuffer, 64))
				{
					return DS5ReturnValue.ErrorBluetoothCommunication;
				}
			}

			return DS5ReturnValue.OK;
		}

		/// <summary>
		/// Free the device context.
		/// </summary>
		/// <returns></returns>
		public DS5ReturnValue FreeDeviceContext()
		{
			return DS5ReturnValue.OK;
		}

		/// <summary>
		/// Try to reconnect a removed device.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public DS5ReturnValue ReconnectDevice(DeviceContext context)
		{
			if (context.DevicePath.Length == 0)
            {
				return DS5ReturnValue.ErrorInvalidArgs;
            }

			SafeFileHandle deviceHandle = CreateFile(context.DevicePath, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
			if (deviceHandle == null)
            {
				return DS5ReturnValue.ErrorDeviceRemoved; 
            }

			context.Connected = true;
			context.DeviceHandle = deviceHandle;

			return DS5ReturnValue.OK;
		}

		/// <summary>
		/// Get device input state.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="inputState"></param>
		/// <returns></returns>
		public DS5ReturnValue GetDeviceInputState(ref DeviceContext context, ref DS5InputState inputState)
		{
			if (!context.Connected)
			{
				return DS5ReturnValue.ErrorInvalidArgs;
			}

			if (context.DeviceHandle == null)
            {
				return DS5ReturnValue.ErrorDeviceRemoved;
            }
			/// Get the most recent package
			bool rv = HidD_FlushQueue(context.DeviceHandle);

            if (!rv) { 
				Console.WriteLine("Error in HidD_FlushQueue()");
			}

			/// Get input report length
			uint inputReportLength = 0;
			if (context.DeviceConnection == DeviceConnection.Bluetooth)
			{
				inputReportLength = 78;
				context.HidBuffer = new byte[78];
				context.HidBuffer[0] = 0x31;
			}
			else
			{
				inputReportLength = 64;
				context.HidBuffer = new byte[64];
				context.HidBuffer[0] = 0x01;
			}

			/// Get device input
			uint numberBytesRead = 0;
			if (!ReadFile(context.DeviceHandle, context.HidBuffer, inputReportLength, ref numberBytesRead, IntPtr.Zero))
			{
				context.DeviceHandle.Close();
				context.DeviceHandle = null;
				context.Connected = false;
				return DS5ReturnValue.ErrorDeviceRemoved;
			}

			/// Evaluate the input buffer
			if (context.DeviceConnection == DeviceConnection.Bluetooth)
			{
				int offset = 2;
				DS5Input.EvaluateHidInputBuffer(ref context.HidBuffer, ref inputState, offset);
			}
			else
			{
				int offset = 1;
				DS5Input.EvaluateHidInputBuffer(ref context.HidBuffer, ref inputState, offset);
			}

			return DS5ReturnValue.OK;
		}

		/// <summary>
		/// Set device output state.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="outputState"></param>
		/// <returns></returns>
		public DS5ReturnValue SetDeviceOutputState(DeviceContext context, DS5OutputState outputState)
		{
			if (!context.Connected)
			{
				return DS5ReturnValue.ErrorInvalidArgs;
			}

			ushort outputReportLength = 0;
			if (context.DeviceConnection == DeviceConnection.Bluetooth)
			{
				outputReportLength = 547;
			}
			else
			{
				outputReportLength = 48;
			}

			/// Build output buffer
			if (context.DeviceConnection == DeviceConnection.Bluetooth)
			{
				context.HidBuffer = new byte[outputReportLength];
				context.HidBuffer[0x00] = 0x31;
				context.HidBuffer[0x01] = 0x02;
				DS5Output.CreateHidOutputBuffer(context.HidBuffer, outputState, 2);

				uint crc = DS5CRC32.Compute(ref context.HidBuffer, 74);
				context.HidBuffer[0x4A] = (byte)((crc & 0x000000FF) >> 0);
				context.HidBuffer[0x4B] = (byte)((crc & 0x0000FF00) >> 8);
				context.HidBuffer[0x4C] = (byte)((crc & 0x00FF0000) >> 16);
				context.HidBuffer[0x4D] = (byte)((crc & 0xFF000000) >> 24);
			}
			else
			{
				context.HidBuffer[0x00] = 0x02;
				DS5Output.CreateHidOutputBuffer(context.HidBuffer, outputState, 1);
			}

			uint numberOfBytesWritten = 0;
			if (!WriteFile(context.DeviceHandle, context.HidBuffer, outputReportLength, ref numberOfBytesWritten, IntPtr.Zero))
			{
				context.DeviceHandle.Close();
				context.DeviceHandle = null;
				context.Connected = false;
				return DS5ReturnValue.ErrorDeviceRemoved;
			}

			return DS5ReturnValue.OK;
		}
        #endregion
    }
}