using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UseTable : MyScript
    {
        public Transform UseTableFrame;
        public List<Slot> slotList = new List<Slot>();

        private void Awake()
        {
            UseTableFrame = Utils.FindChildByName(gameObject, "Frame").transform;
            for (int i = 0; i < 8; i++)
            {
                var slot = Utils.FindChildByName(gameObject, "Slot (" + i + ")").GetComponent<Slot>();
                slot.Initialization(PlayerManager.I.PlayerBag.Bag, i, SlotClickEvent);
                slotList.Add(slot);
            }
            PlayerManager.I.PlayerBag.Bag.SlotChangeAfter += SlotChangeEvent;
        }

        private void Update()
        {
            UpdateFramePostion();
        }

        //更新frame的位置
        public void UpdateFramePostion()
        {
            var useNum = PlayerManager.I.PlayerBag.UseNum;
            UseTableFrame.localPosition = Vector3.Lerp(UseTableFrame.localPosition,
                new Vector3(-161.7f + useNum * 46, UseTableFrame.localPosition.y,
                UseTableFrame.localPosition.z), 0.5f);
        }
        //slot点击事件
        public void SlotClickEvent(Slot slot, bool mouse)
        {
            if (mouse)
                PlayerManager.I.PlayerBag.UseNum = slot.bagNum;
            else
                PlayerManager.I.PlayerBag.ThrowBagItem(slot.bagNum);
        }
        //slot更新事件
        public void SlotChangeEvent(int bagNum)
        {
            if (bagNum >= 8)
                return;
            var bag = PlayerManager.I.PlayerBag.Bag;
            if (bag.ItemList[bagNum] == null)
                Utils.ClearChilds(slotList[bagNum].gameObject);
            else
                bag.ItemList[bagNum].Create(false).transform.SetParent(slotList[bagNum].transform, false);
        }
    }
}
