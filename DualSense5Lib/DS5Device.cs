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
 * @file DS5Device.cs
 * @author Martin Mayr
 * @date 05.06.2022
 * @brief Type definitions for the Device Enumeration.
 * 
 * DISCLAIMER: This file is a C# port of
 *  https://github.com/Ohjurot/DualSense-Windows,
 *  which provides a C++ API for the DualSense.
 */

using Microsoft.Win32.SafeHandles;

namespace DualSense5Library
{
    #region Public Types
    /// <summary>
    /// Enum for the different types of connections.
    /// </summary>
    public enum DeviceConnection
    {
        /// <summary>
        ///  Controller is connected via USB.
        /// </summary>
        USB = 0,

        /// <summary>
        /// Controller is connected via Bluetooth.
        /// </summary>
        Bluetooth = 1
    }

    /// <summary>
    /// Capsules the path and the device connection.
    /// </summary>
    public struct DeviceEnumInfo
    {
        /// <summary>
        /// Path to the discovered device.
        /// </summary>
        public string Path;

        /// <summary>
        /// Connection type of the discovered device.
        /// </summary>
        public DeviceConnection Connection;
    }
    #endregion

    /// <summary>
    /// Class which contains the necessary information to connect to a DualSense5.
    /// </summary>
    public class DeviceContext
    {
        #region Constructor
        public DeviceContext()
        {
            DevicePath = "";
            DeviceHandle = null;
            DeviceConnection = new DeviceConnection();
            Connected = false;
            HidBuffer = null;
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Path to the device.
        /// </summary>
        public string DevicePath;

        /// <summary>
        /// Handle of the device.
        /// </summary>
        public SafeFileHandle DeviceHandle;
        
        /// <summary>
        /// Connection of the device.
        /// </summary>
        public DeviceConnection DeviceConnection;

        /// <summary>
        /// Current state of the connection.
        /// </summary>
        public bool Connected;
        
        /// <summary>
        /// HID Input Buffer.
        /// </summary>
        public byte[] HidBuffer;
        #endregion
    }

}
