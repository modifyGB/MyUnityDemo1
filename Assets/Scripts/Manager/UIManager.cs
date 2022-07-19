using Bags;
using Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Manager
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("组件")]
        public GameObject PlayerInterfacePrefab;
        public GameObject DropItemCanvaPrefab;
        public PlayerStateBar PlayerStateBarPrefab;
        public UseTable UseTablePrefab;
        public ItemMenu ItemMenuPrefab;

        private GameObject playerInterface;
        public GameObject PlayerInterface { get { return playerInterface; } }
        private GameObject dropItemCanva;
        public GameObject DropItemCanva { get { return dropItemCanva; } }
        private PlayerStateBar playerStateBar;
        public PlayerStateBar PlayerStateBar { get { return playerStateBar; } }
        private UseTable useTable;
        public UseTable UseTable { get { return useTable; } }
        private BagTable bagTable;
        public BagTable BagTable { get { return bagTable; } }
        private ItemMenu itemMenu = null;

        private ItemObject pointerItem = null;
        public ItemObject PointerItem
        {
            get { return pointerItem; }
            set
            {
                pointerItem = value;
                if (value != null)
                    pointerItem.transform.SetParent(BagTable.transform, false);
            }
        }
        private bool isUI = false;
        public bool IsUI
        {
            get { return isUI; }
            set
            {
                isUI = value;
                PlayerInterface.SetActive(value);
                UseTable.gameObject.SetActive(!value);
                UISwitch.Invoke(value);
            }
        }
        public Action<bool> UISwitch;

        private Dictionary<KeyCode, bool> inputDic = new Dictionary<KeyCode, bool>();
        private float scrollWheel = 0;
        private bool isMouse0 = false;

        public override void Awake()
        {
            base.Awake();

            useTable = Instantiate(UseTablePrefab);
            playerInterface = Instantiate(PlayerInterfacePrefab);
            dropItemCanva = Instantiate(DropItemCanvaPrefab);
            playerStateBar = Instantiate(PlayerStateBarPrefab);
            bagTable = Utils.FindChildByName(PlayerInterface, "BagTable").GetComponent<BagTable>();
            PlayerInterface.SetActive(false);
            UseTable.gameObject.SetActive(true);

            for (int i = 49; i < 57; i++)
                inputDic.Add((KeyCode)i, false);
            inputDic.Add(KeyCode.E, false);

            UISwitch += DeleteItemMenu;
            UISwitch += ThrowPointerItem;
        }

        private void Start()
        {
            var bag = PlayerManager.I.PlayerBag.Bag;
            if (bag.ItemList[0] != null)
                bag.ItemList[0].Choose();
        }

        private void Update()
        {
            for (int i = 0; i < inputDic.Count; i++)
            {
                var item = inputDic.ElementAt(i);
                if (!inputDic[item.Key])
                    inputDic[item.Key] = Input.GetKeyDown(item.Key);
            }
            if (scrollWheel == 0)
                scrollWheel = Input.GetAxisRaw("Mouse ScrollWheel");
            if (!isMouse0)
                isMouse0 = Input.GetMouseButtonDown(0);

            UpdatePointerItem();
        }

        private void FixedUpdate()
        {
            if (inputDic[KeyCode.E])
            {
                IsUI = !IsUI;
                inputDic[KeyCode.E] = false;
            }

            if (IsUI)
            {
                DeleteItemMenu();
            }
            else
            {
                ChangeUseTable();
            }
        }

        //换物品栏所选物品
        void ChangeUseTable()
        {
            var bag = PlayerManager.I.PlayerBag;
            //keyboard
            bool flag = false;
            for (int i = 49; i < 57; i++)
                if (inputDic[(KeyCode)i])
                {
                    bag.UseNum = i - 49;
                    flag = true;
                    break;
                }
            if (flag)
                for (int i = 49; i < 57; i++)
                    inputDic[(KeyCode)i] = false;
            //scrollWheel
            if (scrollWheel > 0)
            {
                if (bag.UseNum == 0)
                    bag.UseNum = 7;
                else
                    bag.UseNum--;
                scrollWheel = 0;
            }
            else if (scrollWheel < 0)
            {
                if (bag.UseNum == 7)
                    bag.UseNum = 0;
                else
                    bag.UseNum++;
                scrollWheel = 0;
            }
        }
        //创建ItemMenu
        public void CreateItemMenu(Bag bag, int bagNum)
        {
            if (bag.ItemList[bagNum] == null)
                return;
            if (itemMenu != null)
                GameObject.Destroy(itemMenu.gameObject);
            itemMenu = Instantiate(ItemMenuPrefab);
            itemMenu.SetTarget(bag, bagNum);
            itemMenu.transform.SetParent(BagTable.transform, false);
            itemMenu.transform.position = Input.mousePosition;
        }
        //删除ItemMenu
        public void DeleteItemMenu(bool isOpen)
        {
            if (itemMenu != null)
                itemMenu.DestroySelf();
        }
        public void DeleteItemMenu()
        {
            if (isMouse0)
            {
                if (itemMenu != null)
                    itemMenu.DestroySelf();
                isMouse0 = false;
            }
        }
        //更新PointerItem位置
        public void UpdatePointerItem()
        {
            if (pointerItem == null || !UIManager.I.IsUI)
                return;

            pointerItem.transform.position = Input.mousePosition;
        }
        //丢弃PointerItem
        public void ThrowPointerItem(bool Switch)
        {
            if (Switch)
                return;
            if (pointerItem == null)
                return;
            var item = pointerItem.item;
            item.DestroySelf();
            item.Throw(PlayerManager.I.ThrowPoint, PlayerManager.Player.transform.forward * 5);
            pointerItem = null;
        }
        //鼠标与物品栏的合并与交换
        public void PointerAndSlot(Slot slot)
        {
            if (PointerItem == null && slot.bag.ItemList[slot.bagNum] == null)
                return;

            if (PointerItem != null && slot.bag.ItemList[slot.bagNum] != null && PointerItem.item.Num == slot.bag.ItemList[slot.bagNum].Num
                && PointerItem.item.itemSO.isCountable && !PointerItem.item.IsUnlimited && !slot.bag.ItemList[slot.bagNum].IsUnlimited)
            {
                int x = Mathf.Clamp(PointerItem.item.Count, 0, PointerItem.item.itemSO.MaxCount - slot.bag.ItemList[slot.bagNum].Count);
                if (x == 0)
                    return;
                slot.bag.ItemList[slot.bagNum].Count += x;
                PointerItem.item.Count -= x;
                if (!PointerItem.item.Check())
                    PointerItem.DestroySelf();
            }
            else
            {
                var x = slot.bag.PutItemList(slot.bagNum);
                if (PointerItem != null)
                {
                    slot.bag.AddItemList(PointerItem.item, slot.bagNum);
                    PointerItem.DestroySelf();
                }
                if (x != null)
                    PointerItem = x.Create(false);
            }
        }
        //分解一半物品到pointer上
        public void BreakItemHalfToPointer(Bag bag, int bagNum)
        {
            if (PointerItem != null)
                return;

            var newItem = new Item(bag.ItemList[bagNum].Num, bag.ItemList[bagNum].Count / 2);
            bag.ItemList[bagNum].Count -= bag.ItemList[bagNum].Count / 2;
            PointerItem = newItem.Create(false);
        }
    }
}
