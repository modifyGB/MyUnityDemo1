using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class Item
    {
        private int num;
        public int Num { get { return num; } }
        private int count = 1;
        public int Count
        {
            get { return count; }
            set
            {
                if (isUnlimited)
                    return;
                count = value;
                ItemChange.Invoke();
            }
        }
        private float dure = 0;
        public float Dure
        {
            get { return itemSO.MaxDure - dure; }
            set
            {
                if (!itemSO.isDurable || isUnlimited)
                    return;
                if (value < 0)
                    value = 0;
                dure = itemSO.MaxDure - value;
                ItemChange.Invoke();
            }
        }
        private bool isUnlimited = false;
        public bool IsUnlimited
        {
            get { return isUnlimited; }
            set
            {
                isUnlimited = value;
                ItemChange.Invoke();
            }
        }

        public ItemSO itemSO;
        public Action ItemChange;
        public List<ItemObject> objectList = new List<ItemObject>();

        [Serializable]
        public struct Serialization
        {
            public int num;
            public int count;
            public float dure;
            public bool isUnlimited;
            public Serialization(int num) { this.num = num; count = 1; dure = 0; isUnlimited = false; }
            public Serialization(int num, int count) { this.num = num; this.count = count; dure = 0; isUnlimited = false; }
            public Serialization(int num, int count, bool isUnlimited) { this.num = num; this.count = count; dure = 0; this.isUnlimited = isUnlimited; }
            public Serialization(int num, float dure) { this.num = num; count = 1; this.dure = dure; isUnlimited = false; }
            public Serialization(int num, float dure, bool isUnlimited) { this.num = num; count = 1; this.dure = dure; this.isUnlimited = isUnlimited; }
        }

        public Item(int num)
        {
            this.num = num;
            itemSO = GameManager.I.itemTableSO.table[num];
        }
        public Item(int num, int count)
        {
            this.num = num;
            this.count = count;
            itemSO = GameManager.I.itemTableSO.table[num];
        }
        public Item(int num, int count, bool isUnlimited)
        {
            this.num = num;
            this.count = count;
            this.isUnlimited = isUnlimited;
            itemSO = GameManager.I.itemTableSO.table[num];
        }
        public Item(int num, int count, float dure, bool isUnlimited)
        {
            this.num = num;
            this.count = count;
            this.isUnlimited = isUnlimited;
            itemSO = GameManager.I.itemTableSO.table[num];
            Dure = dure;
        }
        public Item(Serialization serialization)
        {
            num = serialization.num;
            count = serialization.count;
            isUnlimited = serialization.isUnlimited;
            itemSO = GameManager.I.itemTableSO.table[num];
            dure = serialization.dure;
        }

        //序列化
        public Serialization ToSerialization()
        {
            if (itemSO.isCountable)
                return new Serialization(num, count, isUnlimited);
            else if (itemSO.isDurable)
                return new Serialization(num, dure, isUnlimited);
            return new Serialization();
        }
        //创建ItemObject
        public ItemObject Create(bool isThrow)
        {
            var newObject = GameObject.Instantiate(itemSO.itemObject);
            newObject.Initialization(this, isThrow);
            objectList.Add(newObject);
            return newObject;
        }
        //使用物品
        public virtual void Use()
        {
            if (itemSO.useType == UseType.AddBlood)
                PlayerManager.I.PlayerValue.NowBlood += itemSO.addBlood;
            else if (itemSO.useType == UseType.Bag1)
            {
                if (PlayerManager.I.PlayerBag.Bag.bagCapacity >= 24)
                    return;

                PlayerManager.I.PlayerBag.ExpansionBag(16);
                UIManager.I.MakeTable.MakeTypeOpenList[Bags.MakeType.Bag1] = false;
                UIManager.I.MakeTable.MakeTypeOpenList[Bags.MakeType.Bag2] = true;
            }
            else if (itemSO.useType == UseType.Bag2)
            {
                if (PlayerManager.I.PlayerBag.Bag.bagCapacity >= 40)
                    return;

                PlayerManager.I.PlayerBag.ExpansionBag(16);
                UIManager.I.MakeTable.MakeTypeOpenList[Bags.MakeType.Bag2] = false;
                UIManager.I.MakeTable.MakeTypeOpenList[Bags.MakeType.Bag3] = true;
            }
            else if (itemSO.useType == UseType.Bag3)
            {
                if (PlayerManager.I.PlayerBag.Bag.bagCapacity >= 56)
                    return;

                PlayerManager.I.PlayerBag.ExpansionBag(16);
                UIManager.I.MakeTable.MakeTypeOpenList[Bags.MakeType.Bag3] = false;
            }
        }
        //选中物品
        public void Choose()
        {
            Build(true);
            Weapon(true);
        }
        //离开物品
        public void Lose()
        {
            Build(false);
            Weapon(false);
        }
        //丢弃Item
        public void Throw(Transform point, Vector3 velocity)
        {
            var itemObject = Create(true);
            itemObject.transform.SetParent(UIManager.I.DropItemCanva.transform, false);
            itemObject.transform.position = point.position;
            itemObject.Rb.velocity = velocity;
        }
        //建造物品触发器
        public void Build(bool Switch)
        {
            if (!itemSO.isPlaceable)
                return;

            if (Switch)
                BuildManager.I.BuildObject = itemSO.placeSO;
            else
                BuildManager.I.BuildObject = null;
        }
        //武器物品触发器
        public void Weapon(bool Switch)
        {
            if (!itemSO.isWeapon)
                return;

            if (Switch)
                PlayerManager.I.Weapon = itemSO.weaponSO.CreateWeapon();
            else
            {
                if (PlayerManager.I.Weapon != null)
                    PlayerManager.I.Weapon.DestroySelf();
                PlayerManager.I.Weapon = null;
            }
            PlayerManager.I.PlayerValue.ExperienceChange.Invoke();
        }
        //物品检查
        public bool Check()
        {
            if ((Count <= 0 || (itemSO.isDurable && Dure <= 0)) && !IsUnlimited)
                return false;
            return true;
        }
        //销毁自己
        public void DestroySelf()
        {
            foreach (var item in objectList)
                if (item != null)
                    item.DestroySelf();
        }
    }
}
