using Bags;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ItemMenu : MyScript
    {
        public int bagNum;
        public Bag bag;

        private ThrowButton ThrowButton;
        private UseButton UseButton;
        private BreakButton BreakButton;

        private void Awake()
        {
            ThrowButton = Utils.FindChildByName(gameObject, "ThrowButton").GetComponent<ThrowButton>();
            UseButton = Utils.FindChildByName(gameObject, "UseButton").GetComponent<UseButton>();
            BreakButton = Utils.FindChildByName(gameObject, "BreakButton").GetComponent<BreakButton>();
            ThrowButton.ItemMenu = this;
            UseButton.ItemMenu = this;
            BreakButton.ItemMenu = this;
        }

        public void SetTarget(Bag bag, int bagNum)
        {
            this.bagNum = bagNum;
            this.bag = bag;
            if (!bag.ItemList[bagNum].itemSO.isCountable || bag.ItemList[bagNum].IsUnlimited || bag.ItemList[bagNum].Count == 1)
                BreakButton.button.interactable = false;
            if (!bag.ItemList[bagNum].itemSO.isUseable)
                UseButton.button.interactable = false;
        }
    }
}
