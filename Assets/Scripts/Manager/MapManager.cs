using GridSystem;
using Place;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class MapManager : Singleton<MapManager>
    {
        [Header("初始化设置")]
        public float cellSize = 2;
        public int rowCount = 100;
        public int columnCount = 100;
        public Vector3 StartOrigin = Vector3.zero;
        public string loadPath = "";
        [Header("设置")]
        public bool isDebug = false;
        public string groundLayerName = "Ground";
        [Header("资源")]
        public Material planeMaterial;

        [HideInInspector]
        public GridXZ grid;
        [HideInInspector]
        public Transform map;

        public override void Awake()
        {
            base.Awake();

            if (loadPath == "")
            {
                grid = new GridXZ(rowCount, columnCount, cellSize, StartOrigin);
                BuildGround();
            }
            else
            {
                LoadMap(loadPath);
            }

            if (isDebug)
                grid.InitializeDebug();
        }
        //绘制地面
        public void BuildGround()
        {
            map = new GameObject("Map").transform;
            map.transform.localPosition = Vector3.zero;
            var ground = new GameObject("Ground");
            ground.transform.localPosition = Vector3.zero;
            ground.transform.parent = map.transform;

            foreach (var gridObject in grid.GridArray)
            {
                var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.parent = ground.transform;
                plane.transform.localScale = new Vector3(grid.CellSize / 10f, 1, grid.CellSize / 10f);
                var pos = gridObject.GetWorldPosition();
                plane.transform.position = new Vector3(pos.x + grid.CellSize / 2, 0, pos.z + grid.CellSize / 2);
                plane.GetComponent<MeshRenderer>().material = planeMaterial;
                plane.layer = LayerMask.NameToLayer(groundLayerName);
            }
        }
        //绘制放置物
        public void BuildPlaceableObject(List<PlaceObject.Serialization> pList)
        {
            foreach (PlaceObject.Serialization o in pList)
            {
                var po = GameManager.I.placeTableSO.table[o.num];
                var newPO = po.Create(new Vector2Int(o.origin[0], o.origin[1]), o.dir);
                newPO.transform.SetParent(map);
            }
        }
        //保存地图信息
        public void SaveMap(string savePath)
        {
            Utils.SaveObjectAsJson(savePath, grid.ToSerialization());
        }
        //加载地图信息并绘制
        public void LoadMap(string loadPath)
        {
            var gridSerialization = Utils.LoadObject<GridXZ.Serialization>(loadPath);
            if (gridSerialization.PlaceableArray == null)
                return;

            cellSize = gridSerialization.CellSize;
            rowCount = gridSerialization.Width;
            columnCount = gridSerialization.Height;
            StartOrigin = new Vector3(gridSerialization.originPosition[0], 
                gridSerialization.originPosition[1], gridSerialization.originPosition[2]);

            grid = new GridXZ(rowCount, columnCount, cellSize, StartOrigin);
            for (int x = 0; x < gridSerialization.Width; x++)
                for (var y = 0; y < gridSerialization.Height; y++)
                    grid.GridArray[x, y].gridEnvironment = gridSerialization.GridArray[x, y].ge;

            BuildGround();
            BuildPlaceableObject(gridSerialization.PlaceableArray);
        }
    }
}
