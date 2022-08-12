using Items;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Bags
{
    public enum MakeType { None, WorkBench, Water }

    [CreateAssetMenu(menuName = "MySO/MakeItemSO")]
    public class MakeItemSO : ScriptableObject
    {
        [Serializable]
        public class Production
        {
            public int num = 0;
            public int count = 0;
        }

        public int num;
        public Production[] From;
        public Production To;
        public MakeType makeType = MakeType.None;

        public MakeItem Create()
        {
            var newMakeItem = GameObject.Instantiate(UIManager.I.MakeItemPrefab);
            newMakeItem.Initialization(this);
            return newMakeItem;
        }

        public bool Check()
        {
            bool flag = true;
            foreach (var pro in From)
            {
                if (!PlayerManager.I.PlayerBag.Bag.CheckBag
                    (Utils.ProductionToSerialization(pro)))
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        public void DeleteFrom()
        {
            foreach (var pro in From)
                PlayerManager.I.PlayerBag.Bag.DeleteBag
                    (Utils.ProductionToSerialization(pro));
        }
    }
}
