using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using System;
using Manager;

namespace Bags
{
    public class Bag
    {
        private List<Item> itemList = new List<Item>();
        public List<Item> ItemList { get { return itemList; } }
        public int bagCapacity = 8;

        public Action<int> SlotChangeBefore;
        public Action<int> SlotChangeAfter;

        public Bag(int bagCapacity)
        {
            this.bagCapacity = bagCapacity;
            for (int i = 0; i < bagCapacity; i++)
                itemList.Add(null);
        }
        //添加物品至背包
        public Item AddBag(Item.Serialization item)
        {
            var table = GameManager.I.itemTableSO.table;
            if (table.Length <= item.num)
                return null;

            if (table[item.num].isCountable)
            {
                for (int i = 0; i < bagCapacity; i++)
                    if (ItemList[i] != null && ItemList[i].Num == item.num)
                    {
                        var x = Mathf.Clamp(item.count, 0, table[item.num].MaxCount - ItemList[i].Count);
                        ItemList[i].Count += x;
                        item.count -= x;
                        if (item.count == 0)
                            break;
                    }
                if (item.count == 0)
                    return null;

                var newItem = new Item(item);
                bool flag = false;
                for (int i = 0; i < bagCapacity; i++)
                    if (ItemList[i] == null)
                    {
                        AddItemList(newItem, i);
                        flag = true;
                        break;
                    }
                if (flag)
                    return null;
                return newItem;
            }
            else
            {
                var newItem = new Item(item);
                bool flag = false;
                for (int i = 0; i < bagCapacity; i++)
                    if (ItemList[i] == null)
                    {
                        AddItemList(newItem, i);
                        flag = true;
                        break;
                    }
                if (flag)
                    return null;
                return newItem;
            }
        }
        //删除物品至背包
        public void DeleteBag(Item.Serialization item)
        {
            var table = GameManager.I.itemTableSO.table;
            if (table.Length <= item.num)
                return;

            for (int i = 0; i < bagCapacity; i++)
                if (ItemList[i] != null && ItemList[i].Num == item.num)
                {
                    if (item.count > ItemList[i].Count)
                    {
                        item.count -= ItemList[i].Count;
                        PutItemList(i);
                    }
                    else if (item.count < ItemList[i].Count)
                    {
                        ItemList[i].Count -= item.count;
                        break;
                    }
                    else
                    {
                        PutItemList(i);
                        break;
                    }
                }
        }
        //查看物品从背包
        public bool CheckBag(Item.Serialization item)
        {
            var count = item.count;
            bool flag = false;
            for (int i = 0; i < bagCapacity; i++)
            {
                if (ItemList[i] != null && ItemList[i].Num == item.num)
                    count -= ItemList[i].Count;
                if (count <= 0)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        //添加物品至itemlist
        public void AddItemList(Item item, int bagNum)
        {
            SlotChangeBefore?.Invoke(bagNum);
            if (itemList[bagNum] != null)
                itemList[bagNum].DestroySelf();
            itemList[bagNum] = item;
            SlotChangeAfter?.Invoke(bagNum);
        }
        //弹出物品从itemlist
        public Item PutItemList(int bagNum)
        {
            if (itemList[bagNum] == null)
                return null;
            SlotChangeBefore?.Invoke(bagNum);
            var outItem = itemList[bagNum];
            itemList[bagNum] = null;
            SlotChangeAfter?.Invoke(bagNum);
            return outItem;
        }
        //删除物品检查
        public void DeleteCheck(int bagNum)
        {
            if (itemList[bagNum] == null)
                return;
            if (itemList[bagNum].Check())
                return;

            SlotChangeBefore?.Invoke(bagNum);
            PutItemList(bagNum).DestroySelf();
            SlotChangeAfter?.Invoke(bagNum);
        }
        //使用一次物品
        public void UseItemOne(int bagNum)
        {
            if (ItemList[bagNum].itemSO.useType == UseType.None)
                return;
            ItemList[bagNum].Use();
            ItemList[bagNum].Count -= 1;
            DeleteCheck(bagNum);
        }
    }
}
