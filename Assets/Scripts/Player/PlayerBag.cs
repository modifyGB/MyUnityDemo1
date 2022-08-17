using Bags;
using Items;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerBag : MyScript
    {
        private Bag bag;
        public Bag Bag { get { return bag; } }

        private int useNum = 0;
        public int UseNum
        {
            get { return useNum; }
            set
            {
                if (useNum != value)
                {
                    if (bag.ItemList[useNum] != null)
                        bag.ItemList[useNum].Lose();
                    if (bag.ItemList[value] != null)
                        bag.ItemList[value].Choose();
                }
                useNum = value;
            }
        }

        private Dictionary<KeyCode, bool> inputDic = new Dictionary<KeyCode, bool>();
        private bool isMouse0 = false;

        private void Awake()
        {
            bag = new Bag(GameManager.I.ArchiveData.Player.bagCapacity);
            bag.SlotChangeBefore += LoseEvent;
            bag.SlotChangeAfter += ChooseEvent;

            inputDic.Add(KeyCode.Q, false);
        }

        private void Start()
        {
            foreach (var item in GameManager.I.ArchiveData.Player.bag)
                bag.AddBag(item);
        }

        private void Update()
        {
            for (int i = 0; i < inputDic.Count; i++)
            {
                var item = inputDic.ElementAt(i);
                if (!inputDic[item.Key])
                    inputDic[item.Key] = Input.GetKeyDown(item.Key);
            }
            if (!isMouse0)
                isMouse0 = Input.GetMouseButtonDown(0);
        }

        private void FixedUpdate()
        {
            if (inputDic[KeyCode.Q])
            {
                if (UIManager.I.UIState == UIState.Play && bag.ItemList[UseNum] != null)
                    ThrowBagItem(UseNum);
                inputDic[KeyCode.Q] = false;
            }

            if (isMouse0)
            {
                if (UIManager.I.UIState == UIState.Play && bag.ItemList[UseNum] != null)
                    bag.UseItemOne(UseNum);
                isMouse0 = false;
            }
        }
        
        //丢弃bag的item
        public void ThrowBagItem(int bagNum)
        {
            if (bag.ItemList[bagNum] == null)
                return;
            var item = bag.PutItemList(bagNum);
            item.DestroySelf();
            item.Throw(PlayerManager.I.ThrowPoint, PlayerManager.Player.transform.forward * 5);
        }
        //Choose事件
        public void ChooseEvent(int bagNum)
        {
            if (bagNum == UseNum && bag.ItemList[bagNum] != null)
                bag.ItemList[bagNum].Choose();
        }
        //Lose事件
        public void LoseEvent(int bagNum)
        {
            if (bagNum == UseNum && bag.ItemList[bagNum] != null)
                bag.ItemList[bagNum].Lose();
        }
        //背包扩容
        public void ExpansionBag(int expansionCapacity)
        {
            expansionCapacity = Mathf.Clamp(expansionCapacity, 0, 56 - bag.bagCapacity);
            if (expansionCapacity == 0)
                return;

            bag.ExpansionBag(expansionCapacity);
            UIManager.I.BagTable.ExpansionSlot(expansionCapacity);
        }
    }
}
