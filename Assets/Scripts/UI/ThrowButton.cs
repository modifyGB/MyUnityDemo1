using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ThrowButton : MyScript, IPointerDownHandler
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
            var item = ItemMenu.bag.PutItemList(ItemMenu.bagNum);
            item.DestroySelf();
            item.Throw(PlayerManager.I.ThrowPoint, PlayerManager.Player.transform.forward * 5);
        }
    }
}
