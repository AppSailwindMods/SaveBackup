using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveBackup.Scripts
{
    internal class GPBackupMenuButton : GoPointerButton
    {
        public StartMenu startMenu;
        public BackupMenu backupMenu;

        public override void OnActivate()
        {
            if (SaveSlots.activeSlotsCount == 0) return;
            UISoundPlayer.instance.PlayUISound(UISounds.buttonBack, 1f, 1.1f);
            startMenu.InvokePrivateMethod("DisableSlotMenu");
            backupMenu.gameObject.SetActive(true);
        }
    }
}
