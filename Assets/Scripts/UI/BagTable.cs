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
            var bagCapacity = PlayerManager.I.bagCapacity;
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
                UIManager.I.CreateItemMenu(slot.bag, slot.bagNum);
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
