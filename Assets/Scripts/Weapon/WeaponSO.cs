using Enemy;
using Manager;
using Place;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapon
{
    [CreateAssetMenu(menuName = "MySO/WeaponSO")]
    public class WeaponSO : MyScriptable
    {
        [Header("id")]
        public int num = 0;
        public WeaponObject weaponPrefab;
        [Header("��ֵ")]
        //������ֵ
        public float attack = 1;
        //��������
        public float attackDistance = 2;
        //������Χ�Ƕ�
        public float attackAngle = 45;
        //��������
        public float attackPrecision = 1;
        //�����ٶ�
        public float attackRate = 1;
        //�������
        public float attackInterval = 1;
        //���������Ͷ�
        public float attackDure = 1;
        [Header("����")]
        public int attackAnim = 0;

        public WeaponObject CreateWeapon()
        {
            var x = GameObject.Instantiate(weaponPrefab);
            x.Initialization(this);
            return x;
        }

        public void AttackToPlace(PlaceObject placeObject)
        {
            var bag = PlayerManager.I.PlayerBag;
            if (bag.Bag.ItemList[bag.UseNum].itemSO.isDurable)
                bag.Bag.ItemList[bag.UseNum].Dure -= attackDure;
            placeObject.BeAttack(this);
        }

        public void AttackToEnemy(EnemyObject enemyObject)
        {
            var bag = PlayerManager.I.PlayerBag;
            if (bag.Bag.ItemList[bag.UseNum].itemSO.isDurable)
                bag.Bag.ItemList[bag.UseNum].Dure -= attackDure;
            enemyObject.BeAttack(this);
        }
    }
}
