using Items;
using Manager;
using Place;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class BoxTable : MyScript
    {
        private Transform BoxItemList;
        private List<Slot> slotList = new List<Slot>();
        public List<Slot> SlotList { get { return slotList; } }
        private Chest chest;
        public Chest Chest { get { return chest; } }

        private void Awake()
        {
            BoxItemList = Utils.FindChildByName(gameObject, "BoxItemList").transform;
        }

        //初始化
        public void Initializate(Chest chest)
        {
            this.chest = chest;
            for (int i = 0; i < chest.Bag.bagCapacity; i++)
            {
                var slot = Instantiate(UIManager.I.SlotPrefab);
                slot.Initialization(chest.Bag, i, SlotClickEvent);
                slot.transform.SetParent(BoxItemList, false);
                slotList.Add(slot);
            }
            chest.Bag.SlotChangeAfter += SlotChangeEvent;
            for (int i = 0; i < chest.Bag.bagCapacity; i++)
                SlotChangeEvent(i);
        }
        //清空
        public void Clear()
        {
            Utils.ClearChilds(BoxItemList.gameObject);
            slotList.Clear();
            chest = null;
        }
        //slot点击事件
        public void SlotClickEvent(Slot slot, bool mouse)
        {
            if (mouse)
                UIManager.I.PointerAndSlot(slot);
            else
            {
                if (UIManager.I.PointerItem == null)
                    UIManager.I.CreateItemMenu(slot.bag, slot.bagNum);
                else if (slot.bag.ItemList[slot.bagNum] == null
                    && UIManager.I.PointerItem.item.itemSO.isCountable)
                {
                    slot.bag.AddItemList(new Item(UIManager.I.PointerItem.item.Num, 1), slot.bagNum);
                    UIManager.I.PointerItem.item.Count--;
                    UIManager.I.DeletePointerItemCheck();

                }
                else if (UIManager.I.PointerItem.item.Num ==
                    slot.bag.ItemList[slot.bagNum].Num)
                {
                    UIManager.I.PointerItem.item.Count--;
                    slot.bag.ItemList[slot.bagNum].Count++;
                    UIManager.I.DeletePointerItemCheck();
                }
                else
                    UIManager.I.PointerAndSlot(slot);
            }
        }
        //slot更新事件
        public void SlotChangeEvent(int bagNum)
        {
            var bag = chest.Bag;
            if (bag.ItemList[bagNum] == null)
                Utils.ClearChilds(slotList[bagNum].gameObject);
            else
                bag.ItemList[bagNum].Create(false).transform.SetParent(slotList[bagNum].transform, false);
        }
    }
}
