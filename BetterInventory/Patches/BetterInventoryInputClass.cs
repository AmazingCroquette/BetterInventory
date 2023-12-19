using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace BetterInventory.Patches
{
    internal class EasyInventoryInputClass : LcInputActions
    {
        [InputAction("<Keyboard>/1", Name = "Inventory 1")]
        public InputAction Inventory1Key { get; set; }
        [InputAction("<Keyboard>/2", Name = "Inventory 2")]
        public InputAction Inventory2Key { get; set; }
        [InputAction("<Keyboard>/3", Name = "Inventory 3")]
        public InputAction Inventory3Key { get; set; }
        [InputAction("<Keyboard>/4", Name = "Inventory 4")]
        public InputAction Inventory4Key { get; set; }


        public static EasyInventoryInputClass Instance = new EasyInventoryInputClass();
    }

    internal class GlobalFlashlightInputClass : LcInputActions
    {
        [InputAction("<Keyboard>/T", Name = "Global Flashlight")]
        public InputAction GlobalFlashlightKey { get; set; }

        public static GlobalFlashlightInputClass Instance = new GlobalFlashlightInputClass();
    }

    internal class SwapModeInputClass : LcInputActions
    {
        [InputAction("<Keyboard>/X", Name = "Inventory Swap")]
        public InputAction SwapModeKey { get; set; }

        public static SwapModeInputClass Instance = new SwapModeInputClass();

    }
}
