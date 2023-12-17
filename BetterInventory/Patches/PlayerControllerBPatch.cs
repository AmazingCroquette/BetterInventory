using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using System.Reflection;

namespace BetterInventory.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        static EasyInventoryInputClass easyInventoryInputSystem = EasyInventoryInputClass.Instance;
        static GlobalFlashlightInputClass globalFlashlightInputSystem = GlobalFlashlightInputClass.Instance;

        static PlayerControllerB _localPlayer;
        static MethodInfo _switchToItemMethod;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Start()
        {
            //doesn't work, happens too early
            //_localPlayer = GameNetworkManager.Instance.localPlayerController;

            _switchToItemMethod = typeof(PlayerControllerB).GetMethod("SwitchToItemSlot", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch()
        {
            if (_localPlayer == null)
            {
                _localPlayer = GameNetworkManager.Instance.localPlayerController;
            }

            if(_localPlayer != null && !_localPlayer.twoHanded)
            {
                EasyInventoryPatch();
            }

            GlobalFlashlight();
        }

        static void EasyInventoryPatch()
        {
            object[] parameters = new object[] {null, null};

            _localPlayer = GameNetworkManager.Instance.localPlayerController;

            if (easyInventoryInputSystem == null)
            {
                return;
            }

            if (easyInventoryInputSystem.Inventory1Key.triggered)
            {
                parameters[0] = 0;
            }
            else if (easyInventoryInputSystem.Inventory2Key.triggered)
            {
                parameters[0] = 1;
            }
            else if (easyInventoryInputSystem.Inventory3Key.triggered)
            {
                parameters[0] = 2;
            }
            else if (easyInventoryInputSystem.Inventory4Key.triggered)
            {
                parameters[0] = 3;
            }

            if (parameters[0] != null)
            {
                _switchToItemMethod.Invoke(_localPlayer, parameters);
            }
        }

        static void GlobalFlashlight()
        {
            if (globalFlashlightInputSystem.GlobalFlashlightKey.triggered)
            {
                for(int i = 0; i < _localPlayer.ItemSlots.Length; i++)
                {
                    if(_localPlayer.ItemSlots[i] is FlashlightItem)
                    {
                        GrabbableObject flashlight = _localPlayer.ItemSlots[i];

                        flashlight.UseItemOnClient();
                        if(_localPlayer.currentItemSlot != i)
                        {
                            flashlight.PocketItem();
                        }
                        break;
                    }
                }
            }
        }
    }
}
