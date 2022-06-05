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
 * @file DS5State.cs
 * @author Martin Mayr
 * @date 05.06.2022
 * @brief Types for Input and Output state.
 * 
 * DISCLAIMER: This file is a C# port of
 *  https://github.com/Ohjurot/DualSense-Windows,
 *  which provides a C++ API for the DualSense.
 */

using System;

namespace DualSense5Library
{

    #region Enums
    /// <summary>
    /// State of the Mic Led.
    /// </summary>
    public enum MicLed
    {
        Off = 0,
        On = 1,
        Pulse = 2
    }

    /// <summary>
    /// Type of Trigger effect.
    /// </summary>
    public enum TriggerEffectType
    {
        /// <summary>
        /// No resistance is applied.
        /// </summary>
        NoResistance = 0,

        /// <summary>
        /// Continuous resistance is applied.
        /// </summary>
        ContinuousResistance = 1,

        /// <summary>
        /// Section resistance is applied.
        /// </summary>
        SectionResistance = 2,

        /// <summary>
        /// Extended trigger effect.
        /// </summary>
        EffectEx = 0x26,

        /// <summary>
        /// Calibrate triggers.
        /// </summary>
        Calibrate = 0xFC
    }
    public enum LedBrightness
    {
        High = 0,
        Medium = 1,
        Low = 2
    }
    #endregion

    #region Classes
    /// <summary>
    /// Addresses for DPad register.
    /// </summary>
    public static class DS5IStateDPad {
        public static readonly byte DS5IStateDPadLeft = 0x01;
        public static readonly byte DS5IStateDPadDown = 0x02;
        public static readonly byte DS5IStateDPadRight = 0x04;
        public static readonly byte DS5IStateDPadUp = 0x08;
        public static readonly byte DS5IStateBtxSquare = 0x10;
        public static readonly byte DS5IStateBtxCross = 0x20;
        public static readonly byte DS5IStateBtxCircle = 0x40;
        public static readonly byte DS5IStateBtxTriangle = 0x80;
    }


    /// <summary>
    /// Addresses for Button A register.
    /// </summary>
    public static class DS5IStateButtonA
    {
        public static readonly byte DS5IStateBtnALeftBumper = 0x01;
        public static readonly byte DS5IStateBtnARightBumper = 0x02;
        public static readonly byte DS5IStateBtnALeftTrigger = 0x04;
        public static readonly byte DS5IStateBtnARightTrigger = 0x08;
        public static readonly byte DS5IStateBtnASelect = 0x10;
        public static readonly byte DS5IStateBtnAMenu = 0x20;
        public static readonly byte DS5IStateBtnALeftStick = 0x40;
        public static readonly byte DS5IStateBtnARightStick = 0x80; 
    }

    /// <summary>
    /// Addresses for Button B register.
    /// </summary>
    public static class DS5IStateButtonB
    {
        public static readonly byte DS5IStateBtnBPlaystationLogo = 0x01;
        public static readonly byte DS5IStateBtnBPadButton = 0x02;
        public static readonly byte DS5IStateBtnBMicButton = 0x04;
    }


    /// <summary>
    /// Addresses for PlayerLed register.
    /// </summary>
    public static class DS5OStatePlayerLed
    {
        public static readonly byte DS5OStatePlayerLedLeft = 0x01;
        public static readonly byte DS5OStatePlayerLedMiddleLeft = 0x02;
        public static readonly byte DS5OStatePlayerLedMiddle = 0x04;
        public static readonly byte DS5OStatePlayerLedMiddleRight = 0x08;
        public static readonly byte DS5OStatePlayerLedRight = 0x10;
    }


    public class AnalogStick
    {
        #region Properties
        /// <summary>
        /// X Position of stick (0 = Center).
        /// </summary>
        public sbyte X { get; set; }

        /// <summary>
        /// Y Position of stick (0 = Center).
        /// </summary>
        public sbyte Y { get; set; }
        #endregion
    }

