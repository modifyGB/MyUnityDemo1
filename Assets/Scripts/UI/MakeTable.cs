using Bags;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class MakeTable : MyScript, IPointerDownHandler, IPointerUpHandler
    {
        private Transform MakeList;
        private Dictionary<int, MakeItem> makeItemList;
        public Dictionary<int, MakeItem> MakeItemList { get { return makeItemList; } }
        private MakeType makeType = MakeType.None;
        public MakeType MakeType 
        { 
            get { return makeType; } 
            set 
            { 
                makeType = value;
                RefreshMakeItem(value, true);
            } 
        }

        private int offset = 350;
        private Vector3 upPos = new Vector3(0, 145, 0);
        private Vector3 downPos = new Vector3(0, 145, 0);
        private Vector3 beginDraggingPoint;
        private Vector3 beginObjectPos;
        private bool isDragging = false;

        private void Awake()
        {
            MakeList = Utils.FindChildByName(gameObject, "MakeList").GetComponent<Transform>();
            makeItemList = new Dictionary<int, MakeItem>();
            PlayerManager.I.PlayerBag.Bag.SlotChangeAfter += MakeItemCheck;
        }

        private void Update()
        {
            if (isDragging)
            {
                var nowDraggingPoint = Input.mousePosition;
                MakeList.localPosition = new Vector3(0, beginObjectPos.y
                    + nowDraggingPoint.y - beginDraggingPoint.y, 0);
            }
            else
            {
                if (MakeList.localPosition.y < upPos.y)
                    MakeList.localPosition = Vector3.Lerp(MakeList.localPosition, upPos, 0.1f);
                else if (MakeList.localPosition.y > downPos.y)
                    MakeList.localPosition = Vector3.Lerp(MakeList.localPosition, downPos, 0.1f);
            }
        }

        public void MakeItemCheck(int x)
        {
            RefreshMakeItem(MakeType.None, true);
            if (makeType != MakeType.None)
                RefreshMakeItem(makeType, true);
        }

        public void RefreshMakeItem(MakeType makeType, bool isUsed)
        {
            if (isUsed)
                foreach (var makeItem in GameManager.I.MakeTypeDic[makeType])
                {
                    if (!makeItem.Check())
                        DeleteMakeItem(makeItem);
                    else if (makeItem.Check())
                        AddMakeItem(makeItem);
                }
            else
                foreach (var makeItem in GameManager.I.MakeTypeDic[makeType])
                    DeleteMakeItem(makeItem);
        }

        public void AddMakeItem(MakeItemSO makeItemSO)
        {
            if (makeItemList.ContainsKey(makeItemSO.num))
                return;
            var makeItem = makeItemSO.Create();
            makeItemList[makeItemSO.num] = makeItem;
            makeItem.transform.SetParent(MakeList, false);
        }

        public void DeleteMakeItem(MakeItemSO makeItemSO)
        {
            if (!makeItemList.ContainsKey(makeItemSO.num))
                return;
            makeItemList[makeItemSO.num].DestroySelf();
            makeItemList.Remove(makeItemSO.num);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            beginDraggingPoint = Input.mousePosition;
            beginObjectPos = MakeList.localPosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
        }
    }
}
