using System;
using System.Runtime.InteropServices;

namespace Nfysoft.GamecubeControllerSupport
{
    public static class PluginCalls
    {
        public const string NativeLib = "gamecube_controller";

        [DllImport(NativeLib, EntryPoint = "setup_debug_log")]
        public static extern bool SetUpDebugLog(Action<string> callback);

        [DllImport(NativeLib, EntryPoint = "is_device_connected")]
        public static extern bool IsDeviceConnected();

        [DllImport(NativeLib, EntryPoint = "start")]
        public static extern int Start(byte[] swap, Action dataRead, Action<int> pollingStopped);

        [DllImport(NativeLib, EntryPoint = "rumble")]
        public static extern int Rumble(byte[] ports);

        [DllImport(NativeLib, EntryPoint = "stop")]
        public static extern void Stop();

        [DllImport(NativeLib, EntryPoint = "reset")]
        public static extern void Reset();
    }
}