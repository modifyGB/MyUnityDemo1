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
            //LoadEnemy();
        }

        public List<EnemyObject.Serialization> DumpEnemy(int blockWidth, int blockHeight)
        {
            var list = new List<EnemyObject.Serialization>();
            var xOrg = blockWidth * 100;
            var yOrg = blockHeight * 100;
            for (int i = 0; i < 100; i++)
            {
                if (!enemyList.ContainsKey(i + xOrg))
                    continue;
                var item1 = enemyList[i + xOrg];
                for (int j = 0; j < 100; j++)
                {
                    if (!item1.ContainsKey(j + yOrg))
                        continue;
                    var item2 = item1[j + yOrg];
                    list.Add(item2);
                }
            }
            return list;
        }

        public void LoadEnemy(int blockWidth, int blockHeight)
        {
            foreach (var e in GameManager.I.ArchiveData.mapDataList[blockWidth, blockHeight].EnemyList)
            {
                var xz = MapManager.I.grid.GetXZ(new Vector3(e.position[0], e.position[1], e.position[2]));
                if (!enemyList.ContainsKey(xz.x))
                    enemyList.Add(xz.x, new Dictionary<int, EnemyObject.Serialization>());
                enemyList[xz.x].Add(xz.y, e);
            }
        }

        public void DeleteEnemy(int blockWidth, int blockHeight)
        {
            foreach (var e in GameManager.I.ArchiveData.mapDataList[blockWidth, blockHeight].EnemyList)
            {
                var xz = MapManager.I.grid.GetXZ(new Vector3(e.position[0], e.position[1], e.position[2]));
                if (!enemyList.ContainsKey(xz.x))
                    return;
                enemyList[xz.x].Remove(xz.y);
            }
        }
    }
}
