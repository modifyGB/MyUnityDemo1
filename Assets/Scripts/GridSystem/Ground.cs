using Enemy;
using Manager;
using Place;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class Ground : MyScript
    {
        public GridObject gridObject;
        public EnemyObject enemyObject;

        public void Initialization(GridObject gridObject)
        {
            this.gridObject = gridObject;
            gridObject.groundObject = this;
            MapManager.I.PlayerGridNow += CheckDestory;
            transform.parent = MapManager.I.ground;
            var pos = gridObject.GetWorldPosition();
            transform.position = new Vector3(pos.x + 0.5f, 0, pos.z + 0.5f);
            gameObject.layer = LayerMask.NameToLayer(MapManager.I.groundLayerName);

            CreatePlace();
            CreateEnemy();
        }

        public void CheckDestory(GridObject playerGrid)
        {
            if (Mathf.Abs(playerGrid.x - gridObject.x) > 50
                || Mathf.Abs(playerGrid.z - gridObject.z) > 50)
                DestroySelf();
        }

        public override void DestroySelf()
        {
            gridObject.groundObject = null;
            MapManager.I.PlayerGridNow -= CheckDestory;
            if (gridObject.PlaceObject != null)
                gridObject.PlaceObject.DestroySelf();
            if (enemyObject != null)
                enemyObject.DestroySelf();

            base.DestroySelf();
        }

        public void CreatePlace()
        {
            if (!MapManager.I.placeList.ContainsKey(gridObject.x) 
                || !MapManager.I.placeList[gridObject.x].ContainsKey(gridObject.z))
                return;
            var place = MapManager.I.placeList[gridObject.x][gridObject.z];
            var so = GameManager.I.placeTableSO.table[place.num];
            if (!so.BuildCheck(new Vector2Int(gridObject.x, gridObject.z), place.dir))
            {
                MapManager.I.placeList[gridObject.x].Remove(gridObject.z);
                if (MapManager.I.chestList.ContainsKey(gridObject.x)
                && MapManager.I.chestList[gridObject.x].ContainsKey(gridObject.z))
                    MapManager.I.chestList[gridObject.x].Remove(gridObject.z);
                return;
            }

            var o = MapManager.I.placeList[gridObject.x][gridObject.z];
            var po = GameManager.I.placeTableSO.table[o.num];
            PlaceObject newPO;
            if (po.GetType() == typeof(ChestPlaceSO))
                newPO = ((ChestPlaceSO)po).Create(new Vector2Int(o.origin[0], o.origin[1]), o.dir);
            else
                newPO = po.Create(new Vector2Int(o.origin[0], o.origin[1]), o.dir);
            newPO.transform.SetParent(MapManager.I.map);

            if (!MapManager.I.chestList.ContainsKey(gridObject.x)
                || !MapManager.I.chestList[gridObject.x].ContainsKey(gridObject.z))
                return;
            ((Chest)newPO).InitializationBag(MapManager.I.chestList[gridObject.x][gridObject.z].bag);
        }

        public void CreateEnemy()
        {
            if (!EnemyManager.I.EnemyList.ContainsKey(gridObject.x)
                || !EnemyManager.I.EnemyList[gridObject.x].ContainsKey(gridObject.z))
                return;
            if (!gridObject.CanBuild)
            {
                EnemyManager.I.EnemyList[gridObject.x].Remove(gridObject.z);
                return;
            }

            var o = EnemyManager.I.EnemyList[gridObject.x][gridObject.z];
            var es = GameManager.I.enemyTableSO.table[o.num];
            var po = new Vector3(o.position[0], o.position[1], o.position[2]);
            var ro = new Vector3(0, o.angle, 0);
            enemyObject = es.Create(po, ro);
        }
    }
}
