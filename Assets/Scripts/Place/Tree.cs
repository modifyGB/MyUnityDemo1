using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Place
{
    public class Tree : PlaceObject
    {
        //受到攻击处理
        public override void BeAttackNow(WeaponSO weapon)
        {
            if (weapon.weaponPrefab.GetType() == typeof(Axe))
                Blood -= weapon.attack;
            else
                Blood -= 1;
        }
        //声音处理
        public override void Sound()
        {
            SoundManager.I.Place(1);
        }
    }
}
