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
 * @file DualSense5.cs
 * @author Martin Mayr, Sebastian Fragner
 * @date 05.06.2022
 * @brief 'Model' of the DualSense5 for easy usage in any MVVM based application.
 * 
 * This file does not provide any new functionality for the DualSense5Lib, but rather
 * uses existing functions and datatypes to create a Model, which can be instantiated 
 * quickly.
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DualSense5Library
{
    #region Event Handlers
    public delegate void ConnectionChangedEventHandler(object sender, ConnectionChangedEventArgs e);

    public delegate void InputStateChangedEventHandler(object sender, InputStateChangedEventArgs e);

    /// <summary>
    /// EventArgs for ConnectionChanged Event.
    /// </summary>
    public class ConnectionChangedEventArgs : EventArgs
    {
        #region Properties
        /// <summary>
        /// The new ConnectionStatus.
        /// </summary>
        public bool ConnectionStatus { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionStatus">The new ConnectionStatus.</param>
        public ConnectionChangedEventArgs(bool connectionStatus)
        {
            ConnectionStatus = connectionStatus;
        }
        #endregion
    }

    /// <summary>
    /// EventArgs for InputStateChanged Event.
    /// </summary>
    public class InputStateChangedEventArgs : EventArgs
    {
        #region Properties
        /// <summary>
        /// The new InputState.
        /// </summary>
        public DS5InputState InputState { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputState">The new InputState.</param>
        public InputStateChangedEventArgs(DS5InputState inputState)
        {
            InputState = inputState;
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// DualSense5. Provides all features of the DualSense5Lib.
    /// </summary>
    public class DualSense5 
    {
        #region Private Members
        /// <summary>
        /// Class used for input/output handling.
        /// </summary>
        private DualSense5Library.DS5IO _io;

        /// <summary>
        /// Class used to save the device context.
        /// </summary>
        private DualSense5Library.DeviceContext _deviceContext;

        /// <summary>
        /// Backing field for <see cref="InputState"./>
        /// </summary>
        private DualSense5Library.DS5InputState _inputState;

        /// <summary>
        /// Backing field for <see cref="OutputState"./>
        /// </summary>
        private DualSense5Library.DS5OutputState _outputState;

        /// <summary>
        /// Saves path and connection type.
        /// </summary>
        private DualSense5Library.DeviceEnumInfo[] _deviceEnumInfo = new DualSense5Library.DeviceEnumInfo[1];

        /// <summary>
        /// How many DualSense5 Controllers are currently connected. NOTE: Current limit: 1. 
        /// </summary>
        private uint _controllerCount = 0;

        private CancellationTokenSource _tokenSource;
        private Task _start_task = null;

        /// <summary>
        /// Backing field for <see cref="IsConnected"./>
        /// </summary>
        private bool _isConnected;
        #endregion

        #region Properties

        /// <summary>
        /// Current InputState of the DualSense5.
        /// </summary>
        public DS5InputState InputState
        {
            get { return _inputState; }
            set
            {
                _inputState = value;
                OnInputStateChanged(value);
            }
        }

        /// <summary>
        /// Current OutputState of the DualSense5.
        /// </summary>
        public DS5OutputState OutputState
        {
            get
            {
                // TODO: Analyse why this is necessary! If not here, then no change of output state.
                OnOutputStateChanged(_outputState);
                return _outputState;
            }
            set
            {
                _outputState = value;
                OnOutputStateChanged(value);
            }
        }

        /// <summary>
        /// Current Connection Status.
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value; 
                OnConnectionChanged(value);
            }
        }
        #endregion

        #region Enumerables

        /// <summary>
        /// Provides enum <see cref="MicLed"/> for external usage.
        /// </summary>
        public IEnumerable<MicLed> MicLedEnum
        {
            get
            {
                return Enum.GetValues(typeof(MicLed)).Cast<MicLed>();
            }
        }

        /// <summary>
        /// Provides enum <see cref="LedBrightness"/> for external usage.
        /// </summary>
        public IEnumerable<LedBrightness> LedBrightnessEnum
        {
            get
            {
                return Enum.GetValues(typeof(LedBrightness)).Cast<LedBrightness>(); 
            }
        }

        /// <summary>
        /// Provides enum <see cref="TriggerEffectType"/> for external usage.
        /// </summary>
        public IEnumerable<TriggerEffectType> TriggerEffectTypeEnum
        {
            get
            {
                return Enum.GetValues(typeof(TriggerEffectType)).Cast<TriggerEffectType>(); 
            }
        }
        #endregion

        #region Event Handling
        /// <summary>
        /// Event Handler for ConnectionChanged event.
        /// </summary>
        public event ConnectionChangedEventHandler ConnectionChanged;

        /// <summary>
        /// Event Handler for InputStateChanged event.
        /// </summary>
        public event InputStateChangedEventHandler InputStateChanged;

        /// <summary>
        /// Event Handler for PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void OnInputStateChanged(DS5InputState inputState)
        {
            if (InputStateChanged != null)
            {
                InputStateChanged(this, new InputStateChangedEventArgs(inputState));
            }
        }

        private void OnConnectionChanged(bool connectionStatus)
        {
            if (ConnectionChanged != null)
            {
                ConnectionChanged(this, new ConnectionChangedEventArgs(connectionStatus));
            }
        }

        private void OnOutputStateChanged(DS5OutputState outputState)
        {
            _io.SetDeviceOutputState(_deviceContext, outputState);
        }
        #endregion

        #region Constructor
        public DualSense5()
        {
            _io = new DualSense5Library.DS5IO();
            _deviceContext = new DualSense5Library.DeviceContext();
            _inputState = new DualSense5Library.DS5InputState();
            _outputState = new DualSense5Library.DS5OutputState();
            _tokenSource = new CancellationTokenSource();
        }

        ~DualSense5()
        {
            _tokenSource.Cancel();
            if (_start_task != null)
            {
                Task.WhenAll(_start_task);
            }
            _tokenSource.Dispose();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Reads InputState of DualSense5 in a loop, and sets the connection status to false if the connection breaks.
        /// </summary>
        /// <param name="ct">Token for proper cleanup.</param>
        /// <returns></returns>
        private async Task RunAsync(CancellationToken ct)
        {
            DS5InputState tmp = new DS5InputState();

            while (true)
            {
                // cancel task
                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("RunAsync() cancelled");
                    ct.ThrowIfCancellationRequested();
                }


                await ConnectAsync(ct);

                while (IsConnected)
                {
                    // cancel task
                    if (ct.IsCancellationRequested)
                    {
                        Console.WriteLine("RunAsync() cancelled");
                        ct.ThrowIfCancellationRequested();
                    }

                    // try to read from controller
                    DS5ReturnValue rv = _io.GetDeviceInputState(ref _deviceContext, ref tmp);
                    if (rv == DS5ReturnValue.OK)
                    {
                        InputState = tmp;
                    }
                    else
                    {
                        IsConnected = false;
                        MessageBox.Show("Unexpected Error in DualSense5Library: " + rv.ToString());
                    }

                    await Task.Delay(1);
                }
            }
        }

        /// <summary>
        /// Tries to connect to a DualSense5. Waits 2 seconds before trying again, when the connection was not successfully established.
        /// </summary>
        /// <param name="ct">Token for proper cleanup.</param>
        /// <returns></returns>
        private async Task ConnectAsync(CancellationToken ct)
        {
            int delay_task = 2000;
            string InformationText = "connect: EnumDevices";

            Console.WriteLine(InformationText);

            DS5ReturnValue rv = _io.EnumDevices(ref _deviceEnumInfo, 1, ref _controllerCount);
            while (rv != DS5ReturnValue.OK)
            {
                // cancel task
                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("ConnectAsync() canceled");
                    ct.ThrowIfCancellationRequested();
                }

                Console.WriteLine(InformationText);

                await Task.Delay(delay_task);
                rv = _io.EnumDevices(ref _deviceEnumInfo, 1, ref _controllerCount);
            }

            InformationText = "connect: InitDeviceContext";
            Console.WriteLine(InformationText);

            rv = _io.InitDeviceContext(ref _deviceEnumInfo[0], ref _deviceContext);
            while (rv != DS5ReturnValue.OK)
            {
                // cancel task
                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("ConnectAsync() canceled");
                    ct.ThrowIfCancellationRequested();
                }

                Console.WriteLine(InformationText);

                await Task.Delay(delay_task);
                rv = _io.InitDeviceContext(ref _deviceEnumInfo[0], ref _deviceContext);
            }

            IsConnected = true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// First connects to DualSense5 and the starts reading the InputState. 
        /// </summary>
        public void Start()
        {
            _start_task = Task.Run(() => RunAsync(_tokenSource.Token), _tokenSource.Token);
        }
        #endregion
    }
}
