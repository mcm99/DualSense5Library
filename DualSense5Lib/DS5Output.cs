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
 * @file DS5Output.cs
 * @author Martin Mayr
 * @date 05.06.2022
 * @brief Provides functions for setting the OutputState of the DualSense5.
 * 
 * DISCLAIMER: This file is a C# port of
 *  https://github.com/Ohjurot/DualSense-Windows,
 *  which provides a C++ API for the DualSense.
 */

using System;

namespace DualSense5Library
{
    /// <summary>
    /// Provides function for setting the OutputState of the DualSense5.
    /// </summary>
    internal static class DS5Output
    {
        #region Public Methods

        /// <summary>
        /// Creates the hid output buffer.
        /// </summary>
        /// <param name="hidOutBuffer"></param>
        /// <param name="outputState"></param>
        public static void CreateHidOutputBuffer(byte[] hidOutBuffer, DS5OutputState outputState, uint offset)
        {
            /// Feature mask
            hidOutBuffer[0x00 + offset] = 0xFF;
            hidOutBuffer[0x01 + offset] = 0xF7;

            /// Rumble Motors
            hidOutBuffer[0x02 + offset] = outputState.RightRumble;
            hidOutBuffer[0x03 + offset] = outputState.LeftRumble;

            /// Mic LED
            hidOutBuffer[0x08 + offset] = (byte)outputState.MicrophoneLed;

            /// Player LED
            hidOutBuffer[0x2B + offset] = outputState.PlayerLeds.Bitmask;
            if (outputState.PlayerLeds.PlayerLedFade)
            {
                /// hidOutBuffer[0x2B] &= ~(0x20);
                hidOutBuffer[0x2B + offset] &= (0xDF);
            }
            else
            {
                hidOutBuffer[0x2B + offset] |= (0x20);
            }

            /// Player LED Brightness
            hidOutBuffer[0x26 + offset] = 0x03;
            hidOutBuffer[0x29 + offset] = outputState.DisableLeds ? (byte)0x01 : (byte)0x02;
            hidOutBuffer[0x2A + offset] = (byte)outputState.PlayerLeds.Brightness;

            hidOutBuffer[0x2C + offset] = outputState.Lightbar.R;
            hidOutBuffer[0x2D + offset] = outputState.Lightbar.G;
            hidOutBuffer[0x2E + offset] = outputState.Lightbar.B;

            ProcessTrigger(outputState.LeftTriggerEffect, hidOutBuffer, 0x15 + offset);
            ProcessTrigger(outputState.RightTriggerEffect, hidOutBuffer, 0x0A + offset);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Process trigger
        /// </summary>
        /// <param name="triggerEffect">Effect to be applied.</param>
        /// <param name="buffer">Buffer for trigger parameters.</param>
        private static void ProcessTrigger(TriggerEffect triggerEffect, byte[] buffer, uint offset)
        {
            /// switch on effect
            switch (triggerEffect.Type)
            {
                case TriggerEffectType.ContinuousResistance:
                    buffer[0x00 + offset] = 0x01;
                    buffer[0x01 + offset] = triggerEffect.StartPosition;
                    buffer[0x02 + offset] = triggerEffect.Force;
                    break;
                case TriggerEffectType.SectionResistance:
                    buffer[0x00 + offset] = 0x02;
                    buffer[0x01 + offset] = triggerEffect.StartPosition;
                    buffer[0x02 + offset] = triggerEffect.Force;
                    break;
                case TriggerEffectType.EffectEx:
                    buffer[0x00 + offset] = 0x02 | 0x20 | 0x04;
                    buffer[0x01 + offset] = (byte)(0xFF - triggerEffect.StartPosition);
                    if (triggerEffect.KeepEffect)
                    {
                        buffer[0x02] = 0x02;
                    }
                    buffer[0x04] = triggerEffect.BeginForce;
                    buffer[0x05] = triggerEffect.MiddleForce;
                    buffer[0x06] = triggerEffect.EndForce;
                    buffer[0x09] = (byte)Math.Max(1, triggerEffect.Frequency / 2);
                    break;
                case TriggerEffectType.Calibrate:
                    buffer[0x00 + offset] = 0xFC;
                    break;
                case TriggerEffectType.NoResistance:
                default:
                    buffer[0x00 + offset] = 0x00;
                    buffer[0x01 + offset] = 0x00;
                    buffer[0x02 + offset] = 0x00;
                    break;
            }
        }
        #endregion
    }
}
