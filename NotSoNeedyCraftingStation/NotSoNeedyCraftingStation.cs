using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace NotSoNeedyCraftingStation
{
    [BepInPlugin("uk.co.oliapps.valheim.notsoneedycraftingstation", "Not So Needy Crafting Station", "1.1.0")]
    public class NotSoNeedyCraftingStation : BaseUnityPlugin
    {
        private static ConfigEntry<bool> disableWeatherDamageForCraftingStation;

        public void Awake()
        {
            disableWeatherDamageForCraftingStation = Config.Bind<bool>("General", "Disable Rain Damage", false, "Disables rain damage for the crafting station");
            Config.Save();
            Harmony.CreateAndPatchAll(typeof(NotSoNeedyCraftingStation), null);
        }
        
        [HarmonyPatch(typeof(CraftingStation), "Start")]
        [HarmonyPostfix]
        public static void CraftingStation_Start(ref CraftingStation __instance)
        {
            if (disableWeatherDamageForCraftingStation.Value)
            {
                WearNTear wearNTear = __instance.GetComponent<WearNTear>();
                if (wearNTear)
                {
                    ZLog.Log("[Not So Needy Crafting Station] Disabling rain damage");
                    wearNTear.m_noRoofWear = false;
                }
            }
        }

        [HarmonyPatch(typeof(CraftingStation), "CheckUsable")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        public static bool CheckUsable(ref CraftingStation __instance, Player player, bool showMessage, ref bool __result)
        {
            bool haveFire = (bool)AccessTools.Field(typeof(CraftingStation), "m_haveFire").GetValue(__instance);
            if (!__instance.m_craftRequireFire || haveFire)
            {
                __result = true;
                return false;
            }
            if (showMessage)
                player.Message(MessageHud.MessageType.Center, "$msg_needfire", 0, (Sprite)null);
            __result = false;
            return false;
        }
    }
}