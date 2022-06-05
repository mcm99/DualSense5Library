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
 * @file DS5ReturnValues.cs
 * @author Martin Mayr
 * @date 05.06.2022
 * @brief Definition of API Return Values.
 * 
 * DISCLAIMER: This file is a C# port of
 *  https://github.com/Ohjurot/DualSense-Windows,
 *  which provides a C++ API for the DualSense.
 */

namespace DualSense5Library
{
    #region Types
    /// <summary>
    /// Return values of the API functions.
    /// </summary>
    public enum DS5ReturnValue
    {
        OK = 0,
        ErrorUnknown = 1,
        ErrorInsufficientBuffer = 2,
        ErrorExternalWinapi = 3,
        ErrorStackOverflow = 4,
        ErrorInvalidArgs = 5,
        ErrorCurrentlyNotSupported = 6,
        ErrorDeviceRemoved = 7,
        ErrorBluetoothCommunication = 8,
        ErrorNoDeviceFound = 9
    }
    #endregion
}
