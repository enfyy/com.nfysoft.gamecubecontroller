using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace Nfysoft.GamecubeControllerSupport
{
    /// <summary>
    /// Describes the state of an input of a nintendo gamecube controller connected with the nintendo game cube usb adapter.
    /// </summary>
    public struct GamecubeControllerState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('G','C','C','A');

        // A, B, X, Y, LEFT, RIGHT, DOWN, UP
        [InputControl(name= "aButton",     displayName = "A",     layout = "Button", bit = 0, format = "BIT")]
        [InputControl(name= "bButton",     displayName = "B",     layout = "Button", bit = 1, format = "BIT")]
        [InputControl(name= "xButton",     displayName = "X",     layout = "Button", bit = 2, format = "BIT")]
        [InputControl(name= "yButton",     displayName = "Y",     layout = "Button", bit = 3, format = "BIT")]
        [InputControl(name= "dpad",        layout = "Dpad", bit = 4, sizeInBits = 4)]
        [InputControl(name= "dpad/left",   displayName = "LEFT",  layout = "Button", bit = 4, format = "BIT")]
        [InputControl(name= "dpad/right",  displayName = "RIGHT", layout = "Button", bit = 5, format = "BIT")]
        [InputControl(name= "dpad/down",   displayName = "DOWN",  layout = "Button", bit = 6, format = "BIT")]
        [InputControl(name= "dpad/up",     displayName = "UP",    layout = "Button", bit = 7, format = "BIT")]
        public byte first;

        // START, Z, R, L
        [InputControl(name= "startButton", displayName = "Start", layout = "Button", bit = 0, format = "BIT")]
        [InputControl(name= "zButton",     displayName = "Z",     layout = "Button", bit = 1, format = "BIT")]
        [InputControl(name= "rButton",     displayName = "R",     layout = "Button", bit = 2, format = "BIT")]
        [InputControl(name= "lButton",     displayName = "L",     layout = "Button", bit = 3, format = "BIT")]
        public byte second;

        // Left stick
        [InputControl(name = "leftStick", format = "VC2S", layout = "Stick")]
        [InputControl(name = "leftStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
        [InputControl(name = "leftStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
        [InputControl(name = "leftStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85")]
        [InputControl(name = "leftStick/y", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
        [InputControl(name = "leftStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85,invert=false")]
        [InputControl(name = "leftStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
        public byte third;
        public byte fourth;

        // C stick
        [InputControl(name = "cStick", format = "VC2S", layout = "Stick")]
        [InputControl(name = "cStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
        [InputControl(name = "cStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
        [InputControl(name = "cStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85")]
        [InputControl(name = "cStick/y", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
        [InputControl(name = "cStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85,invert=false")]
        [InputControl(name = "cStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
        public byte fifth;
        public byte sixth;

        // Analog L
        [InputControl(name = "leftTrigger",  displayName = "Left Trigger",  layout = "Axis")]
        public byte seventh;

        // Analog R
        [InputControl(name = "rightTrigger", displayName = "Right Trigger", layout = "Axis")]
        public byte eighth;
    }
}
