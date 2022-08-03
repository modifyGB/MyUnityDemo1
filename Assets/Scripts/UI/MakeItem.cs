using Bags;
using Items;
using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MakeItem : MyScript
    {
        private List<Slot> slotList = new List<Slot>();
        public List<Slot> SlotList { get { return slotList; } }
        private Slot outSlot;
        public Slot OutSlot { get { return outSlot; } }

        private MakeItemSO makeItemSO;

        private void Awake()
        {
            for (int i = 0; i < 6; i++)
            {
                var slot = Utils.FindChildByName(gameObject, "Slot (" + i + ")").GetComponent<Slot>();
                slot.Initialization(null, i, (a, b) => { });
                slotList.Add(slot);
            }
            outSlot = Utils.FindChildByName(gameObject, "OutSlot").GetComponent<Slot>();
            outSlot.Initialization(null, 0, OutSlotClickEvent);
        }

        public void Initialization(MakeItemSO makeItemSO)
        {
            this.makeItemSO = makeItemSO;
            var item = new Item(makeItemSO.To.num, makeItemSO.To.count);
            var itemObject = item.Create(false);
            itemObject.transform.SetParent(outSlot.transform, false);
            for (int i = 0; i < makeItemSO.From.Length; i++)
            {
                item = new Item(makeItemSO.From[i].num, makeItemSO.From[i].count);
                itemObject = item.Create(false);
                itemObject.transform.SetParent(SlotList[i].transform, false);
            }
        }

        public void OutSlotClickEvent(Slot slot, bool mouse)
        {
            if (UIManager.I.PointerItem == null)
            {
                var newItem = new Item(makeItemSO.To.num, makeItemSO.To.count);
                var newItemObject = newItem.Create(false);
                UIManager.I.PointerItem = newItemObject;
            }
            else if (UIManager.I.PointerItem != null && !UIManager.I.PointerItem.item.IsUnlimited 
                && UIManager.I.PointerItem.item.itemSO.isCountable
                && UIManager.I.PointerItem.item.Num == makeItemSO.To.num)
            {
                if (UIManager.I.PointerItem.item.Count +
                    makeItemSO.To.count > UIManager.I.PointerItem.item.itemSO.MaxCount)
                    return;
                UIManager.I.PointerItem.item.Count += makeItemSO.To.count;
            }

            makeItemSO.DeleteFrom();
        }
    }
}
