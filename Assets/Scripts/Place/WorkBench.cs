using Bags;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Place
{
    public class WorkBench : PlaceObject
    {
        float dis4Player;

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
            if (dis4Player > 3)
                return;
            UIManager.I.UIState = UIState.Interface;
            UIManager.I.UILeftState = UILeftState.Make;
        }
        //��������
        public override void Sound()
        {
            SoundManager.I.Place(1);
        }

        private void Update()
        {
            dis4Player = Vector3.Distance(PlayerManager.Player.transform.position, transform.position);

            var lastMakeType = UIManager.I.MakeTable.MakeTypeOpenList[MakeType.WorkBench];
            if (dis4Player > 3)
                UIManager.I.MakeTable.MakeTypeOpenList[MakeType.WorkBench] = false;
            else
                UIManager.I.MakeTable.MakeTypeOpenList[MakeType.WorkBench] = true;
            if (lastMakeType != UIManager.I.MakeTable.MakeTypeOpenList[MakeType.WorkBench])
                UIManager.I.MakeTable.MakeItemCheck();
        }
    }
}
