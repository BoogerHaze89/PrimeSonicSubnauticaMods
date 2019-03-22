﻿namespace MoreCyclopsUpgrades.Patchers
{
    using Managers;
    using Harmony;

    [HarmonyPatch(typeof(CyclopsUpgradeConsoleHUDManager))]
    [HarmonyPatch("RefreshScreen")]
    internal class CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref CyclopsUpgradeConsoleHUDManager __instance)
        {
            CyclopsHUDManager hudMgr = CyclopsManager.GeHUDManager(__instance.subRoot);

            if (hudMgr == null)
            {
                return true;
            }

            hudMgr.UpdateConsoleHUD(__instance);

            return false;
        }
    }
}
