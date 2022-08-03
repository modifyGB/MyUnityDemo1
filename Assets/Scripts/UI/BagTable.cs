using Items;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BagTable : MyScript
    {
        public List<Slot> slotList = new List<Slot>();

        private void Awake()
        {
            var bagCapacity = PlayerManager.I.PlayerBag.Bag.bagCapacity;
            for (int i = 0; i < bagCapacity; i++)
            {
                var slot = Utils.FindChildByName(gameObject, "Slot (" + i + ")").GetComponent<Slot>();
                slot.Initialization(PlayerManager.I.PlayerBag.Bag, i, SlotClickEvent);
                slotList.Add(slot);
                var image = slot.GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
                slotList[i].GetComponent<Button>().interactable = true;
            }
            PlayerManager.I.PlayerBag.Bag.SlotChangeAfter += SlotChangeEvent;
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
                else if (PlayerManager.I.PlayerBag.Bag.ItemList[slot.bagNum] == null 
                    && UIManager.I.PointerItem.item.itemSO.isCountable)
                {
                    PlayerManager.I.PlayerBag.Bag.AddItemList(
                        new Item(UIManager.I.PointerItem.item.Num, 1), slot.bagNum);
                    UIManager.I.PointerItem.item.Count--;
                    UIManager.I.DeletePointerItemCheck();

                }
                else if (UIManager.I.PointerItem.item.Num ==
                    PlayerManager.I.PlayerBag.Bag.ItemList[slot.bagNum].Num)
                {
                    UIManager.I.PointerItem.item.Count--;
                    PlayerManager.I.PlayerBag.Bag.ItemList[slot.bagNum].Count++;
                    UIManager.I.DeletePointerItemCheck();
                }
                else
                    UIManager.I.PointerAndSlot(slot);
            }
        }
        //slot更新事件
        public void SlotChangeEvent(int bagNum)
        {
            var bag = PlayerManager.I.PlayerBag.Bag;
            if (bag.ItemList[bagNum] == null)
                Utils.ClearChilds(slotList[bagNum].gameObject);
            else
                bag.ItemList[bagNum].Create(false).transform.SetParent(slotList[bagNum].transform, false);
        }
    }
}
