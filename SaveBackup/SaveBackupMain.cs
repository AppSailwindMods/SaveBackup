using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SailwindModdingHelper;
using SaveBackup.Patches;
using SaveBackup.Scripts;
using System.Reflection;
using UnityEngine;

namespace SaveBackup
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(SailwindModdingHelperMain.GUID, "2.0.3")]
    public class SaveBackupMain : BaseUnityPlugin
    {
        public const string GUID = "com.app24.savebackup";
        public const string NAME = "Save Tweaks";
        public const string VERSION = "1.0.0";

        internal static ManualLogSource logSource;

        internal static SaveBackupMain instance;

        private ConfigEntry<KeyboardShortcut> quickSaveKey;
        internal ConfigEntry<int> saveBackupCount;
        internal ConfigEntry<float> autosaveTimer;
        internal ConfigEntry<bool> autoSaveEnabled;

        private void Awake()
        {
            instance = this;
            logSource = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);

            quickSaveKey = Config.Bind("Hotkeys", "Quick Save", new KeyboardShortcut(KeyCode.F8));
            autoSaveEnabled = Config.Bind("Values", "Enable Autosave", true);
            saveBackupCount = Config.Bind("Values", "Save Backup Count", 3, new ConfigDescription("", new AcceptableValueRange<int>(1, 10)));
            autosaveTimer = Config.Bind("Values", "Autosave Timer", 30f, new ConfigDescription("Timer is measured in minutes"));

            if(autosaveTimer.Value < 0.01f)
            {
                autosaveTimer.Value = 0.01f;
            }

            autosaveTimer.SettingChanged += (_, __) =>
            {
                if (autosaveTimer.Value < 0.01f)
                {
                    autosaveTimer.Value = 0.01f;
                }
            };

            GameEvents.OnPlayerInput += (_, __) =>
            {
                if (quickSaveKey.Value.IsDown() && SaveLoadManager.readyToSave)
                {
                    SaveLoadManager.instance.SaveGame(true);
                    NotificationUi.instance.ShowNotification("Game Saved!");
                }
            };

            GameEvents.OnGameSave += (_, __) =>
            {
                BackupPatches.UpdatePatch.timer = 0;
            };

            GameEvents.OnGameStart += (_, __) =>
            {
                BackupMenuData.Setup();
            };

            GameEvents.OnSaveLoadPost += (_, __) =>
            {
                BackupMenu.currentBackup = 0;
            };
        }
    }
}
