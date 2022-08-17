using Bags;
using Items;
using Manager;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

namespace UI
{
    public class Slot : MyScript, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
    {
        public Bag bag;
        public int bagNum;
        private Button button;
        public ItemSO itemSO = null;
        public Action<Slot, bool> ClickEvent;

        private float timer = 0;
        private bool isStay = false;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void Update()
        {
            if (isStay)
                timer += Time.deltaTime;
            if (timer > 1 && isStay == true)
            {
                if (UIManager.I.UIState == UIState.Interface && UIManager.I.PointerItem == null)
                {
                    if (bag != null && bag.ItemList[bagNum] != null)
                        UIManager.I.CreateDescribe(bag.ItemList[bagNum]
                            .itemSO.itemName, bag.ItemList[bagNum].itemSO.text);
                    else if (itemSO != null)
                        UIManager.I.CreateDescribe(itemSO.itemName, itemSO.text);
                }
                timer = 0;
                isStay = false;
            }
        }

        public void Initialization(Bag bag, int bagNum, Action<Slot, bool> clickEvent)
        {
            this.bag = bag;
            this.bagNum = bagNum;
            ClickEvent += clickEvent;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!button.interactable)
                return;

            if (eventData.button == InputButton.Left)
                ClickEvent.Invoke(this, true);
            else if (eventData.button == InputButton.Right)
                ClickEvent.Invoke(this, false);

            SoundManager.I.buttonSource.Play();
            UIManager.I.DeleteDescribe();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isStay = true;
            timer = 0;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isStay = false;
            UIManager.I.DeleteDescribe();
        }
    }
}
