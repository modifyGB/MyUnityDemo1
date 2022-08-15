using Bags;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class MakeTable : MyScript, IPointerDownHandler, IPointerUpHandler
    {
        private Transform MakeList;
        private Dictionary<int, MakeItem> makeItemList;
        public Dictionary<int, MakeItem> MakeItemList { get { return makeItemList; } }
        private Dictionary<MakeType, bool> makeTypeOpenList = new Dictionary<MakeType, bool>();
        public Dictionary<MakeType, bool> MakeTypeOpenList { get { return makeTypeOpenList; } }

        private int offset = -350;
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
            foreach (MakeType type in Enum.GetValues(typeof(MakeType)))
                makeTypeOpenList.Add(type, false);
            makeTypeOpenList[MakeType.None] = true;
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

        //检查makeitem
        public void MakeItemCheck()
        {
            for (int i = 0; i < makeTypeOpenList.Count; i++)
            {
                var item = makeTypeOpenList.ElementAt(i);
                if (makeTypeOpenList[item.Key])
                    RefreshMakeItem(item.Key, true);
                else
                    RefreshMakeItem(item.Key, false);
            }
        }
        public void MakeItemCheck(int x)
        {
            MakeItemCheck();
        }
        //更新makeitem列表
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
        //加入makeitem
        public void AddMakeItem(MakeItemSO makeItemSO)
        {
            if (makeItemList.ContainsKey(makeItemSO.num))
                return;
            var makeItem = makeItemSO.Create();
            makeItemList[makeItemSO.num] = makeItem;
            makeItem.transform.SetParent(MakeList, false);

            offset += 80;
            downPos = new Vector3(0, 145 + Mathf.Clamp(offset, 0, float.MaxValue), 0);
        }
        //删除makeitem
        public void DeleteMakeItem(MakeItemSO makeItemSO)
        {
            if (!makeItemList.ContainsKey(makeItemSO.num))
                return;
            makeItemList[makeItemSO.num].DestroySelf();
            makeItemList.Remove(makeItemSO.num);

            offset -= 80;
            downPos = new Vector3(0, 145 + Mathf.Clamp(offset, 0, float.MaxValue), 0);
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
