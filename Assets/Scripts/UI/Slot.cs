using Bags;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

namespace UI
{
    public class Slot : MyScript, IPointerUpHandler
    {
        public Bag bag;
        public int bagNum;
        private Button button;
        public Action<Slot, bool> ClickEvent;

        private void Awake()
        {
            button = GetComponent<Button>();
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
        }
    }
}
