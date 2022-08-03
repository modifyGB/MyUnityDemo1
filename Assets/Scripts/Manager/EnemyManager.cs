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

        private void Start()
        {
            LoadEnemy();
        }

        public List<EnemyObject.Serialization> FindAllEnemy()
        {
            var list = new List<EnemyObject.Serialization>();
            for (int i = 0; i < enemy.transform.childCount; i++)
                list.Add(enemy.transform.GetChild(i).GetComponent<EnemyObject>().ToSerialization());
            return list;
        }

        public void LoadEnemy()
        {
            if (GameManager.I.isInitialize)
                return;
            var el = GameManager.I.ArchiveObject.EnemyList;
            var et = GameManager.I.enemyTableSO.table;
            foreach (var e in el)
            {
                var po = new Vector3(e.position[0], e.position[1], e.position[2]);
                var ro = new Vector3(0, e.angle, 0);
                et[e.num].Create(po, ro);
            }
        }
    }
}
