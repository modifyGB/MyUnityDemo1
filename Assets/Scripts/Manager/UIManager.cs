using Bags;
using Items;
using Place;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public enum UIState { Play, Interface, Setting, Died }
    public enum UILeftState { State, Make, Box }

    public class UIManager : Singleton<UIManager>
    {
        [Header("组件")]
        public GameObject PlayerInterfacePrefab;
        public GameObject DropItemCanvaPrefab;
        public PlayerStateBar PlayerStateBarPrefab;
        public UseTable UseTablePrefab;
        public ItemMenu ItemMenuPrefab;
        public MakeItem MakeItemPrefab;
        public Slot SlotPrefab;
        public ItemObject ItemObjectPrefab;
        public SettingTable SettingTablePrefab;
        public Describe describePrefab;
        public DiedCanvas DiedCanvasPrefab;

        private GameObject playerInterface;
        public GameObject PlayerInterface { get { return playerInterface; } }
        private SettingTable settingTable;
        public SettingTable SettingTable { get { return settingTable; } }
        private GameObject dropItemCanva;
        public GameObject DropItemCanva { get { return dropItemCanva; } }
        private PlayerStateBar playerStateBar;
        public PlayerStateBar PlayerStateBar { get { return playerStateBar; } }
        private PlayerStateValue playerStateValue;
        public PlayerStateValue PlayerStateValue { get { return playerStateValue; } }
        private PlayerStateButton playerStateButton;
        public PlayerStateButton PlayerStateButton { get { return playerStateButton; } }
        private MakeTableButton makeTableButton;
        public MakeTableButton MakeTableButton { get { return makeTableButton; } }
        private BoxButton boxButton;
        public BoxButton BoxButton { get { return boxButton; } }
        private UseTable useTable;
        public UseTable UseTable { get { return useTable; } }
        private BagTable bagTable;
        public BagTable BagTable { get { return bagTable; } }
        private MakeTable makeTable;
        public MakeTable MakeTable { get { return makeTable; } }
        private BoxTable boxTable;
        public BoxTable BoxTable { get { return boxTable; } }
        private Describe describe;
        public Describe Describe { get { return describe; } }
        private DiedCanvas diedCanvas;
        public DiedCanvas DiedCanvas { get { return diedCanvas; } }

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
        private Chest nowChest = null;
        public Chest NowChest
        {
            get { return nowChest; }
            set 
            {
                nowChest = value;
                if (value != null)
                {
                    boxTable.Initializate(value);
                    UIState = UIState.Interface;
                    UILeftState = UILeftState.Box;
                }
                else
                    boxTable.Clear();
            }
        }
        private UILeftState uILeftState = UILeftState.State;
        public UILeftState UILeftState
        {
            get { return uILeftState; }
            set
            {
                uILeftState = value;

                playerStateValue.gameObject.SetActive(false);
                makeTable.gameObject.SetActive(false);
                boxTable.gameObject.SetActive(false);
                playerStateButton.button.interactable = true;
                makeTableButton.button.interactable = true;
                boxButton.button.interactable = true;
                if (nowChest == null)
                    boxButton.gameObject.SetActive(false);
                else
                    boxButton.gameObject.SetActive(true);

                if (value == UILeftState.State)
                {
                    playerStateValue.gameObject.SetActive(true);
                    playerStateButton.button.interactable = false;
                }
                else if (value == UILeftState.Make)
                {
                    makeTable.gameObject.SetActive(true);
                    makeTableButton.button.interactable = false;
                }
                else if (value == UILeftState.Box)
                {
                    boxTable.gameObject.SetActive(true);
                    boxButton.button.interactable = false;             
                }
                UILeftSwitch?.Invoke(value);
            }
        }
        private UIState uIState = UIState.Play;
        public UIState UIState
        {
            get { return uIState; }
            set
            {
                uIState = value;
                if (value == UIState.Play)
                {
                    PlayerInterface.SetActive(false);
                    UseTable.gameObject.SetActive(true);
                    settingTable.gameObject.SetActive(false);
                    if (diedCanvas != null) diedCanvas.DestroySelf();
                }
                else if (value == UIState.Interface)
                {
                    PlayerInterface.SetActive(true);
                    UseTable.gameObject.SetActive(false);
                    settingTable.gameObject.SetActive(false);
                    if (diedCanvas != null) diedCanvas.DestroySelf();
                    UILeftState = UILeftState.State;
                }
                else if (value == UIState.Setting)
                {
                    PlayerInterface.SetActive(false);
                    UseTable.gameObject.SetActive(false);
                    settingTable.gameObject.SetActive(true);
                    if (diedCanvas != null) diedCanvas.DestroySelf();
                }
                else if (value == UIState.Died)
                {
                    PlayerInterface.SetActive(false);
                    UseTable.gameObject.SetActive(false);
                    settingTable.gameObject.SetActive(false);
                    if (diedCanvas == null) diedCanvas = Instantiate(DiedCanvasPrefab);
                }
                UISwitch.Invoke(value);
            }
        }
        public Action<UIState> UISwitch;
        public Action<UILeftState> UILeftSwitch;

        private Dictionary<KeyCode, bool> inputDic = new Dictionary<KeyCode, bool>();
        private float scrollWheel = 0;
        private bool isMouse0 = false;
        private bool isMouse1 = false;

        public override void Awake()
        {
            base.Awake();

            useTable = Instantiate(UseTablePrefab);
            playerInterface = Instantiate(PlayerInterfacePrefab);
            dropItemCanva = Instantiate(DropItemCanvaPrefab);
            playerStateBar = Instantiate(PlayerStateBarPrefab);
            settingTable = Instantiate(SettingTablePrefab);
            bagTable = Utils.FindChildByName(PlayerInterface, "BagTable").GetComponent<BagTable>();
            makeTable = Utils.FindChildByName(PlayerInterface, "MakeTable").GetComponent<MakeTable>();
            boxTable = Utils.FindChildByName(PlayerInterface, "BoxTable").GetComponent<BoxTable>();
            playerStateValue = Utils.FindChildByName(PlayerInterface, "PlayerState").GetComponent<PlayerStateValue>();
            playerStateButton = Utils.FindChildByName(PlayerInterface, "PlayerStateButton").GetComponent<PlayerStateButton>();
            makeTableButton = Utils.FindChildByName(PlayerInterface, "MakeTableButton").GetComponent<MakeTableButton>();
            boxButton = Utils.FindChildByName(PlayerInterface, "BoxButton").GetComponent<BoxButton>();

            UIState = UIState.Play;
            UILeftState = UILeftState.State;

            for (int i = 49; i < 57; i++)
                inputDic.Add((KeyCode)i, false);
            inputDic.Add(KeyCode.E, false);
            inputDic.Add(KeyCode.Escape, false);

            UISwitch += DeleteItemMenu;
            UISwitch += ThrowPointerItem;
            UISwitch += ClearNowChest;
            UISwitch += DeleteDescribe;
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
            if (!isMouse1)
                isMouse1 = Input.GetMouseButtonDown(1);

            UpdatePointerItem();
            UpdateDescribe();
        }

        private void FixedUpdate()
        {
            if (inputDic[KeyCode.E])
            {
                if (UIState == UIState.Interface)
                    UIState = UIState.Play;
                else
                    UIState = UIState.Interface;
                inputDic[KeyCode.E] = false;
            }

            if (inputDic[KeyCode.Escape])
            {
                if (UIState == UIState.Setting)
                    UIState = UIState.Play;
                else
                    UIState = UIState.Setting;
                inputDic[KeyCode.Escape] = false;
            }

            if (isMouse0)
            {
                if (itemMenu != null)
                    itemMenu.DestroySelf();
                isMouse0 = false;
            }

            Mouse1Event();
            ChangeUseTable();
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
                    if (UIState == UIState.Play)
                    {
                        bag.UseNum = i - 49;
                        flag = true;
                    }
                    break;
                }
            if (flag)
                for (int i = 49; i < 57; i++)
                    inputDic[(KeyCode)i] = false;
            //scrollWheel
            if (scrollWheel > 0)
            {
                if (UIState == UIState.Play)
                {
                    if (bag.UseNum == 0)
                        bag.UseNum = 7;
                    else
                        bag.UseNum--;
                    scrollWheel = 0;
                }
            }
            else if (scrollWheel < 0)
            {
                if (UIState == UIState.Play)
                {
                    if (bag.UseNum == 7)
                        bag.UseNum = 0;
                    else
                        bag.UseNum++;
                    scrollWheel = 0;
                }
            }
        }
        //右键事件
        void Mouse1Event()
        {
            if (isMouse1)
            {
                var hitObject_ = Utils.CameraRay();
                if (hitObject_.transform != null && uIState == UIState.Play)
                {
                    var place = Utils.CameraRay().transform.GetComponent<PlaceObject>();
                    if (place != null)
                        place.Mouse1Event();
                }
                isMouse1 = false;
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

            DeleteDescribe();
        }
        //删除ItemMenu
        public void DeleteItemMenu(UIState isOpen)
        {
            if (itemMenu != null)
                itemMenu.DestroySelf();
        }
        //更新PointerItem位置
        public void UpdatePointerItem()
        {
            if (pointerItem == null || UIManager.I.UIState == UIState.Play)
                return;

            pointerItem.transform.position = Input.mousePosition;
        }
        //丢弃PointerItem
        public void ThrowPointerItem(UIState flag)
        {
            if (flag != UIState.Interface)
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
        //删除pointerItem检查
        public void DeletePointerItemCheck()
        {
            if (PointerItem == null)
                return;
            if (PointerItem.item.Check())
                return;

            PointerItem.DestroySelf();
            PointerItem = null;
        }
        //清空nowChest
        public void ClearNowChest(UIState uIState)
        {
            if (uIState == UIState.Play)
                NowChest = null;
        }
        //改变UIState
        public void ChangeUIState(int uIState)
        {
            UIState = (UIState)uIState;
            SoundManager.I.buttonSource.Play();
        }
        //创建Describe
        public void CreateDescribe(string name, string text)
        {
            DeleteDescribe();
            describe = Instantiate(describePrefab);
            describe.Initialization(name, text);
            describe.transform.SetParent(playerInterface.transform, false);
        }
        //删除Describe
        public void DeleteDescribe()
        {
            if (describe != null)
                describe.DestroySelf();
            describe = null;
        }
        public void DeleteDescribe(UIState uIState)
        {
            if (uIState != UIState.Interface)
                DeleteDescribe();
        }
        //更新Describe位置
        public void UpdateDescribe()
        {
            if (describe == null)
                return;

            describe.transform.position = Input.mousePosition;

        }
    }
}
