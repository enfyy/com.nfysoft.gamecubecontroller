namespace Nfysoft.GamecubeControllerSupport
{
    /// <summary>
    /// Feedback and error code messages from the plugin
    /// </summary>
    public enum PluginFeedback
    {
        /// <summary>
        /// Success
        /// </summary>
        Success = 0,

        /// <summary>
        /// The device was not found.
        /// </summary>
        NotFound = 1,

        /// <summary>
        /// Initialization failed.
        /// </summary>
        InitFailed  = 2,

        /// <summary>
        /// No devices were found in the list.
        /// </summary>
        NoDevices   = 3,

        /// <summary>
        /// Opening the adapter failed.
        /// </summary>
        OpenFailed  = 4,

        /// <summary>
        /// Claiming the interface failed.
        /// </summary>
        ClaimFailed = 5,

        /// <summary>
        /// The initial transfer failed.
        /// </summary>
        InitialTransferFailed = 6,

        /// <summary>
        /// Hotplug is not supported on this platform
        /// </summary>
        NoHotplugSupport = 7,

        /// <summary>
        /// 
        /// </summary>
        HotplugRegisterFailed = 8,

        /// <summary>
        /// Tried to rumble but plugin was not started.
        /// </summary>
        NotStarted = 9,

        /// <summary>
        /// The rumble transfer failed.
        /// </summary>
        RumbleFailed = 10
    }
}