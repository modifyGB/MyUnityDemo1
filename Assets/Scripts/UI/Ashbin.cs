using Items;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class Ashbin : MyScript, IPointerDownHandler
    {
        private ItemObject itemObject = null;
        private GameObject background = null;


        private void Awake()
        {
            background = transform.Find("background").gameObject;
            UIManager.I.UISwitch += Clear;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var bag = UIManager.I;
            if (bag.PointerItem != null)
            {
                if (itemObject != null)
                    itemObject.item.DestroySelf();
                itemObject = bag.PointerItem;
                bag.PointerItem = null;
                itemObject.transform.SetParent(transform);
                itemObject.transform.localPosition = Vector3.zero;
                background.SetActive(false);
            }
            else if (itemObject != null)
            {
                itemObject.transform.SetParent(UIManager.I.BagTable.transform);
                bag.PointerItem = itemObject;
                itemObject = null;
                background.SetActive(true);
            }
        }

        public void Clear(UIState flag)
        {
            if (flag != UIState.Interface)
                return;
            if (itemObject != null)
                itemObject.item.DestroySelf();
            itemObject = null;
            background.SetActive(true);
        }
    }
}
