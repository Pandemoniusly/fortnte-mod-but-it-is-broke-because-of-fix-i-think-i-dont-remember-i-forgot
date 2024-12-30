using LethalCompanyInputUtils.Api;
using LethalCompanyInputUtils.BindingPathEnums;
using UnityEngine.InputSystem;

namespace CarStuff.BindingInfo
{
    public class gravbinds : LcInputActions
    {
        public static readonly gravbinds Instance = new();

        public InputAction switchwall => Asset["gravbindss"];

        public override void CreateInputActions(in InputActionMapBuilder builder)
        {
            builder.NewActionBinding()
                .WithActionId("gravbindss")
                .WithActionType(InputActionType.Button)
                .WithKeyboardControl(KeyboardControl.J) // or .WithKbmPath("<Keyboard>/j")
                .WithGamepadControl(GamepadControl.ButtonNorth) // or .WithGamepadPath("<Gamepad>/buttonNorth")
                .WithBindingName("BindGravity")
                .Finish();
        }
    }
}
