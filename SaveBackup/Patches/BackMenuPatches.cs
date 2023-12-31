using HarmonyLib;
using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveBackup.Patches
{
    internal static class BackMenuPatches
    {
        [HarmonyPatch(typeof(StartMenu), "EnableSlotMenu")]
        private static class DisableSlotMenuPatch
        {
            [HarmonyPostfix]
            public static void Postfix(StartMenu __instance)
            {
                BackupMenuData.saveSlotUILabelText.text = __instance.GetPrivateField<bool>("selectedContinue") ? "Continue\n" : "New Game\n";
                BackupMenuData.backupMenuButton.SetActive(__instance.GetPrivateField<bool>("selectedContinue"));
            }
        }

        /*[HarmonyPatch(typeof(StartMenu), "ButtonClick", typeof(int))]
        private static class ButtonClickPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(int slotNumber, int ___animsPlaying)
            {
                if (___animsPlaying > 0) return false;

                if (!BackupMenuData.backupMenuOpened) return true;

                if (!SaveSlots.slotsActive[slotNumber]) return false;

                return false;
            }
        }*/
    }
}
