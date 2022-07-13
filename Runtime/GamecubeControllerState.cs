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
        [InputControl(name = "leftStick",   displayName = "Left Stick", layout = "Stick", format = "VC2S")]
        [InputControl(name = "leftStick/x", displayName = "Left stick X", defaultState = 127, offset = 0, format = "BYTE",
            parameters = "normalize, normalizeMin=0, normalizeMax=1, normalizeZero=0.5")]
        [InputControl(name = "leftStick/y", displayName = "Left stick Y", defaultState = 127, offset = 1, format = "BYTE",
            parameters = "normalize, normalizeMin=0, normalizeMax=1, normalizeZero=0.5")]
        public byte third;
        public byte fourth;

        // C stick
        [InputControl(name = "cStick",   displayName = "C Stick", layout = "Stick", format = "VC2S")]
        [InputControl(name = "cStick/x", displayName = "C Stick X", defaultState = 127, offset = 0, format = "BYTE",
            parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "cStick/y", displayName = "C Stick Y", defaultState = 127, offset = 1, format = "BYTE",
            parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
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
