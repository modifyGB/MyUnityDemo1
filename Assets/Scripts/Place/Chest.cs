using Bags;
using Items;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Place
{
    public class Chest : PlaceObject
    {
        private Animator animator;
        public Animator Animator { get { return animator; } }

        private Bag bag;
        public Bag Bag { get { return bag; } }

        float dis4Player;

        public struct ChestSerialization
        {
            public PlaceObject.Serialization place;
            public List<Item.Serialization> bag;
            public ChestSerialization(PlaceObject.Serialization place, List<Item.Serialization> bag)
            { this.place = place; this.bag = bag; }
        }

        public ChestSerialization ToChestSerialization()
        {
            var b = new List<Item.Serialization>();
            foreach (var item in bag.ItemList)
            {
                if (item == null)
                    continue;
                b.Add(item.ToSerialization());
            }
            return new ChestSerialization(ToSerialization(), b);
        }

        public override void Awake()
        {
            base.Awake();

            animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            dis4Player = Vector3.Distance(PlayerManager.Player.transform.position, transform.position);
        }

        public void Initialization(int bagCapacity)
        {
            bag = new Bag(bagCapacity);

            bag.SlotChangeAfter += Save;
        }

        public void InitializationBag(List<Item.Serialization> bagList)
        {
            if (bagList == null)
                return;
            foreach (Item.Serialization item in bagList)
                bag.AddBag(item);
        }
        //右键事件
        public override void Mouse1Event()
        {
            if (dis4Player < 3)
                UIManager.I.NowChest = this;
        }

        public override void BeAttackNow(WeaponSO weapon)
        {
            if (weapon.weaponPrefab.GetType() == typeof(Axe))
                Blood -= weapon.attack;
            else
                Blood -= 1;
        }

        public override void Drop()
        {
            base.Drop();
            foreach (var item in bag.ItemList)
            {
                if (item == null) continue;
                item.Throw(dropPoint, new Vector3(0, 3, 0));
            }
        }
        //声音处理
        public override void Sound()
        {
            SoundManager.I.Place(1);
        }
        //保存宝箱
        public void Save()
        {
            if (!MapManager.I.chestList.ContainsKey(origin.x)
                || !MapManager.I.chestList[origin.x].ContainsKey(origin.y))
                return;
            MapManager.I.chestList[origin.x][origin.y] = ToChestSerialization();
        }
        public void Save(int x)
        {
            if (bag.ItemList[x] != null)
                bag.ItemList[x].ItemChange += Save;
            Save();
        }
    }
}
