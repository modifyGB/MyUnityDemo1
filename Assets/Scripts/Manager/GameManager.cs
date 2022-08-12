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
        [Header("资源")]
        public ItemTableSO itemTableSO;
        public PlaceTableSO placeTableSO;
        public EnemyTableSO enemyTableSO;
        public MakeItemTableSO makeItemTableSO;
        [Header("其他")]
        public bool Debug = false;
        public string archiveName;

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
                EnemyList = EnemyManager.I.DumpEnemy(); 
                ChestList = MapManager.I.DumpChest();
                PlaceList = MapManager.I.DumpPlace();
            }
        }

        public override void Awake()
        {
            base.Awake();

            inputDic.Add(KeyCode.V, false);
            foreach (MakeType type in Enum.GetValues(typeof(MakeType)))
                makeTypeDic.Add(type, new List<MakeItemSO>());
            foreach (var makeItem in makeItemTableSO.table)
            {
                if (makeItem == null) continue;
                makeTypeDic[makeItem.makeType].Add(makeItem);
            }

            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Archive")))
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Archive"));

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
            if (Debug)
                Utils.SaveObjectAsJson("Archive/" + archiveName + ".json", saveObject);
            else
                Utils.SaveObjectAsJson("Archive/" + Convert.ToString(World.archiveName) + ".json", saveObject);
            print("save success");
        }
        //加载存档
        public void LoadArchive()
        {
            if (Debug)
                archiveObject = Utils.LoadObject<GameData>("Archive/" + archiveName + ".json");
            else
                archiveObject = Utils.LoadObject<GameData>("Archive/" + Convert.ToString(World.archiveName) + ".json");
        }
    }
}
