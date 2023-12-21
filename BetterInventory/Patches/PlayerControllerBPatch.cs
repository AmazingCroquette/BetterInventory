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

        static bool _shouldDeactivateSwapMode = false;
        static void EasyInventoryPatch()
        {
            if (_localPlayer.twoHanded || _localPlayer.inTerminalMenu)
            {
                return;
            }

            object[] parameters = new object[] { null, null };
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
                //BetterInventory.logger.LogInfo("EasyInventory called");
                _switchToItemMethod.Invoke(_localPlayer, parameters);
                _shouldDeactivateSwapMode = true;
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
                //BetterInventory.logger.LogInfo("GlobalFlashlight called");
            }
        }

        static bool swapModeActive = false;
        static void ToggleSwapMode()
        {

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

            if (swapModeActive)
            {
                if (_shouldDeactivateSwapMode || _localPlayer.twoHanded || _localPlayer.inTerminalMenu)
                {
                    DeactivateSwapMode();
                }
            }
        }

        static int initialSwapIndex = -1;
        static Color _inventoryFrameColor = Color.black;

        static bool skipNextCall = false;

        [HarmonyPatch("SwitchToItemSlot")]
        [HarmonyPrefix]
        static void ManageSwapMode(int slot, ref GrabbableObject fillSlotWithItem)
        {
            if (!swapModeActive || _localPlayer.twoHanded)
            {
                return;
            }

            if (skipNextCall) //swap uses same function, need to skip some calls
            {
                skipNextCall = false;
                return;
            }

            bool isGrabbingNewItem = fillSlotWithItem != null;
            if (isGrabbingNewItem)
            {
                _shouldDeactivateSwapMode = true;
                return;
            }

            int newSwapIndex = slot;

            GrabbableObject swap1 = _localPlayer.ItemSlots[initialSwapIndex];
            GrabbableObject swap2 = _localPlayer.ItemSlots[newSwapIndex];

            skipNextCall = true;
            ChangeItemInSlot(initialSwapIndex, swap2);
            skipNextCall = true;
            ChangeItemInSlot(newSwapIndex, swap1);

            if (!_shouldDeactivateSwapMode)
            {
                //setup swap for new slot
                HighlightInventoryForSwapMode();
                initialSwapIndex = newSwapIndex;
            }
        }

        [HarmonyPatch("SwitchToItemSlot")]
        [HarmonyPostfix]
        static void PostSlotSwitch()
        {
            if (swapModeActive)
            {
                //cancel inventory fade out
                HUDManager.Instance.PingHUDElement(HUDManager.Instance.Inventory, 0.1f, 1f, 1f);
            }
            else
            {
                //inventory fade out
                HUDManager.Instance.PingHUDElement(HUDManager.Instance.Inventory, 1.5f, 1f, 0.13f);
            }
        }

        static void HighlightInventoryForSwapMode()
        {
            if (_inventoryFrameColor == Color.black) //base color is not black (currently)
            {
                _inventoryFrameColor = HUDManager.Instance.itemSlotIconFrames[_localPlayer.currentItemSlot].color;
            }

            //change frames color to highlight swap mode
            for (int i = 0; i < _localPlayer.ItemSlots.Length; i++)
            {
                Color frameColor = (i == _localPlayer.currentItemSlot) ? Color.green : Color.gray;

                HUDManager.Instance.itemSlotIconFrames[i].color = frameColor;
            }
        }

        static void ActivateSwapMode()
        {
            //BetterInventory.logger.LogInfo("SwapMode active");

            swapModeActive = true;
            _shouldDeactivateSwapMode = false;
            initialSwapIndex = _localPlayer.currentItemSlot;

            HighlightInventoryForSwapMode();
            PostSlotSwitch();
        }

        static void DeactivateSwapMode()
        {
            //BetterInventory.logger.LogInfo("SwapMode inactive");
            //reset slots color
            for (int i = 0; i < _localPlayer.ItemSlots.Length; i++)
            {
                HUDManager.Instance.itemSlotIconFrames[i].color = _inventoryFrameColor;
            }

            swapModeActive = false;
            PostSlotSwitch();
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
