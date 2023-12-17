using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace BetterInventory.Patches
{
    internal class EasyInventoryInputClass : LcInputActions
    {
        [InputAction("<Keyboard>/1", Name = "Inventory1")]
        public InputAction Inventory1Key { get; set; }
        [InputAction("<Keyboard>/2", Name = "Inventory2")]
        public InputAction Inventory2Key { get; set; }
        [InputAction("<Keyboard>/3", Name = "Inventory3")]
        public InputAction Inventory3Key { get; set; }
        [InputAction("<Keyboard>/4", Name = "Inventory4")]
        public InputAction Inventory4Key { get; set; }


        public static EasyInventoryInputClass Instance = new EasyInventoryInputClass();
    }
}
