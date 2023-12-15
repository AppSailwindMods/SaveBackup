using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveBackup.Scripts
{
    internal class GPBackupUIButton : GoPointerButton
    {
        public BackupMenu backupMenu;
        public int slotNumber;
        public StartMenuButtonType type;

        public override void OnActivate()
        {
            if (type == StartMenuButtonType.Slot)
            {
                backupMenu.ButtonClick(slotNumber);
            }
            else if (type == StartMenuButtonType.RegionConfirm)
            {
                backupMenu.BackupClick(slotNumber);
            }
            else
            {
                backupMenu.ButtonClick(type);
            }
        }
    }
}
