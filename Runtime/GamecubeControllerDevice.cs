using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace Nfysoft.GamecubeControllerSupport
{
    [InputControlLayout(displayName = "Gamecube Controller", stateType = typeof(GamecubeControllerState))]
    public class GamecubeControllerDevice : InputDevice
    {
        public ButtonControl ButtonA     { get; private set; }

        public ButtonControl ButtonB     { get; private set; }

        public ButtonControl ButtonX     { get; private set; }

        public ButtonControl ButtonY     { get; private set; }

        public DpadControl Dpad { get; private set; }

        public ButtonControl ButtonStart { get; private set; }

        public ButtonControl ButtonZ     { get; private set; }

        public ButtonControl ButtonR     { get; private set; }

        public ButtonControl ButtonL     { get; private set; }

        public StickControl  LeftStick   { get; private set; }

        public StickControl  CStick      { get; private set; }

        public AxisControl   AnalogL     { get; private set; }

        public AxisControl   AnalogR     { get; private set; }


        protected override void FinishSetup()
        {
            base.FinishSetup();

            ButtonA     = GetChildControl<ButtonControl>("aButton");
            ButtonB     = GetChildControl<ButtonControl>("bButton");
            ButtonX     = GetChildControl<ButtonControl>("xButton");
            ButtonY     = GetChildControl<ButtonControl>("yButton");
            Dpad        = GetChildControl<DpadControl>("dpad");
            ButtonStart = GetChildControl<ButtonControl>("startButton");
            ButtonZ     = GetChildControl<ButtonControl>("zButton");
            ButtonR     = GetChildControl<ButtonControl>("rButton");
            ButtonL     = GetChildControl<ButtonControl>("lButton");

            LeftStick = GetChildControl<StickControl>("leftStick");
            CStick = GetChildControl<StickControl>("cStick");
            AnalogL = GetChildControl<AxisControl>("leftTrigger");
            AnalogR = GetChildControl<AxisControl>("rightTrigger");
        }
    }
}
