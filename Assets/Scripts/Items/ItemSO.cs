using Place;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Items
{
    [CreateAssetMenu(menuName = "MySO/ItemSO")]
    public class ItemSO : MyScriptable
    {
        [Header("id")]
        public int num = 0;
        public ItemObject itemObject;
        [Header("种类")]
        public bool isCountable = false;
        public bool isUseable = false;
        public bool isDurable = false;
        [Header("特性")]
        public int MaxCount = 64;
        public float MaxDure = 100;
        public PlaceSO placeSO = null;
        public WeaponSO weaponSO = null;
        public string text = "ItemObject describe";

        public bool isPlaceable { get { return placeSO != null; } }
        public bool isWeapon { get { return weaponSO != null; } }
    }
}
