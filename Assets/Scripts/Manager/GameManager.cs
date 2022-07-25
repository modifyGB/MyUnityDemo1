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
        [Header("存档")]
        public int archive = 1;

        private Dictionary<KeyCode, bool> inputDic = new Dictionary<KeyCode, bool>();

        public struct GameData
        {
            public GridXZ.Serialization GridXZ;
            public PlayerManager.PlayerMessage Player;
            public List<EnemyObject.Serialization> EnemyList;
            public GameData(GridXZ.Serialization GridXZ, PlayerManager.PlayerMessage Player, List<EnemyObject.Serialization> EnemyList) 
            { this.Player = Player; this.GridXZ = GridXZ; this.EnemyList = EnemyList; }
            public GameData(int x) { Player = PlayerManager.I.ToSerialization(); 
                GridXZ = MapManager.I.grid.ToSerialization(); EnemyList = EnemyManager.I.FindAllEnemy(); }
        }

        public override void Awake()
        {
            base.Awake();

            inputDic.Add(KeyCode.V, false);
            //链接1存档
            SocketManager.Socket.Send(Convert.ToString(archive), SocketSign.Connect, new AsyncCallback(ConnectCallBack));
        }

        private void Start()
        {
            //var bag = PlayerManager.I.PlayerBag.Bag;
            //bag.AddBag(new Item.Serialization(2, 0f));
            //bag.AddBag(new Item.Serialization(1, 64));

            //enemyTableSO.table[1].Create(new Vector3(10, 0, 10), new Vector3(0, 0, 0));
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
                SaveAll();
                inputDic[KeyCode.V] = false;
            }
        }

        //手动保存存档
        public void SaveAll()
        {
            var gameData = new GameData(0);
            SocketManager.Socket.Send(JsonConvert.SerializeObject(gameData), SocketSign.SaveAll);
            print("save success");
        }

        public void ConnectCallBack(IAsyncResult ar)
        {
            var message = SocketManager.Socket.ReceiveCallback(ar);
            if (message == null)
                return;

            print(message.message);
        }
    }
}
