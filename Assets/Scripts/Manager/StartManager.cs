using Enemy;
using GridSystem;
using Items;
using Place;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UI;
using UnityEngine;

namespace Manager
{
    public class StartManager : Singleton<StartManager>
    {
        [Header("组件")]
        public ArchiveItem archiveItemPrefab;
        public CreateArchiveButton createArchiveButtonPrefab;
        public ArchiveTable archiveTablePrefab;
        public GameObject createArchivePrefab;
        public SettingTable settingTablePrefab;
        public LoadCanvas loadCanvasPrefab;
        public GameObject StartCanvas;

        private ArchiveTable archiveTable = null;
        public ArchiveTable ArchiveTable { get { return archiveTable; } }
        private GameObject createArchive = null;
        private SettingTable settingTable = null;
        private LoadCanvas loadCanvas = null;
        private List<string> fileList = new List<string>();
        public List<string> FileList { get { return fileList; } }

        public enum StartState { StartGame, StartCanvas, CreateArchive, ArchiveTable, SettingTable, Load }
        private StartState state = StartState.StartCanvas;
        public StartState State
        {
            get { return state; }
            set 
            {
                state = value; 
                if (value == StartState.StartCanvas)
                {
                    if (archiveTable != null) archiveTable.DestroySelf();
                    if (settingTable != null) settingTable.DestroySelf();
                    if (loadCanvas != null) loadCanvas.DestroySelf();
                    if (createArchive != null) Destroy(createArchive);
                    StartCanvas.SetActive(true);
                }
                else if (value == StartState.CreateArchive)
                {
                    if (archiveTable != null) archiveTable.DestroySelf();
                    if (settingTable != null) settingTable.DestroySelf();
                    if (loadCanvas != null) loadCanvas.DestroySelf();
                    if (createArchive == null) createArchive = Instantiate(createArchivePrefab);
                    StartCanvas.SetActive(false);
                }
                else if (value == StartState.ArchiveTable)
                {
                    if (archiveTable == null) archiveTable = Instantiate(archiveTablePrefab);
                    if (settingTable != null) settingTable.DestroySelf();
                    if (loadCanvas != null) loadCanvas.DestroySelf();
                    if (createArchive != null) Destroy(createArchive);
                    StartCanvas.SetActive(false);
                }
                else if (value == StartState.SettingTable)
                {
                    if (archiveTable != null) archiveTable.DestroySelf();
                    if (settingTable == null) settingTable = Instantiate(settingTablePrefab);
                    if (loadCanvas != null) loadCanvas.DestroySelf();
                    if (createArchive != null) Destroy(createArchive);
                    StartCanvas.SetActive(false);
                }
                else if (value == StartState.Load)
                {
                    if (archiveTable != null) archiveTable.DestroySelf();
                    if (settingTable != null) settingTable.DestroySelf();
                    if (loadCanvas == null) loadCanvas = Instantiate(loadCanvasPrefab);
                    if (createArchive != null) Destroy(createArchive);
                    StartCanvas.SetActive(false);
                }
                else
                {
                    if (archiveTable != null) archiveTable.DestroySelf();
                    if (settingTable == null) settingTable.DestroySelf();
                    if (loadCanvas != null) loadCanvas.DestroySelf();
                    if (createArchive != null) Destroy(createArchive);
                }
            }
        }

        public override void Awake()
        {
            base.Awake();

            SoundManager.I.ChangeBackground(0);
        }

        //更新存档列表
        public void UpdateArchiveList()
        {
            fileList.Clear();
            foreach (var file in Utils.FindFile("Archive"))
                if (file.Name.EndsWith(".list.json"))
                    fileList.Add(file.Name.Split(".")[0]);
        }
        //初始化Load
        public void LoadInitialization(ArchiveData archiveData)
        {
            State = StartState.Load;
            loadCanvas.Initialization(archiveData);
        }
        public void LoadInitialization(string archiveName)
        {
            State = StartState.Load;
            loadCanvas.Initialization(archiveName);
        }
        
    }
}
