using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Place
{
    public class WorkBench : PlaceObject
    {
        //受到攻击处理
        public override void BeAttackNow(WeaponSO weapon)
        {
            if (weapon.weaponPrefab.GetType() == typeof(Axe))
                Blood -= weapon.attack;
            else
                Blood -= 1;
        }
        //右键事件
        public override void Mouse1Event()
        {
            UIManager.I.UIState = UIState.Interface;
            UIManager.I.MakeTable.MakeType = Bags.MakeType.WorkBench;
            UIManager.I.UILeftState = UILeftState.Make;
        }
    }
}
