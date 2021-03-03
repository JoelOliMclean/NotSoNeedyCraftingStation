using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace NotSoNeedyCraftingStation
{
    [BepInPlugin("uk.co.oliapps.valheim.notsoneedycraftingstation", "Not So Needy Crafting Station", "0.0.1")]
    public class NotSoNeedyCraftingStation : BaseUnityPlugin
    {
        public void Awake() => Harmony.CreateAndPatchAll(typeof(NotSoNeedyCraftingStation), null);

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