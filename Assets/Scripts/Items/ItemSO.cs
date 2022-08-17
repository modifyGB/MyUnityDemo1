using Place;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Items
{
    public enum UseType { None, AddBlood, Bag1, Bag2, Bag3 }

    [CreateAssetMenu(menuName = "MySO/ItemSO")]
    public class ItemSO : MyScriptable
    {
        [Header("id")]
        public int num = 0;
        public ItemObject itemObject;
        public string itemName = "Item Name";
        public string text = "ItemObject describe";
        [Header("种类")]
        public bool isCountable = false;
        public bool isDurable = false;
        [Header("特性")]
        public int MaxCount = 64;
        public float MaxDure = 100;
        public PlaceSO placeSO = null;
        public WeaponSO weaponSO = null;
        [Header("使用")]
        public UseType useType = UseType.None;
        public float addBlood = 0;


        public bool isPlaceable { get { return placeSO != null; } }
        public bool isWeapon { get { return weaponSO != null; } }
    }
}
