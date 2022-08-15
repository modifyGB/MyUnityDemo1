using Bags;
using Cinemachine;
using Items;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Manager
{
    public enum PlayerState { Move, Attack, Die, GetHit }

    public struct PlayerMessage
    {
        public float nowBlood;
        public float experience;
        public float[] position;
        public int bagCapacity;
        public List<Item.Serialization> bag;
        public PlayerMessage(float nowBlood, float experience, float[] position, int bagCapacity, List<Item.Serialization> bag)
        { this.nowBlood = nowBlood; this.experience = experience; this.position = position; this.bagCapacity = bagCapacity; this.bag = bag; }
        public PlayerMessage(float[] position)
        {this.nowBlood = 100; this.experience = 0; this.position = position; this.bagCapacity = 8; this.bag = new List<Item.Serialization>();}
    }

    public class PlayerManager : Singleton<PlayerManager>
    {
        [Header("人物")]
        public GameObject PlayerPrefab;
        [Header("数值")]
        public float walkSpeed = 5f;
        public float runSpeedPlus = 5f;
        public float jumpSpeed = 5f;
        [Header("设置")]
        public bool isBug = false;

        private static GameObject player;
        public static GameObject Player
        {
            get { return player; }
            set
            {
                if (player == null)
                    player = value;
                else if (player != value)
                    Destroy(value);
            }
        }
        private Transform shieldTransform;
        public Transform ShieldTransform { get { return shieldTransform; } }
        private Transform weaponTransform;
        public Transform WeaponTransform { get { return weaponTransform; } }
        private Transform centerPoint;
        public Transform CenterPoint { get { return centerPoint; } }
        private Transform throwPoint;
        public Transform ThrowPoint { get { return throwPoint; } }
        private WeaponObject shield = null;
        public WeaponObject Shield
        { get { return shield; } set { shield = value; } }
        private WeaponObject weapon = null;
        public WeaponObject Weapon
        {
            get { return weapon; }
            set
            {
                if (weapon != null)
                    weapon.DestroySelf();
                if (value != null)
                    value.SetTransform(WeaponTransform);
                weapon = value;
            }
        }

        private PlayerMovement playerMovement;
        public PlayerMovement PlayerMovement { get { return playerMovement; } }
        private PlayerAttack playerAttack;
        public PlayerAttack PlayerAttack { get { return playerAttack; } }
        private PlayerValue playerValue;
        public PlayerValue PlayerValue { get { return playerValue; } }
        private PlayerBag playerBag;
        public PlayerBag PlayerBag { get { return playerBag; } }

        private PlayerState playerState = PlayerState.Move;
        public PlayerState PlayerState { get { return playerState; } set { playerState = value; } }
        public bool isWeapon { get { return weapon != null; } }

        public PlayerMessage ToSerialization()
        {
            var nowBlood = playerValue.NowBlood;
            var experience = playerValue.Experience;
            var bagCapacity = PlayerBag.Bag.bagCapacity;
            var position = new float[3] { Player.transform.position.x,
                Player.transform.position.y, Player.transform.position.z };
            var bag = new List<Item.Serialization>();
            foreach (var item in PlayerBag.Bag.ItemList)
            {
                if (item == null)
                    continue;
                bag.Add(item.ToSerialization());
            }
            return new PlayerMessage(nowBlood, experience, position, bagCapacity, bag);
        }

        public override void Awake()
        {
            base.Awake();

            var p = GameManager.I.ArchiveObject.Player;
            var po = new Vector3(p.position[0], p.position[1], p.position[2]);
            Player = Instantiate(PlayerPrefab, po, Quaternion.identity);

            shieldTransform = Utils.FindChildByName(Player, "Shield").transform;
            weaponTransform = Utils.FindChildByName(Player, "Weapon").transform;
            centerPoint = Utils.FindChildByName(Player, "Center").transform;
            throwPoint = Utils.FindChildByName(Player, "Throw").transform;
            playerMovement = Player.GetComponent<PlayerMovement>();
            playerAttack = Player.GetComponent<PlayerAttack>();
            playerValue = Player.GetComponent<PlayerValue>();
            playerBag = Player.GetComponent<PlayerBag>();
        }

        private void Start()
        {
            
        }

        void Update()
        {

        }

        public void WaterCheck()
        {
            var lastMakeType = UIManager.I.MakeTable.MakeTypeOpenList[MakeType.Water];
            var gridObject = MapManager.I.grid.GetGridObject(player.transform.position);
            if (gridObject.gridEnvironment == GridSystem.GridEnvironment.WATER)
                UIManager.I.MakeTable.MakeTypeOpenList[MakeType.Water] = true;
            else
                UIManager.I.MakeTable.MakeTypeOpenList[MakeType.Water] = false;
            if (lastMakeType != UIManager.I.MakeTable.MakeTypeOpenList[MakeType.Water])
                UIManager.I.MakeTable.MakeItemCheck();
        }
    }
}
