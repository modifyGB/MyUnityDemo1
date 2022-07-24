using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        public GameObject enemyBloodPrefab;
        private GameObject enemy;
        public GameObject Enemy { get { return enemy; } }

        public override void Awake()
        {
            base.Awake();

            enemy = new GameObject("Enemy");
        }

        public List<EnemyObject.Serialization> FindAllEnemy()
        {
            var list = new List<EnemyObject.Serialization>();
            for (int i = 0; i < enemy.transform.childCount; i++)
                list.Add(enemy.transform.GetChild(i).GetComponent<EnemyObject>().ToSerialization());
            return list;
        }
    }
}
