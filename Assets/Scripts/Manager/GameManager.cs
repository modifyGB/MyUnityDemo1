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
    public class GameManager : Singleton<GameManager>
    {
        [Header("初始化设置")]
        public int mapWidth = 100;
        public int mapHeight = 100;
        public int seed;
        public bool isInitialize = false;
        [Header("柏林噪声设置")]
        public float cellSize = 1;
        public Vector3 StartOrigin = Vector3.zero;
        public float scale = 20;
        public int octaves = 1;
        public float persistance = 0.5f;
        public float lacunarity = 0;
        [Header("资源")]
        public ItemTableSO itemTableSO;
        public PlaceTableSO placeTableSO;
        public EnemyTableSO enemyTableSO;
        public MakeItemTableSO makeItemTableSO;
        [Header("存档")]
        public int archive = 1;

        private GameData archiveObject;
        public GameData ArchiveObject { get { return archiveObject; } }
        private Dictionary<KeyCode, bool> inputDic = new Dictionary<KeyCode, bool>();
        private Dictionary<MakeType, List<MakeItemSO>> makeTypeDic = new Dictionary<MakeType, List<MakeItemSO>>();
        public Dictionary<MakeType, List<MakeItemSO>> MakeTypeDic { get { return makeTypeDic; } }

        public struct GameData
        {
            public GridXZ.Serialization GridXZ;
            public PlayerMessage Player;
            public List<PlaceObject.Serialization> PlaceList;
            public List<EnemyObject.Serialization> EnemyList;
            public List<Chest.ChestSerialization> ChestList;
            public GameData(int x) 
            { 
                Player = PlayerManager.I.ToSerialization(); 
                GridXZ = MapManager.I.grid.ToSerialization();
                EnemyList = EnemyManager.I.FindAllEnemy(); 
                ChestList = MapManager.I.DumpChest();
                PlaceList = MapManager.I.DumpPlace();
            }
        }

        public override void Awake()
        {
            base.Awake();

            inputDic.Add(KeyCode.V, false);
            foreach (var makeItem in makeItemTableSO.table)
            {
                if (makeItem == null) continue;
                if (!makeTypeDic.ContainsKey(makeItem.makeType))
                    makeTypeDic[makeItem.makeType] = new List<MakeItemSO>();
                makeTypeDic[makeItem.makeType].Add(makeItem);
            }

            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Archive")))
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Archive"));

            if (isInitialize)
                CreateWorld(seed, mapWidth, mapHeight);
            else
                LoadArchive();
        }

        private void Start()
        {
            //PlayerManager.I.PlayerBag.Bag.AddBag(new Item.Serialization(1, 64));
            //PlayerManager.I.PlayerBag.Bag.AddBag(new Item.Serialization(2));
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
            var saveObject = new GameData(0);
            Utils.SaveObjectAsJson("Archive/" + Convert.ToString(archive) + ".json", saveObject);
            print("save success");
        }
        //加载存档
        public void LoadArchive()
        {
            archiveObject = Utils.LoadObject<GameData>("Archive/" + Convert.ToString(archive) + ".json");
        }
        //创建世界
        public void CreateWorld(int seed, int mapWidth, int mapHeight)
        {
            //初始化
            archiveObject = default(GameData);
            archiveObject.Player = new PlayerMessage(100, 0, new float[3] 
            { mapWidth / 2, 0, mapHeight / 2 }, 8, new List<Item.Serialization>());
            archiveObject.GridXZ = new GridXZ.Serialization(mapWidth, mapHeight, cellSize, StartOrigin);
            archiveObject.EnemyList = new List<EnemyObject.Serialization>();
            archiveObject.ChestList = new List<Chest.ChestSerialization>();
            archiveObject.PlaceList = new List<PlaceObject.Serialization>();
            //环境初始化
            var map = Utils.CreateNoiseMap(mapWidth, mapHeight, scale, octaves, persistance, lacunarity, seed);
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] >= 0.5) archiveObject.GridXZ.GridArray[i, j] = new GridObject.Serialization(GridEnvironment.GRASS);
                    else if (map[i, j] >= 0.4) archiveObject.GridXZ.GridArray[i, j] = new GridObject.Serialization(GridEnvironment.SOIL);
                    else archiveObject.GridXZ.GridArray[i, j] = new GridObject.Serialization(GridEnvironment.WATER);
                }
            //物体生成
            var ll = Utils.GetRandomPoint2(mapWidth, mapWidth / 10, mapHeight, mapHeight / 10);
            foreach (var x in ll[0])
                foreach (var z in ll[1])
                    CreatePlace(x, z);
        }
        //物体生成逻辑
        public void CreatePlace(int x, int z)
        {
            var gridObject = archiveObject.GridXZ.GridArray[x, z];
            if (gridObject.ge == GridEnvironment.GRASS)
                archiveObject.PlaceList.Add(
                    new PlaceObject.Serialization(1, new Vector2Int(x, z), Dir.Down));
        }
    }
}
