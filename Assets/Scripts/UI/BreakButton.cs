using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class BreakButton : MyScript, IPointerDownHandler
    {
        public ItemMenu ItemMenu;
        public Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.interactable)
                return;

            UIManager.I.BreakItemHalfToPointer(ItemMenu.bag, ItemMenu.bagNum);

            SoundManager.I.buttonSource.Play();
        }
    }
}
