using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AOT;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

namespace Nfysoft.GamecubeControllerSupport
{
    /// <summary>
    /// The gamecube controller adapter that polls inputs and acts as a interface to the unity input system.
    /// </summary>
    public class GamecubeControllerAdapter : MonoBehaviour //TODO: singleton?
    {
        private const string InterfaceName = "Gamecube Controller Adapter Interface";
        private const string ProductName = "Gamecube Controller";
        private const int PayloadSize = 37;
        private const int ScanTimeout = 500;

        private static readonly ConcurrentQueue<Action> _mainThreadExecution = new ConcurrentQueue<Action>();
        private static readonly GamecubeControllerDevice[] _ports = new GamecubeControllerDevice[4];
        private static Thread _deviceScanThread;
        private static bool _isScanningForDevice;
        private static bool _isStopping;
        private static byte[] _readData;

        [field: SerializeField, Tooltip("Enables debug logs.")]
        private bool EnableDebugLogs { get; set; }

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            InputSystem.RegisterLayout<GamecubeControllerDevice>
                (matches: new InputDeviceMatcher().WithInterface(InterfaceName).WithProduct(ProductName));

            if (EnableDebugLogs)
                PluginCalls.SetUpDebugLog(msg => Debug.Log($"plugin log: {msg}"));
        }

        private void OnEnable()
        {
            _isStopping = false;
            Open();
        }

        private void OnDisable() => Close();

        private void Update()
        {
            if (_isStopping)
                return;

            while (_mainThreadExecution.TryDequeue(out var action))
                action?.Invoke();
        }

        /// <summary>
        /// Connect to the device or start scanning for it if unable to connect.
        /// </summary>
        private static void Open()
        {
            if (TryToConnectDevice()) 
                return;

            _deviceScanThread = new Thread(ScanForDevice);
            _deviceScanThread.Start();
        }

        /// <summary>
        /// Close device and exit.
        /// </summary>
        private static void Close()
        {
            _isStopping = true;
            _isScanningForDevice = false;
            _deviceScanThread?.Join();
            PluginCalls.Stop();
            RemoveDevices();
        }

        /// <summary>
        /// Scan for the device periodically. And try connecting to the device when it was found.
        /// </summary>
        private static void ScanForDevice()
        {
            _isScanningForDevice = true;
            var deviceFound = false;
            while (_isScanningForDevice && !deviceFound)
            {
                deviceFound = PluginCalls.IsDeviceConnected();
                Thread.Sleep(ScanTimeout);
            }

            if (deviceFound)
                _mainThreadExecution.Enqueue(Open);
        }

        /// <summary>
        /// Try connecting to the device and start polling data from it.
        /// </summary>
        /// <returns>True when device was connected to and polling of inputs has started.</returns>
        private static bool TryToConnectDevice()
        {
            if (!PluginCalls.IsDeviceConnected())
                return false;

            _readData = new byte[PayloadSize];
            PluginCalls.Reset();
            var ret = PluginCalls.Start(_readData, OnDataRead, OnPollingStopped);

            return ret == 0;
        }

        /// <summary>
        /// When data was read, handle it and send to unity input system.
        /// </summary>
        delegate void OnDataReadDelegate();
        [MonoPInvokeCallback(typeof(OnDataReadDelegate))]
        private static void OnDataRead()
        {
            if (_isStopping)
                return;

            _mainThreadExecution.Enqueue(() => HandleInputData(_readData));
        }

        /// <summary>
        /// Start scanning for the device again when polling stopped.
        /// </summary>
        delegate void OnPollingStoppedDelegate(int errorCode);
        [MonoPInvokeCallback(typeof(OnPollingStoppedDelegate))]
        private static void OnPollingStopped(int errorCode)
        {
            if (_isStopping)
                return;

            if (errorCode == (int) LibUsbError.NoDevice) //TODO: maybe also do this on some other errors.
                Open();
            else
                Debug.LogError(((LibUsbError) errorCode).ToString());
        }

        /// <summary>
        /// Handles the received input data.
        /// </summary>
        /// <param name="data">The input data.</param>
        private static void HandleInputData(IReadOnlyList<byte> data)
        {
            //if all the bytes of a port equal 0, it means theres no controller plugged into that port.

            if (data.Where((_, i) => i is > 1 and < 10).Any(b => b != 0))
            {
                _ports[0] ??= (GamecubeControllerDevice) InputSystem.AddDevice(ControllerDescription());
                InputSystem.QueueStateEvent(_ports[0], CreateControllerState(0, data));
            }

            if (data.Where((_, i) => i is > 9 and < 19).Any(b => b != 0))
            {
                _ports[1] ??= (GamecubeControllerDevice) InputSystem.AddDevice(ControllerDescription());
                InputSystem.QueueStateEvent(_ports[1], CreateControllerState(1, data));
            }

            if (data.Where((_, i) => i is > 18 and < 28).Any(b => b != 0))
            {
                _ports[2] ??= (GamecubeControllerDevice) InputSystem.AddDevice(ControllerDescription());
                InputSystem.QueueStateEvent(_ports[2], CreateControllerState(2, data));
            }

            if (data.Where((_, i) => i is > 27 and < 37).Any(b => b != 0))
            {
                _ports[3] ??= (GamecubeControllerDevice) InputSystem.AddDevice(ControllerDescription());
                InputSystem.QueueStateEvent(_ports[3], CreateControllerState(3, data));
            }
        }

        /// <summary>
        /// Creates a new state for the port using the adapter data.
        /// </summary>
        /// <param name="portIndex">The port index. (eg. 0 => Port 1)</param>
        /// <param name="data">The data of the adapter.</param>
        /// <returns>The <see cref="GamecubeControllerState"/> of the port.</returns>
        private static GamecubeControllerState CreateControllerState(int portIndex, IReadOnlyList<byte> data)
        {
            var offset = portIndex * 9;
            return new GamecubeControllerState()
            {
                first   = data[offset + 2],
                second  = data[offset + 3],
                third   = data[offset + 4],
                fourth  = data[offset + 5],
                fifth   = data[offset + 6],
                sixth   = data[offset + 7],
                seventh = data[offset + 8],
                eighth  = data[offset + 9],
            };
        }

        /// <summary>
        /// Returns the default <see cref="InputDeviceDescription"/> for a gamecube controller adapter.
        /// </summary>
        /// <returns>The <see cref="InputDeviceDescription"/> for a gamecube controller adapter.</returns>
        private static InputDeviceDescription ControllerDescription()
        {
            return new InputDeviceDescription
            {
                interfaceName = InterfaceName,
                product = ProductName,
            };
        }

        /// <summary>
        /// Remove the devices from the input system.
        /// </summary>
        private static void RemoveDevices()
        {
            foreach (var port in _ports)
                if (port != null) InputSystem.RemoveDevice(port);

            Array.Clear(_ports, 0, 4);
        }

        /// <summary>
        /// Turn rumble on or off for the controllers.
        /// </summary>
        /// <param name="port1">The rumble status for the controller in port 1.</param>
        /// <param name="port2">The rumble status for the controller in port 2.</param>
        /// <param name="port3">The rumble status for the controller in port 3.</param>
        /// <param name="port4">The rumble status for the controller in port 4.</param>
        public static void Rumble(bool port1, bool port2, bool port3, bool port4)
        {
            PluginCalls.Rumble(new[]
            {
                (byte) (port1 ? 1 : 0),
                (byte) (port2 ? 1 : 0),
                (byte) (port3 ? 1 : 0),
                (byte) (port4 ? 1 : 0),
            });
        }

    }
}