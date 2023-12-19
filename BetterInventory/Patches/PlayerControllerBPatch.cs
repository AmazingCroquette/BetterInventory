using GameNetcodeStuff;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

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
            _switchToItemMethod = typeof(PlayerControllerB).GetMethod("SwitchToItemSlot", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch()
        {
            if (!(_localPlayer = GameNetworkManager.Instance.localPlayerController))
            {
                return;
            }

            EasyInventoryPatch();
            GlobalFlashlight();
            ToggleSwapMode();
        }

        static void EasyInventoryPatch()
        {
            if (_localPlayer.twoHanded || _localPlayer.inTerminalMenu)
            {
                return;
            }

            object[] parameters = new object[] { null, null };

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
            if (_localPlayer.inTerminalMenu)
            {
                return;
            }

            if (globalFlashlightInputSystem.GlobalFlashlightKey.triggered)
            {
                for (int i = 0; i < _localPlayer.ItemSlots.Length; i++)
                {
                    if (_localPlayer.ItemSlots[i] is FlashlightItem)
                    {
                        FlashlightItem flashlight = _localPlayer.ItemSlots[i] as FlashlightItem;
                        if (flashlight.insertedBattery.charge > 0)
                        {
                            flashlight.UseItemOnClient();
                            if (_localPlayer.currentItemSlot != i)
                            {
                                flashlight.PocketItem();
                            }
                            break;
                        }
                    }
                }
            }
        }

        static bool swapModeActive = false;
        static void ToggleSwapMode()
        {
            if (_localPlayer.twoHanded || _localPlayer.inTerminalMenu)
            {
                if (swapModeActive)
                {
                    DeactivateSwapMode();
                }
                return;
            }

            if (SwapModeInputClass.Instance.SwapModeKey.triggered)
            {
                if (!swapModeActive) //activate swap mode
                {
                    ActivateSwapMode();
                }
                else
                {
                    DeactivateSwapMode();
                }
            }
        }

        static int initialSwapIndex = -1;
        static Color _inventoryFrameColor = Color.black;

        [HarmonyPatch("SwitchToItemSlot")]
        [HarmonyPrefix]
        static void ManageSwapMode(int slot, ref GrabbableObject fillSlotWithItem)
        {
            if (!swapModeActive || _localPlayer.twoHanded)
            {
                return;
            }

            int newSwapIndex = slot;

            GrabbableObject swap1 = _localPlayer.ItemSlots[initialSwapIndex];
            GrabbableObject swap2 = _localPlayer.ItemSlots[newSwapIndex];

            DeactivateSwapMode();

            ChangeItemInSlot(initialSwapIndex, swap2);
            ChangeItemInSlot(newSwapIndex, swap1);
        }

        static void ActivateSwapMode()
        {
            if (_inventoryFrameColor == Color.black) //base color is not black (currently)
            {
                _inventoryFrameColor = HUDManager.Instance.itemSlotIconFrames[_localPlayer.currentItemSlot].color;
            }

            swapModeActive = true;
            initialSwapIndex = _localPlayer.currentItemSlot;

            //change frame color to highlight swap mode
            HUDManager.Instance.itemSlotIconFrames[_localPlayer.currentItemSlot].color = Color.green;

            //cancel inventory fade out
            _localPlayer.StopCoroutine(HUDManager.Instance.Inventory.fadeCoroutine);
            HUDManager.Instance.Inventory.targetAlpha = 1f;
        }

        static void DeactivateSwapMode()
        {
            //fade inventory
            HUDManager.Instance.PingHUDElement(HUDManager.Instance.Inventory, 1.5f, 1f, 0.13f);
            HUDManager.Instance.itemSlotIconFrames[initialSwapIndex].color = _inventoryFrameColor;

            swapModeActive = false;
        }

        static void ChangeItemInSlot(int slotIndex, GrabbableObject newObject)
        {
            _switchToItemMethod.Invoke(_localPlayer, new object[] { slotIndex, newObject });

            if (newObject == null)
            {
                _localPlayer.ItemSlots[slotIndex] = null;
                HUDManager.Instance.itemSlotIcons[slotIndex].enabled = false;
            }
        }
    }
}
