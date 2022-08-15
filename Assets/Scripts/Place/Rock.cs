using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Place
{
    public class Rock : PlaceObject
    {
        //�ܵ���������
        public override void BeAttackNow(WeaponSO weapon)
        {
            if (weapon.weaponPrefab.GetType() == typeof(Pickaxe))
                Blood -= weapon.attack;
            else
                Blood -= 1;
        }
        //��������
        public override void Sound()
        {
            SoundManager.I.Place(2);
        }
    }
}
