using Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manager
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        public GameObject enemyBloodPrefab;
        private Transform enemy;
        public Transform Enemy { get { return enemy; } }
        private Dictionary<int, Dictionary<int, EnemyObject.Serialization>> enemyList
            = new Dictionary<int, Dictionary<int, EnemyObject.Serialization>>();
        public Dictionary<int, Dictionary<int, EnemyObject.Serialization>>
            EnemyList { get { return enemyList; } }

        public override void Awake()
        {
            base.Awake();

            enemy = new GameObject("Enemy").transform;
            LoadEnemy();
        }

        public List<EnemyObject.Serialization> DumpEnemy()
        {
            var list = new List<EnemyObject.Serialization>();
            for (int i = 0; i < enemyList.Count; i++)
            {
                var item1 = enemyList.ElementAt(i);
                for (int j = 0; j < item1.Value.Count; j++)
                {
                    var item2 = item1.Value.ElementAt(j);
                    list.Add(item2.Value);
                }
            }
            return list;
        }

        public void LoadEnemy()
        {
            foreach (var e in GameManager.I.ArchiveObject.EnemyList)
            {
                var xz = MapManager.I.grid.GetXZ(new Vector3(e.position[0], e.position[1], e.position[2]));
                if (!enemyList.ContainsKey(xz.x))
                    enemyList.Add(xz.x, new Dictionary<int, EnemyObject.Serialization>());
                enemyList[xz.x].Add(xz.y, e);
            }
        }
    }
}
