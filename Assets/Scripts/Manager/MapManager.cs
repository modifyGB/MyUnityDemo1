using GridSystem;
using Place;
using System;
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
        public bool isInitialize = false;
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

            if (isInitialize)
            {
                grid = new GridXZ(rowCount, columnCount, cellSize, StartOrigin);
                BuildGround();
            }
            else
            {
                LoadMap();
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
        //加载地图信息并绘制
        public void LoadMap1(string loadPath)
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
        public void LoadMap()
        {
            SocketManager.Socket.Send("GridXZ originPosition", 
                MySocket.SocketSign.GET, new AsyncCallback(StartOriginCallBack));
            SocketManager.Socket.Send("GridXZ Height",
                MySocket.SocketSign.GET, new AsyncCallback(ColumnCountCallBack));
            SocketManager.Socket.Send("GridXZ Width",
                MySocket.SocketSign.GET, new AsyncCallback(RowCountCallBack));
            SocketManager.Socket.Send("GridXZ CellSize",
                MySocket.SocketSign.GET, new AsyncCallback(CellSizeCallBack));
        }
        //回调函数
        public void StartOriginCallBack(IAsyncResult ar)
        {
            var message = SocketManager.Socket.ReceiveCallback(ar);
            if (message == null)
                return;

            print(message.message);
            var oc = message.ToSerialization<float[]>().data;
            StartOrigin = new Vector3(oc[0], oc[1], oc[2]);
        }
        public void ColumnCountCallBack(IAsyncResult ar)
        {
            var message = SocketManager.Socket.ReceiveCallback(ar);
            if (message == null)
                return;

            print(message.message);
            columnCount = message.ToSerialization<int>().data;
        }
        public void RowCountCallBack(IAsyncResult ar)
        {
            var message = SocketManager.Socket.ReceiveCallback(ar);
            if (message == null)
                return;

            print(message.message);
            rowCount = message.ToSerialization<int>().data;
        }
        public void CellSizeCallBack(IAsyncResult ar)
        {
            var message = SocketManager.Socket.ReceiveCallback(ar);
            if (message == null)
                return;

            print(message.message);
            cellSize = message.ToSerialization<float>().data;
        }
    }
}
