using Bags;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class MakeTable : MyScript
    {
        private Transform MakeList;
        private Dictionary<int, MakeItem> makeItemList;
        public Dictionary<int, MakeItem> MakeItemList { get { return makeItemList; } }

        private void Awake()
        {
            MakeList = Utils.FindChildByName(gameObject, "MakeList").GetComponent<Transform>();
            makeItemList = new Dictionary<int, MakeItem>();
            PlayerManager.I.PlayerBag.Bag.SlotChangeAfter += MakeItemCheck;
        }

        public void MakeItemCheck(int x)
        {
            foreach (var makeItem in GameManager.I.MakeTypeDic[MakeType.None])
            {
                if (!makeItem.Check() && makeItemList.ContainsKey(makeItem.num))
                    DeleteMakeItem(makeItem);
                else if (makeItem.Check() && !makeItemList.ContainsKey(makeItem.num))
                    AddMakeItem(makeItem);
            }
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
    }
}
