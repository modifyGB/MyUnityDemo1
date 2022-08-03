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

        public void Initialization(GridObject gridObject)
        {
            this.gridObject = gridObject;
            gridObject.groundObject = this;
            MapManager.I.PlayerGridNow += CheckDestory;
            transform.parent = MapManager.I.ground;
            var pos = gridObject.GetWorldPosition();
            transform.position = new Vector3(pos.x, 0, pos.z);
            gameObject.layer = LayerMask.NameToLayer(MapManager.I.groundLayerName);

            CreatePlace();
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

            base.DestroySelf();
        }

        public void CreatePlace()
        {
            if (!MapManager.I.placeList.ContainsKey(gridObject.x) 
                || !MapManager.I.placeList[gridObject.x].ContainsKey(gridObject.z))
                return;
            if (!gridObject.CanBuild)
            {
                MapManager.I.placeList[gridObject.x].Remove(gridObject.z);
                if (MapManager.I.chestList.ContainsKey(gridObject.x)
                && MapManager.I.chestList[gridObject.x].ContainsKey(gridObject.z))
                    MapManager.I.chestList[gridObject.x].Remove(gridObject.z);
                return;
            }

            var o = MapManager.I.placeList[gridObject.x][gridObject.z];
            var po = GameManager.I.placeTableSO.table[o.num];
            var newPO = po.Create(new Vector2Int(o.origin[0], o.origin[1]), o.dir);
            newPO.transform.SetParent(MapManager.I.map);

            if (!MapManager.I.chestList.ContainsKey(gridObject.x)
                || !MapManager.I.chestList[gridObject.x].ContainsKey(gridObject.z))
                return;
            ((Chest)newPO).InitializationBag(MapManager.I.chestList[gridObject.x][gridObject.z].bag);
        }
    }
}