    public class Button
    {
        #region Properties
        /// <summary>
        /// Wheter the button is currently pressed or not.
        /// </summary>
        public bool IsPressed { get; set; }
        #endregion
    }

    /// <summary>
    /// 3 component vector.
    /// </summary>
    public class Vector3
    {
        #region Constructor
        public Vector3() {}

        public Vector3(Int16 x, Int16 y, Int16 z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        #endregion

        #region Properties
        public Int16 X { get; set; }
        public Int16 Y { get; set; }
        public Int16 Z { get; set; }
        #endregion
    }

    /// <summary>
    /// RGB Color.
    /// </summary>
    public class Color
    {
        #region Constructor
        public Color()
        { }

        public Color(byte r, byte g, byte b)
        {
            R = r; G = g; B = b; 
        }
        #endregion

        #region Properties
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        #endregion
    }

    public class Touch
    {
        #region Properties
        /// <summary>
        /// X position of finger (~0 - 2000)
        /// </summary>
        public uint X { get; set; }

        /// <summary>
        /// Y position of finger (~0 - 2048)
        /// </summary>
        public uint Y { get; set; }

        /// <summary>
        /// Touchpad is pushed down.
        /// </summary>
        public bool IsDown { get; set; }

        /// <summary>
        /// ID for touch.
        /// </summary>
        public byte Id { get; set; }
        #endregion
    }

    public class Battery
    {
        #region Properties
        /// <summary>
        /// Charging state of the battery.
        /// </summary>
        public bool IsCharging { get; set; }   

        /// <summary>
        /// Indicates that the battery is fully charged.
        /// </summary>
        public bool IsFullyCharged { get; set; }

        /// <summary>
        /// Battery charge level.
        /// </summary>
        public byte Level { get; set; }
        #endregion
    }


    public class TriggerEffect
    {
        #region Properties
        public TriggerEffectType Type { get; set; }

        /// <summary>
        /// Union one raw data.
        /// </summary>
        public byte[] U1_Raw { get; set; }

        /// <summary>
        /// Start Position of resistance.
        /// </summary>
        public byte StartPosition { get; set; }
        /// <summary>
        /// Force of resistance.
        /// </summary>
        public byte Force { get; set; }
        /// <summary>
        /// Unused pad.
        /// </summary>
        public byte[] Pad { get; set; }

        /// <summary>
        /// End Position of resistance (>= StartPosition).
        /// </summary>
        public byte EndPosition { get; set; }

        /// <summary>
        /// Wheter the effect should keep playing when the trigger goes beyond 255.
        /// </summary>
        public bool KeepEffect { get; set; }

        /// <summary>
        /// Force applied when trigger >= (255 / 2).
        /// </summary>
        public byte BeginForce { get; set; }
        /// <summary>
        /// Force applied when trigger <= (255 / 2).
        /// </summary>
        public byte MiddleForce { get; set; }
        /// <summary>
        /// Force applied when trigger is beyond 255.
        /// </summary>
        public byte EndForce { get; set; }
        /// <summary>
        /// Vibration Frequency of the trigger.
        /// </summary>
        public byte Frequency { get; set; }
        #endregion
    }



    public class PlayerLeds
    {
        #region Properties
        /// <summary>
        /// Player indication leds bitflag.
        /// </summary>
        public byte Bitmask { get; set; }  
        /// <summary>
        /// Indicates wheter the player leds should fade in.
        /// </summary>
        public bool PlayerLedFade { get; set; }
        /// <summary>
        /// Brightness of the player leds.
        /// </summary>
        public LedBrightness Brightness { get; set; }
        #endregion
    }

