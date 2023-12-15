using HarmonyLib;
using SailwindModdingHelper;
using SaveBackup.Scripts;
using System.IO;
using UnityEngine;

namespace SaveBackup.Patches
{
    static class BackupPatches
    {
        [HarmonyPatch(typeof(Sun), "Update")]
        internal static class UpdatePatch
        {
            internal static float timer;
            internal static bool autosave;

            [HarmonyPostfix]
            public static void Postfix()
            {
                if (!SaveBackupMain.instance.autoSaveEnabled.Value) return;
                if (!GameState.playing && !SaveLoadManager.readyToSave) return;
                if (Utilities.GamePaused) return;
                timer += Time.unscaledDeltaTime;
                if(timer >= SaveBackupMain.instance.autosaveTimer.Value * 60f)
                {
                    timer = 0;
                    autosave = true;
                    SaveLoadManager.instance.SaveGame(true);
                }
            }
        }

        [HarmonyPatch(typeof(SaveLoadManager), "SaveGame")]
        private static class SaveGamePatch
        {
            [HarmonyPrefix]
            public static void Prefix()
            {
                ModLogger.LogInfo(SaveBackupMain.instance.Info, "Creating backup...");
                var filePath = SaveSlots.GetCurrentSavePath();
                var modPath = ModSave.GetSaveDirectory(SaveSlots.currentSlot);
                for (int i = SaveBackupMain.instance.saveBackupCount.Value; i > 0; i--)
                {
                    if (i == SaveBackupMain.instance.saveBackupCount.Value)
                    {
                        if (File.Exists(filePath + i))
                        {
                            File.Delete(filePath + i);
                        }

                        if(Directory.Exists(modPath + i))
                        {
                            Directory.Delete(modPath + i, true);
                        }
                    }
                    else
                    {
                        if (File.Exists(filePath + i))
                        {
                            File.Move(filePath + i, filePath + (i + 1));
                        }

                        if (Directory.Exists(modPath + i))
                        {
                            Directory.Move(modPath + i, modPath + (i + 1));
                        }
                    }
                }
                if (File.Exists(filePath))
                {
                    File.Move(filePath, filePath + 1);
                }

                if (Directory.Exists(modPath))
                {
                    Directory.Move(modPath, modPath + 1);
                }

                if (UpdatePatch.autosave)
                {
                    UpdatePatch.autosave = false;
                    NotificationUi.instance.ShowNotification("Autosave");
                }
            }
        }

        [HarmonyPatch(typeof(SaveSlots), "GetCurrentSavePath")]
        private static class GetCurrentSavePathPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref string __result)
            {
                if (BackupMenu.currentBackup > 0)
                {
                    __result += BackupMenu.currentBackup;
                }
            }
        }

        [HarmonyPatch(typeof(ModSave), "GetSaveDirectory")]
        private static class GetSaveDirectoryPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref string __result)
            {
                if (BackupMenu.currentBackup > 0)
                {
                    __result += BackupMenu.currentBackup;
                }
            }
        }
    }
}
