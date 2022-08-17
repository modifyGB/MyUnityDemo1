using Bags;
using Enemy;
using GridSystem;
using Items;
using MySocket;
using Newtonsoft.Json;
using Place;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Manager
{
    public struct ArchiveData
    {
        public string name;
        public int width;
        public int height;
        public int seed;
        public PlayerMessage Player;
        public GridXZ.Serialization GridXZ;
        public MapData[,] mapDataList;
        public ArchiveData(ArchiveData archive)
        {
            name = archive.name;
            width = archive.width;
            height = archive.height;
            seed = archive.seed;
            Player = PlayerManager.I.ToSerialization();
            GridXZ = MapManager.I.grid.ToSerialization();
            mapDataList = null;
        }
    }

    public struct MapData
    {
        
        public GridObject.Serialization[,] GridArray;
        public List<PlaceObject.Serialization> PlaceList;
        public List<EnemyObject.Serialization> EnemyList;
        public List<Chest.ChestSerialization> ChestList;
        public MapData(int blockWidth, int blockHeight)
        {
            GridArray = MapManager.I.grid.GridToSerialization(blockWidth, blockHeight);
            EnemyList = EnemyManager.I.DumpEnemy(blockWidth, blockHeight);
            ChestList = MapManager.I.DumpChest(blockWidth, blockHeight);
            PlaceList = MapManager.I.DumpPlace(blockWidth, blockHeight);
        }
    }

    public class GameManager : Singleton<GameManager>
    {
        [Header("资源")]
        public ItemTableSO itemTableSO;
        public PlaceTableSO placeTableSO;
        public EnemyTableSO enemyTableSO;
        public MakeItemTableSO makeItemTableSO;
        [Header("其他")]
        public bool Debug = false;
        public string archiveName;

        private ArchiveData archiveData;
        public ArchiveData ArchiveData { get { return archiveData; } }
        private Dictionary<KeyCode, bool> inputDic = new Dictionary<KeyCode, bool>();
        private Dictionary<MakeType, List<MakeItemSO>> makeTypeDic = new Dictionary<MakeType, List<MakeItemSO>>();
        public Dictionary<MakeType, List<MakeItemSO>> MakeTypeDic { get { return makeTypeDic; } }

        public override void Awake()
        {
            base.Awake();

            SoundManager.I.ChangeBackground(1);

            inputDic.Add(KeyCode.V, false);
            foreach (MakeType type in Enum.GetValues(typeof(MakeType)))
                makeTypeDic.Add(type, new List<MakeItemSO>());
            foreach (var makeItem in makeItemTableSO.table)
            {
                if (makeItem == null) continue;
                makeTypeDic[makeItem.makeType].Add(makeItem);
            }

            LoadArchive();
        }

        private void Start()
        {
            //PlayerManager.I.PlayerBag.Bag.AddBag(new Item.Serialization(10));
            //PlayerManager.I.PlayerBag.Bag.AddBag(new Item.Serialization(11));
            //PlayerManager.I.PlayerBag.Bag.AddBag(new Item.Serialization(12));
            //PlayerManager.I.PlayerBag.Bag.AddBag(new Item.Serialization(13));
            //PlayerManager.I.PlayerBag.Bag.AddBag(new Item.Serialization(4, 10));
            //enemyTableSO.table[1].Create(new Vector3(50, 0, 60), Vector3.zero);
        }

        private void Update()
        {
            for (int i = 0; i < inputDic.Count; i++)
            {
                var item = inputDic.ElementAt(i);
                if (!inputDic[item.Key])
                    inputDic[item.Key] = Input.GetKeyDown(item.Key);
            }

            UpdateArchive();
        }

        private void FixedUpdate()
        {
            if (inputDic[KeyCode.V])
            {
                SaveArchive();
                inputDic[KeyCode.V] = false;
            }
        }

        //保存存档
        public void SaveArchive()
        {
            var saveObject = new ArchiveData(archiveData);
            if (Debug)
                Utils.SaveObjectAsJson("Archive/" + archiveName + "/archiveData.json", saveObject);
            else
                Utils.SaveObjectAsJson("Archive/" + World.archiveName + "/archiveData.json", saveObject);

            for (int i = 0; i < archiveData.mapDataList.GetLength(0); i++)
                for (int j = 0; j < archiveData.mapDataList.GetLength(1); j++)
                    if (archiveData.mapDataList[i, j].GridArray != null)
                        SaveBlock(i, j);
            print("save success");
        }
        //保存Block
        public void SaveBlock(int blockWidth, int blockHeight)
        {
            string name;
            if (Debug)
                name = archiveName;
            else
                name = World.archiveName;
            var mapData = new MapData(blockWidth, blockHeight);
            Utils.SaveObjectAsJson("Archive/" + name + "/archiveData.json", new ArchiveData(archiveData));
            Utils.SaveObjectAsJson("Archive/" + name + "/" + blockWidth + "-" + blockHeight + ".json", mapData);
        }
        //加载存档
        public void LoadArchive()
        {
            string name;
            if (Debug)
                name = archiveName;
            else
                name = World.archiveName;
            archiveData = Utils.LoadObject<ArchiveData>("Archive/" + name + "/archiveData.json");
            var blockWidth = Mathf.CeilToInt(archiveData.width / 100f);
            var blockHeight = Mathf.CeilToInt(archiveData.height / 100f);
            archiveData.mapDataList = new MapData[blockWidth, blockHeight];

            int x = (int)(archiveData.Player.position[0] / 100);
            int y = (int)(archiveData.Player.position[2] / 100);
            for (int i = x - 1; i <= x + 1; i++)
                for (int j = y - 1; j <= y + 1; j++)
                {
                    var mapData = Utils.LoadObject<MapData>("Archive/" + name + "/" + i + "-" + j + ".json");
                    if (mapData.GridArray == null)
                        continue;
                    archiveData.mapDataList[i, j] = mapData;
                }
        }
        //更新存档
        public void UpdateArchive()
        {
            int x = (int)(PlayerManager.Player.transform.position.x / 100);
            int y = (int)(PlayerManager.Player.transform.position.z / 100);
            var gridArrayList = MapManager.I.grid.GridArrayList;
            for (int i = Mathf.Clamp(x - 2, 0, gridArrayList.GetLength(0) - 1); i <= Mathf.Clamp(x + 2, 0, gridArrayList.GetLength(0) - 1); i++)
                for (int j = Mathf.Clamp(y - 2, 0, gridArrayList.GetLength(1) - 1); j <= Mathf.Clamp(y + 2, 0, gridArrayList.GetLength(1) - 1); j++)
                {
                    if (i == x - 2 || i == x + 2 || j == y - 2 || j == y + 2)
                    {
                        if (archiveData.mapDataList[i, j].GridArray != null)
                        {
                            SaveBlock(i, j);
                            MapManager.I.DeleteBlock(i, j);
                            archiveData.mapDataList[i, j] = default;
                        }
                    }
                    else
                    {
                        if (archiveData.mapDataList[i, j].GridArray == null)
                        {
                            string name;
                            if (Debug)
                                name = archiveName;
                            else
                                name = World.archiveName;
                            var mapData = Utils.LoadObject<MapData>("Archive/" + name + "/" + i + "-" + j + ".json");
                            archiveData.mapDataList[i, j] = mapData;
                            MapManager.I.LoadBlock(i, j);
                         }
                    }
                }
        }
    }
}
