using BepInEx;
using BepInEx.Logging;
using BetterInventory.Patches;
using HarmonyLib;

namespace BetterInventory
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class BetterInventory : BaseUnityPlugin
    {
        private const string modGUID = "Croquette.BetterInventory";
        private const string modName = "Better Inventory";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static BetterInventory Instance;

        internal static ManualLogSource logger;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }

            logger = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            harmony.PatchAll(typeof(BetterInventory));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(EasyInventoryInputClass));
            harmony.PatchAll(typeof(GlobalFlashlightInputClass));
        }

        static ManualLogSource GetLogger()
        {
            return logger;
        }
    }
}
