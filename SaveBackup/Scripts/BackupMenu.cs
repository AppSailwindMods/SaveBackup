using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveBackup.Scripts
{
    internal class BackupMenu : MonoBehaviour
    {
        public StartMenu startMenu;

        public GameObject backupListUI => BackupMenuData.backupListUI;

        public int currentSlot;

        public static int currentBackup;

        public void ButtonClick(int numSlot)
        {
            if (!SaveSlots.slotsActive[numSlot]) return;
            currentSlot = numSlot;
            BackupMenuData.SetupBackupSlots(currentSlot);
            backupListUI.SetActive(true);
            gameObject.SetActive(false);
        }

        public void BackupClick(int backupNum)
        {
            var filePath = SaveSlots.GetSlotSavePath(currentSlot);
            if (backupNum > 0 && !File.Exists(filePath + backupNum)) return;
            currentBackup = backupNum;
            SaveSlots.currentSlot = currentSlot;
            backupListUI.SetActive(false);
            startMenu.InvokePrivateMethod("LoadGame");
        }

        public void ButtonClick(StartMenuButtonType type)
        {
            switch (type)
            {
                case StartMenuButtonType.Back:
                    {
                        UISoundPlayer.instance.PlayUISound(UISounds.buttonBack, 1f, 1.1f);
                        gameObject.SetActive(false);
                        startMenu.InvokePrivateMethod("EnableSlotMenu");
                    }
                    break;
                case StartMenuButtonType.Quit:
                    {
                        currentSlot = -1;
                        UISoundPlayer.instance.PlayUISound(UISounds.buttonBack, 1f, 1.1f);
                        backupListUI.SetActive(false);
                        gameObject.SetActive(true);
                    }
                    break;
            }
        }
    }
}
