using Place;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Items
{
    public enum UseType { None, AddBlood }

    [CreateAssetMenu(menuName = "MySO/ItemSO")]
    public class ItemSO : MyScriptable
    {
        [Header("id")]
        public int num = 0;
        public ItemObject itemObject;
        [Header("����")]
        public bool isCountable = false;
        public bool isDurable = false;
        [Header("����")]
        public int MaxCount = 64;
        public float MaxDure = 100;
        public PlaceSO placeSO = null;
        public WeaponSO weaponSO = null;
        public string text = "ItemObject describe";
        [Header("ʹ��")]
        public UseType useType = UseType.None;
        public float addBlood = 0;


        public bool isPlaceable { get { return placeSO != null; } }
        public bool isWeapon { get { return weaponSO != null; } }
    }
}