    public class DS5InputState
    {
        #region Constructor
        public DS5InputState()
        {
            LeftStick = new AnalogStick();
            RightStick = new AnalogStick();
            Accelerometer = new Vector3();
            Gyroscope = new Vector3();
            Gyroscope_F = new Vector3(0, 0, 0);
            TouchPoint1 = new Touch();
            TouchPoint2 = new Touch();
            Battery = new Battery();
            DPadLeft = new Button();
            DPadUp = new Button();
            DPadRight = new Button();
            DPadDown = new Button();
            Square = new Button();
            Triangle = new Button();
            Circle = new Button();
            Cross = new Button();
            LeftBumper = new Button();
            RightBumper = new Button();
            LeftTriggerButton = new Button();
            RightTriggerButton = new Button();
            LeftStickButton = new Button();
            RightStickButton = new Button();
            Menu = new Button();
            Select = new Button();
            PlayStationLogo = new Button();
            MicrophoneButton = new Button();
            Touchpad = new Button();
        }
        #endregion

        #region Properties
        public AnalogStick LeftStick { get; set; }

        public AnalogStick RightStick { get; set; }

        public byte LeftTrigger { get; set; }  

        public byte RightTrigger { get; set; }

        public byte ButtonsAndDpad { get; set; }

        public Button DPadLeft { get; set; }

        public Button DPadUp { get; set; }

        public Button DPadRight { get; set; }

        public Button DPadDown { get; set; }

        public Button Square { get; set; }

        public Button Triangle { get; set; }

        public Button Circle { get; set; }

        public Button Cross { get; set; }

        public Button LeftBumper { get; set; }

        public Button RightBumper { get; set; } 

        public Button LeftStickButton { get; set; }
        
        public Button RightStickButton { get; set; }

        public Button LeftTriggerButton { get; set; }

        public Button RightTriggerButton { get; set; }

        public Button Menu { get; set; }

        public Button Select { get; set; }  

        public Button Touchpad { get; set; }

        public Button PlayStationLogo { get; set; } 

        public Button MicrophoneButton { get; set; }

        public Vector3 Accelerometer { get; set; }

        public Vector3 Gyroscope { get; set; }

        /// <summary>
        /// Gyroscope data filtered.
        /// </summary>
        public Vector3 Gyroscope_F { get; set; }

        public Touch TouchPoint1 { get; set; }

        public Touch TouchPoint2 { get; set; }

        public Battery Battery { get; set; }

        public bool HeadPhonesConnected { get; set; }

        public byte LeftTriggerFeedback { get; set; }

        public byte RightTriggerFeedback { get; set; }

        public byte ButtonsA { get; set; }

        public byte ButtonsB { get; set; }
        #endregion
    }

    public class DS5OutputState 
    {
        #region Constructor
        public DS5OutputState()
        {
            PlayerLeds = new PlayerLeds();
            Lightbar = new Color();
            LeftTriggerEffect = new TriggerEffect();
            RightTriggerEffect = new TriggerEffect();
            LeftRumble = new byte();
            RightRumble = new byte();
            MicrophoneLed = new MicLed();
            DisableLeds = new bool();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Left / Hard rumble motor.
        /// </summary>
        public byte LeftRumble { get; set; }
        /// <summary>
        /// Right / Soft rumble motor.
        /// </summary>
        public byte RightRumble { get; set; }
        /// <summary>
        /// State of the microphone led.
        /// </summary>
        public MicLed MicrophoneLed { get; set; }
        /// <summary>
        /// Disable all leds.
        /// </summary>
        public bool DisableLeds { get; set; }
        /// <summary>
        /// Player leds.
        /// </summary>
        public PlayerLeds PlayerLeds { get; set; }
        /// <summary>
        /// Color of the lightbar.
        /// </summary>
        public Color Lightbar { get; set; }
        /// <summary>
        /// Effect of left trigger.
        /// </summary>
        public TriggerEffect LeftTriggerEffect { get; set; }
        /// <summary>
        /// Effect of right trigger.
        /// </summary>
        public TriggerEffect RightTriggerEffect { get; set; }
        #endregion 

    }
    #endregion

}
