using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Place
{
    public class WorkBench : PlaceObject
    {
        //�ܵ���������
        public override void BeAttackNow(WeaponSO weapon)
        {
            if (weapon.weaponPrefab.GetType() == typeof(Axe))
                Blood -= weapon.attack;
            else
                Blood -= 1;
        }
        //�Ҽ��¼�
        public override void Mouse1Event()
        {
            UIManager.I.UIState = UIState.Interface;
            UIManager.I.MakeTable.MakeType = Bags.MakeType.WorkBench;
            UIManager.I.UILeftState = UILeftState.Make;
        }
    }
}
