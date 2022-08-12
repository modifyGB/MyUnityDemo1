using Bags;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy
{
    [CreateAssetMenu(menuName = "MySO/EnemySO")]
    public class EnemySO : ScriptableObject
    {
        [Header("id")]
        public int num = 0;
        public EnemyObject enemyPrefab;
        [Header("数值")]
        public float maxBlood = 5;
        public float attack = 1;
        public float defence = 0;
        public float experience = 5;
        [Header("移动")]
        public float walkSpeed = 5;
        public float runSpeedPlus = 5;
        public List<Vector3> PatrolPoints;
        [Header("攻击")]
        public float findDistance = 5;
        public float findAngle = 180;
        public float findPrecision = 5;
        public float attackDistance = 1;
        public float attackRate = 5;
        public float attackInterval = 1;
        public float loseDistance = 10;
        [Header("其他")]
        public DropTableSO dropTableSO;
        public bool isBug = false;

        public EnemyObject Create(Vector3 position, Vector3 rotation)
        {
            var newEnemy = GameObject.Instantiate(enemyPrefab, position, Quaternion.Euler(rotation));
            newEnemy.Initialization(this, rotation);
            return newEnemy;
        }
    }
}
