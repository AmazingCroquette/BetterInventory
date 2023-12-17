using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace BetterInventory.Patches
{
    internal class GlobalFlashlightInputClass : LcInputActions
    {
        [InputAction("<Keyboard>/T", Name = "GlobalFlashlight")]
        public InputAction GlobalFlashlightKey { get; set; }

        public static GlobalFlashlightInputClass Instance = new GlobalFlashlightInputClass();
    }
}
