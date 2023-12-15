using cakeslice;
using SailwindModdingHelper;
using SaveBackup.Scripts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveBackup
{
    internal static class BackupMenuData
    {
        public static GameObject backupSlotUI;

        public static GameObject backupMenuButton;

        public static GameObject backupButtonPrefab;

        public static GameObject backupListUI;

        public static void Setup()
        {
            SetupButton();
        }

        private static void SetupButton()
        {

            StartMenu startMenu = Object.FindObjectOfType<StartMenu>();
            if (startMenu)
            {
                var startUI = startMenu.GetPrivateField<GameObject>("startUI");
                if(startUI)
                {
                    backupListUI = GameObject.Instantiate(startUI.gameObject);
                    backupListUI.name = "Backup list ui";
                    backupListUI.transform.parent = startMenu.transform;

                    backupListUI.transform.localScale = Vector3.one;
                    backupListUI.transform.localRotation = Quaternion.identity;
                    backupListUI.transform.localPosition = new Vector3(-0.0001f, 1.5f, 2.4f);

                    foreach(Transform transform in backupListUI.transform)
                    {
                        if(transform.name != "bg")
                        {
                            GameObject.Destroy(transform.gameObject);
                        }
                        else
                        {
                            transform.localScale *= 1.5f;
                        }
                    }

                    backupListUI.SetActive(false);

                    foreach (Transform transform in startUI.transform)
                    {
                        if(transform.name == "button new game")
                        {
                            backupButtonPrefab = transform.gameObject;
                        }
                    }
                    /*foreach(Transform transform in startUI.transform)
                    {
                        if(transform.name == "button new game")
                        {
                            var button = GameObject.Instantiate(transform.gameObject);
                            button.name = "button backups";
                            button.transform.parent = startUI.transform;
                            button.transform.localScale = transform.localScale;
                            button.transform.rotation = transform.rotation;
                            button.transform.localPosition = transform.localPosition - new Vector3(0, 0.3f, 0);
                            var background = button.GetComponentInChildren<StartMenuButton>().gameObject;
                            GameObject.Destroy(button.GetComponentInChildren<StartMenuButton>());
                            GameObject.Destroy(background.GetComponentInChildren<Outline>());
                            background.AddComponent<GPBackupMenuButton>().startMenu = startMenu;
                            button.GetComponentInChildren<TextMesh>().text = "Backups";
                        }
                        else if(transform.name == "button settings")
                        {
                            transform.localPosition -= new Vector3(0, 0.3f, 0);
                        }
                    }*/
                }
                var saveSlotUI = startMenu.GetPrivateField<GameObject>("saveSlotUI");
                if (saveSlotUI)
                {
                    // Create backup UI
                    backupSlotUI = GameObject.Instantiate(saveSlotUI.gameObject);
                    backupSlotUI.name = "backup slot ui";
                    backupSlotUI.transform.parent = startMenu.transform;
                    backupSlotUI.transform.position = saveSlotUI.transform.position;
                    backupSlotUI.transform.rotation = saveSlotUI.transform.rotation;
                    backupSlotUI.transform.localScale = saveSlotUI.transform.localScale;
                    backupSlotUI.AddComponent<BackupMenu>().startMenu = startMenu;
                    backupSlotUI.SetActive(false);

                    foreach(var button in backupSlotUI.GetComponentsInChildren<StartMenuButton>())
                    {
                        int slot = button.GetPrivateField<int>("saveSlot");
                        StartMenuButtonType type = button.GetPrivateField<StartMenuButtonType>("type");
                        GameObject.Destroy(button);
                        GameObject.Destroy(button.GetComponent<Outline>());

                        var backupButton = button.gameObject.AddComponent<GPBackupUIButton>();
                        backupButton.backupMenu = backupSlotUI.GetComponent<BackupMenu>();
                        backupButton.type = type;
                        if (type == StartMenuButtonType.Slot)
                        {
                            backupButton.slotNumber = slot;
                        }
                    }

                    {
                        foreach (Transform transform in saveSlotUI.transform)
                        {
                            if (transform.name == "button back (1)")
                            {
                                transform.localPosition -= new Vector3(0, 0.1f, 0);

                                backupMenuButton = GameObject.Instantiate(transform.gameObject);
                                backupMenuButton.name = "button backup";
                                backupMenuButton.transform.parent = saveSlotUI.transform;

                                {
                                    var position = transform.localPosition;
                                    position.y += 0.3f;

                                    backupMenuButton.transform.localPosition = position;
                                }
                                backupMenuButton.transform.rotation = transform.rotation;
                                backupMenuButton.transform.localScale = transform.localScale;

                                var background = backupMenuButton.GetComponentInChildren<StartMenuButton>().gameObject;
                                GameObject.Destroy(background.GetComponentInChildren<StartMenuButton>());
                                GameObject.Destroy(background.GetComponentInChildren<Outline>());
                                var backupButton = background.AddComponent<GPBackupMenuButton>();
                                backupButton.startMenu = startMenu;
                                backupButton.backupMenu = backupSlotUI.GetComponent<BackupMenu>();

                                {
                                    var texts = backupMenuButton.GetComponentsInChildren<TextMesh>();
                                    texts[0].text = "backups";
                                    var position = texts[0].transform.localPosition;
                                    position.y = 0.025f;
                                    texts[0].transform.localPosition = position;
                                    GameObject.Destroy(texts[1]);
                                }
                            }
                        }
                    }

                    {
                        foreach (Transform transform in saveSlotUI.transform)
                        {
                            if (transform.name == "button back (1)")
                            {
                                var backupMenuButton = GameObject.Instantiate(transform.gameObject);
                                backupMenuButton.name = "button backup";
                                backupMenuButton.transform.parent = backupListUI.transform;

                                backupMenuButton.transform.position = transform.position;
                                backupMenuButton.transform.rotation = transform.rotation;
                                backupMenuButton.transform.localScale = transform.localScale;

                                var background = backupMenuButton.GetComponentInChildren<StartMenuButton>().gameObject;
                                GameObject.Destroy(background.GetComponentInChildren<StartMenuButton>());
                                GameObject.Destroy(background.GetComponentInChildren<Outline>());
                                var backupButton = background.gameObject.AddComponent<GPBackupUIButton>();
                                backupButton.backupMenu = backupSlotUI.GetComponent<BackupMenu>();
                                backupButton.type = StartMenuButtonType.Quit;
                            }
                        }
                    }
                }
            }
        }

        public static void SetupBackupSlots(int numSlot)
        {
            foreach (var button in backupListUI.GetComponentsInChildren<GPBackupUIButton>())
            {
                if (button.type == StartMenuButtonType.RegionConfirm)
                {
                    GameObject.Destroy(button.transform.parent.gameObject);
                }
            }

            var filePath = SaveSlots.GetSlotSavePath(numSlot);

            CreateBackupButtonList(filePath, "Load Save", 0);

            for (int i = 1; i <= SaveBackupMain.instance.saveBackupCount.Value; i++)
            {
                if (File.Exists(filePath + i))
                {
                    CreateBackupButtonList(filePath + i, $"Load Backup {i}", i);
                }
            }
        }

        private static void CreateBackupButtonList(string fileLocation, string buttonText, int saveCount)
        {
            var lastWriteTime = File.GetLastWriteTime(fileLocation);
            string text = lastWriteTime.ToShortTimeString() + "\n" + lastWriteTime.ToShortDateString();

            var button = GameObject.Instantiate(backupButtonPrefab);
            button.transform.parent = backupListUI.transform;
            button.transform.localPosition = new Vector3(0, 1.4f - (0.3f * saveCount), 0);
            button.transform.localScale = Vector3.one;
            button.transform.localRotation = Quaternion.Euler(0, 180, 0);

            var background = button.GetComponentInChildren<StartMenuButton>().gameObject;
            GameObject.Destroy(background.GetComponent<StartMenuButton>());
            GameObject.Destroy(background.GetComponent<Outline>());
            var scale = background.transform.localScale;
            scale.x *= 2;
            background.transform.localScale = scale;

            var buttonData = background.AddComponent<GPBackupUIButton>();
            buttonData.backupMenu = backupSlotUI.GetComponent<BackupMenu>();
            buttonData.slotNumber = saveCount;
            buttonData.type = StartMenuButtonType.RegionConfirm;

            button.GetComponentInChildren<TextMesh>().text = buttonText + " - " + text;
        }
    }
}
