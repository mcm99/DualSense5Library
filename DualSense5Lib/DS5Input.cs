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
 * @file DS5Input.cs
 * @author Martin Mayr
 * @date 05.06.2022
 * @brief Provides functions for evaluating the hid input buffer as an InputState of DualSense5.
 * 
 * DISCLAIMER: This file is a C# port of
 *  https://github.com/Ohjurot/DualSense-Windows,
 *  which provides a C++ API for the DualSense.
 */

using System;

namespace DualSense5Library
{
    /// <summary>
    /// Provides methods to interpret a HID Buffer as an <see cref="DualSense5Library.InputState"/>.
    /// </summary>
    internal static class DS5Input
    {
        #region Public Methods
        /// <summary>
        /// Interprets hidInBuffer as an InputState of DualSense5.
        /// </summary>
        /// <param name="hidInBuffer">The bytes to interpret</param>
        /// <param name="inputState">The interpreted InputState.</param>
        /// <param name="offset">Differs on Connection Type, therefor given as in parameter.</param>
        public static void EvaluateHidInputBuffer(ref byte[] hidInBuffer, ref DualSense5Library.DS5InputState inputState,  int offset)
        {
            /// Convert sticks to signed range.
            inputState.LeftStick.X = (sbyte)(((short)(hidInBuffer[0 + offset] - 128)));
            inputState.LeftStick.Y = (sbyte)(((short)(hidInBuffer[1 + offset] - 127)) * -1);
            inputState.RightStick.X = (sbyte)(((short)(hidInBuffer[2 + offset] - 128)));
            inputState.RightStick.Y = (sbyte)(((short)(hidInBuffer[3 + offset] - 127)) * -1);

            /// Convert ytihhrt to unsigned range
            inputState.LeftTrigger = hidInBuffer[4 + offset];
            inputState.RightTrigger = hidInBuffer[5 + offset];  

            /// Buttons
            inputState.ButtonsAndDpad = (byte)(hidInBuffer[7 + offset] & 0xF0);
            inputState.ButtonsA = hidInBuffer[8 + offset];
            inputState.ButtonsB = hidInBuffer[9 + offset];
            inputState.LeftStickButton.IsPressed = (inputState.ButtonsA & DS5IStateButtonA.DS5IStateBtnALeftStick) != 0;
            inputState.RightStickButton.IsPressed = (inputState.ButtonsA & DS5IStateButtonA.DS5IStateBtnARightStick) != 0;
            inputState.LeftTriggerButton.IsPressed = (inputState.ButtonsA & DS5IStateButtonA.DS5IStateBtnALeftTrigger) != 0;
            inputState.RightTriggerButton.IsPressed = (inputState.ButtonsA & DS5IStateButtonA.DS5IStateBtnARightTrigger) != 0;
            inputState.LeftBumper.IsPressed = (inputState.ButtonsA & DS5IStateButtonA.DS5IStateBtnALeftBumper) != 0;
            inputState.RightBumper.IsPressed = (inputState.ButtonsA & DS5IStateButtonA.DS5IStateBtnARightBumper) != 0;
            inputState.Square.IsPressed = (inputState.ButtonsAndDpad & DS5IStateDPad.DS5IStateBtxSquare) != 0;
            inputState.Triangle.IsPressed = (inputState.ButtonsAndDpad & DS5IStateDPad.DS5IStateBtxTriangle) != 0;
            inputState.Circle.IsPressed = (inputState.ButtonsAndDpad & DS5IStateDPad.DS5IStateBtxCircle) != 0;
            inputState.Cross.IsPressed = (inputState.ButtonsAndDpad & DS5IStateDPad.DS5IStateBtxCross) != 0;
            inputState.Menu.IsPressed = (inputState.ButtonsA & DS5IStateButtonA.DS5IStateBtnAMenu) != 0;
            inputState.Select.IsPressed = (inputState.ButtonsA & DS5IStateButtonA.DS5IStateBtnASelect) != 0;
            inputState.Touchpad.IsPressed = (inputState.ButtonsB & DS5IStateButtonB.DS5IStateBtnBPadButton) != 0;
            inputState.PlayStationLogo.IsPressed = (inputState.ButtonsB & DS5IStateButtonB.DS5IStateBtnBPlaystationLogo) != 0;
            inputState.MicrophoneButton.IsPressed = (inputState.ButtonsB & DS5IStateButtonB.DS5IStateBtnBMicButton) != 0;

            /// DPad
            switch (hidInBuffer[7 + offset] & 0x0F)
            {
                case 0x0: inputState.ButtonsAndDpad |= DS5IStateDPad.DS5IStateDPadUp; break;
                case 0x4: inputState.ButtonsAndDpad |= DS5IStateDPad.DS5IStateDPadDown; break;
                case 0x6: inputState.ButtonsAndDpad |= DS5IStateDPad.DS5IStateDPadLeft; break;
                case 0x2: inputState.ButtonsAndDpad |= DS5IStateDPad.DS5IStateDPadRight; break;
                case 0x5: inputState.ButtonsAndDpad |= (byte)(DS5IStateDPad.DS5IStateDPadLeft | DS5IStateDPad.DS5IStateDPadDown); break;
                case 0x7: inputState.ButtonsAndDpad |= (byte)(DS5IStateDPad.DS5IStateDPadLeft | DS5IStateDPad.DS5IStateDPadUp); break;
                case 0x1: inputState.ButtonsAndDpad |= (byte)(DS5IStateDPad.DS5IStateDPadRight | DS5IStateDPad.DS5IStateDPadUp); break;
                case 0x3: inputState.ButtonsAndDpad |= (byte)(DS5IStateDPad.DS5IStateDPadRight | DS5IStateDPad.DS5IStateDPadDown); break;
            }
            inputState.DPadLeft.IsPressed = (inputState.ButtonsAndDpad & DS5IStateDPad.DS5IStateDPadLeft) != 0;
            inputState.DPadRight.IsPressed = (inputState.ButtonsAndDpad & DS5IStateDPad.DS5IStateDPadRight) != 0;
            inputState.DPadUp.IsPressed = (inputState.ButtonsAndDpad & DS5IStateDPad.DS5IStateDPadUp) != 0;
            inputState.DPadDown.IsPressed = (inputState.ButtonsAndDpad & DS5IStateDPad.DS5IStateDPadDown) != 0;

            /// TODO timestamp ??? as short
            /// in registers 12 and 13

            /// Accelerometer
            short[] arr = new short[3];
            Buffer.BlockCopy(hidInBuffer, (0x0F + offset), arr, 0, 2 * 3);
            inputState.Accelerometer.X = arr[0];
            inputState.Accelerometer.Y = arr[1];
            inputState.Accelerometer.Z = arr[2];

            /// Gyroscope
            Buffer.BlockCopy(hidInBuffer, (0x15 + offset), arr, 0, 2 * 3);
            inputState.Gyroscope.X = arr[0];
            inputState.Gyroscope.Y = arr[1];
            inputState.Gyroscope.Z = arr[2];

            /// TODO: make member with filtered gyroscope data
            double smoothness_factor = 0.2;
            inputState.Gyroscope_F.X = (short)(inputState.Gyroscope_F.X + ((inputState.Gyroscope.X - inputState.Gyroscope_F.X) * smoothness_factor));
            inputState.Gyroscope_F.Y = (short)(inputState.Gyroscope_F.Y + ((inputState.Gyroscope.Y - inputState.Gyroscope_F.Y) * smoothness_factor));
            inputState.Gyroscope_F.Z = (short)(inputState.Gyroscope_F.Z + ((inputState.Gyroscope.Z - inputState.Gyroscope_F.Z) * smoothness_factor));


            /// TODO values in registers 28 - 31? 

            /// Touch State 1
            uint[] uintarr = new uint[1];
            Buffer.BlockCopy(hidInBuffer, (0x20 + offset), uintarr, 0, 4);
            uint touchpad1Raw = uintarr[0];
            inputState.TouchPoint1.Y = (touchpad1Raw & 0xFFF00000) >> 20;
            inputState.TouchPoint1.X = (touchpad1Raw & 0x000FFF00) >> 8;
            inputState.TouchPoint1.IsDown = (touchpad1Raw & (1 << 7)) == 0;
            inputState.TouchPoint1.Id = (byte)(touchpad1Raw & 127);

            /// Touch State 2
            Buffer.BlockCopy(hidInBuffer, (0x24 + offset), uintarr, 0, 4);
            uint touchpad2Raw = uintarr[0];
            inputState.TouchPoint2.Y = (touchpad2Raw & 0xFFF00000) >> 20;
            inputState.TouchPoint2.X = (touchpad2Raw & 0x000FFF00) >> 8;
            inputState.TouchPoint2.IsDown = (touchpad2Raw & (1 << 7)) == 0;
            inputState.TouchPoint2.Id = (byte)(touchpad2Raw & 127);

            /// Headphones
            inputState.HeadPhonesConnected = (hidInBuffer[0x35 + offset] & 0x01) == 1;

            /// Trigger feedback
            inputState.LeftTriggerFeedback = hidInBuffer[0x2A + offset];
            inputState.RightTriggerFeedback = hidInBuffer[0x29 + offset];

            /// Battery TODO always 0?
            inputState.Battery.IsCharging = (hidInBuffer[0x35 + offset] & 0x08) == 1;
            inputState.Battery.IsFullyCharged = (hidInBuffer[0x36 + offset] & 0x20) == 1;
            inputState.Battery.Level = (byte)(hidInBuffer[0x36 + offset] & 0x0F);
        }
        #endregion
    }
}
