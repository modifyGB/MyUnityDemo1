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
        [Header("数值")]
        //攻击数值
        public float attack = 1;
        //攻击距离
        public float attackDistance = 2;
        //攻击范围角度
        public float attackAngle = 45;
        //攻击精度
        public float attackPrecision = 1;
        //攻击速度
        public float attackRate = 1;
        //攻击间隔
        public float attackInterval = 1;
        //攻击消耗耐度
        public float attackDure = 1;
        [Header("设置")]
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
