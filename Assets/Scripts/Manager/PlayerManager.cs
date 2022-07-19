using Cinemachine;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Manager
{
    public enum PlayerState { Move, Attack, Die, GetHit }

    public class PlayerManager : Singleton<PlayerManager>
    {
        [Header("人物")]
        public GameObject PlayerPrefab;
        [Header("数值")]
        public float walkSpeed = 5f;
        public float runSpeedPlus = 5f;
        public int bagCapacity = 8;
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

        public struct PlayerMessage
        {

        }

        //序列化
        public PlayerMessage ToSerialization()
        {
            return default;
        }

        public override void Awake()
        {
            base.Awake();

            Player = Instantiate(PlayerPrefab, new Vector3(5, 1, 5), Quaternion.identity);

            shieldTransform = Utils.FindChildByName(Player, "Shield").transform;
            weaponTransform = Utils.FindChildByName(Player, "Weapon").transform;
            centerPoint = Utils.FindChildByName(Player, "Center").transform;
            throwPoint = Utils.FindChildByName(Player, "Throw").transform;
            playerMovement = Player.GetComponent<PlayerMovement>();
            playerAttack = Player.GetComponent<PlayerAttack>();
            playerValue = Player.GetComponent<PlayerValue>();
            playerBag = Player.GetComponent<PlayerBag>();
        }

        void Update()
        {

        }

    }
}
