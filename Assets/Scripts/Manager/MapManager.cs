using GridSystem;
using Place;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manager
{
    public class MapManager : Singleton<MapManager>
    {
        [Header("浮力")]
        public float p = 1000;
        public float floor = 0;
        public float friction = 0.1f;
        [Header("设置")]
        public bool isDebug = false;
        public string groundLayerName = "Ground";
        [Header("资源")]
        public Buoyancy buoyancyPrefab;
        public Ground groundGrass;
        public Ground groundSoil;
        public Ground groundWater;

        [HideInInspector]
        public GridXZ grid;
        [HideInInspector]
        public Dictionary<int, Dictionary<int, Chest.ChestSerialization>> chestList
            = new Dictionary<int, Dictionary<int, Chest.ChestSerialization>>();
        [HideInInspector]
        public Dictionary<int, Dictionary<int, PlaceObject.Serialization>> placeList
            = new Dictionary<int, Dictionary<int, PlaceObject.Serialization>>();
        [HideInInspector]
        public Transform map;
        [HideInInspector]
        public Transform ground;

        public Action<GridObject> PlayerGridNow;

        public override void Awake()
        {
            base.Awake();

            BuildMap();
        }

        private void Update()
        {
            var playerGrid = grid.GetGridObject(PlayerManager.Player.transform.position);
            PlayerGridNow?.Invoke(playerGrid);
            UpdateGround(playerGrid);
        }
        //更新地面
        public void UpdateGround(GridObject playerGrid)
        {
            for (int i = Mathf.Clamp(playerGrid.x - 30, 0, grid.Width); 
                i < Mathf.Clamp(playerGrid.x + 30, 0, grid.Width); i++)
                for (int j = Mathf.Clamp(playerGrid.z - 30, 0, grid.Height); 
                    j < Mathf.Clamp(playerGrid.z + 30, 0, grid.Height); j++)
                {
                    var gridObject = grid.GridArrayList[i / 100, j / 100][i % 100, j % 100];
                    if (gridObject.groundObject != null)
                        continue;
                    Ground plane = null;
                    if (gridObject.gridEnvironment == GridEnvironment.GRASS)
                        plane = Instantiate(groundGrass);
                    else if (gridObject.gridEnvironment == GridEnvironment.SOIL)
                        plane = Instantiate(groundSoil);
                    else if (gridObject.gridEnvironment == GridEnvironment.WATER)
                        plane = Instantiate(groundWater);
                    plane.Initialization(gridObject);
                }
        }
        //加载放置物
        public void LoadPlace(int blockWidth, int blockHeight)
        {
            foreach (var place in GameManager.I.ArchiveData.mapDataList[blockWidth, blockHeight].PlaceList)
            {
                if (!placeList.ContainsKey(place.origin[0]))
                    placeList.Add(place.origin[0], new Dictionary<int, PlaceObject.Serialization>());
                if (!placeList[place.origin[0]].ContainsKey(place.origin[1]))
                    placeList[place.origin[0]].Add(place.origin[1], place);
            }
        }
        //删除放置物
        public void DeletePlace(int blockWidth, int blockHeight)
        {
            foreach (var place in GameManager.I.ArchiveData.mapDataList[blockWidth, blockHeight].PlaceList)
            {
                if (!placeList.ContainsKey(place.origin[0]))
                    return;
                placeList[place.origin[0]].Remove(place.origin[1]);
            }
        }
        //存储放置物
        public List<PlaceObject.Serialization> DumpPlace(int blockWidth, int blockHeight)
        {
            var ll = new List<PlaceObject.Serialization>();
            var xOrg = blockWidth * 100;
            var yOrg = blockHeight * 100;
            for (int i = 0; i < 100; i++)
            {
                if (!placeList.ContainsKey(i + xOrg))
                    continue;
                var item1 = placeList[i + xOrg];
                for (int j = 0; j < 100; j++)
                {
                    if (!item1.ContainsKey(j + yOrg))
                        continue;
                    var item2 = item1[j + yOrg];
                    ll.Add(item2);
                }
            }
            return ll;
        }
        //加载宝箱
        public void LoadChest(int blockWidth, int blockHeight)
        {
            foreach (var chest in GameManager.I.ArchiveData.mapDataList[blockWidth, blockHeight].ChestList)
            {
                if (!chestList.ContainsKey(chest.place.origin[0]))
                    chestList.Add(chest.place.origin[0], new Dictionary<int, Chest.ChestSerialization>());
                chestList[chest.place.origin[0]].Add(chest.place.origin[1], chest);
            }
        }
        //存储宝箱
        public List<Chest.ChestSerialization> DumpChest(int blockWidth, int blockHeight)
        {
            var ll = new List<Chest.ChestSerialization>();
            var xOrg = blockWidth * 100;
            var yOrg = blockHeight * 100;
            for (int i = 0; i < 100; i++)
            {
                if (!chestList.ContainsKey(i + xOrg))
                    continue;
                var item1 = chestList[i + xOrg];
                for (int j = 0; j < 100; j++)
                {
                    if (!item1.ContainsKey(j + yOrg))
                        continue;
                    var item2 = item1[j + yOrg];
                    ll.Add(item2);
                }
            }
            return ll;
        }
        //删除宝箱
        public void DeleteChest(int blockWidth, int blockHeight)
        {
            foreach (var chest in GameManager.I.ArchiveData.mapDataList[blockWidth, blockHeight].ChestList)
            {
                if (!chestList.ContainsKey(chest.place.origin[0]))
                    return;
                chestList[chest.place.origin[0]].Remove(chest.place.origin[1]);
            }
        }
        //添加物体
        public void AddPlace(PlaceObject placeObject)
        {
            if (!placeList.ContainsKey(placeObject.Origin[0]))
                placeList.Add(placeObject.Origin[0], new Dictionary<int, PlaceObject.Serialization>());
            placeList[placeObject.Origin[0]].Add(placeObject.Origin[1], placeObject.ToSerialization());
            if (placeObject.GetType() != typeof(Chest))
                return;
            var chestObject = (Chest) placeObject;
            if (!chestList.ContainsKey(chestObject.Origin[0]))
                chestList.Add(chestObject.Origin[0], new Dictionary<int, Chest.ChestSerialization>());
            chestList[chestObject.Origin[0]].Add(chestObject.Origin[1], chestObject.ToChestSerialization());
        }
        //加载地图信息并绘制
        public void BuildMap()
        {
            var gridSerialization = GameManager.I.ArchiveData.GridXZ;
            var cellSize = gridSerialization.CellSize;
            var rowCount = gridSerialization.Width;
            var columnCount = gridSerialization.Height;
            var StartOrigin = new Vector3(gridSerialization.originPosition[0], 
                gridSerialization.originPosition[1], gridSerialization.originPosition[2]);

            grid = new GridXZ(rowCount, columnCount, cellSize, StartOrigin);

            map = new GameObject("Map").transform;
            map.transform.localPosition = Vector3.zero;
            ground = new GameObject("Ground").transform;
            ground.localPosition = Vector3.zero;
            ground.parent = map.transform;
            var buoyancy = Instantiate(buoyancyPrefab);
            buoyancy.Initialization(rowCount, columnCount);
            buoyancy.transform.SetParent(map, false);
            BuildWall(rowCount, columnCount);

            int x = (int)(GameManager.I.ArchiveData.Player.position[0] / 100);
            int y = (int)(GameManager.I.ArchiveData.Player.position[2] / 100);
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (i < 0 || i >= grid.GridArrayList.GetLength(0))
                    continue;
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (j < 0 || j >= grid.GridArrayList.GetLength(1))
                        continue;
                    LoadBlock(i, j);
                }

            }
        }
        //加载Ground信息
        public void LoadGround(int blockWidth, int blockHeight)
        {
            var mapData = GameManager.I.ArchiveData.mapDataList[blockWidth, blockHeight];
            grid.GridArrayList[blockWidth, blockHeight] =
                new GridObject[mapData.GridArray.GetLength(0), mapData.GridArray.GetLength(1)];
            var gridArray = grid.GridArrayList[blockWidth, blockHeight];
            for (int i=0;i< mapData.GridArray.GetLength(0); i++)
                for (int j = 0; j < mapData.GridArray.GetLength(1); j++)
                {
                    gridArray[i, j] = new GridObject(grid, blockWidth * 100 + i, blockHeight * 100 + j);
                    gridArray[i, j].gridEnvironment = mapData.GridArray[i, j].ge;
                }
        }
        //加载Block信息
        public void LoadBlock(int blockWidth, int blockHeight)
        {
            LoadGround(blockWidth, blockHeight);
            LoadPlace(blockWidth, blockHeight);
            LoadChest(blockWidth, blockHeight);
            EnemyManager.I.LoadEnemy(blockWidth, blockHeight);
        }
        //删除Block信息
        public void DeleteBlock(int blockWidth, int blockHeight)
        {
            grid.GridArrayList[blockWidth, blockHeight] = null;
            DeletePlace(blockWidth, blockHeight);
            DeleteChest(blockWidth, blockHeight);
            EnemyManager.I.DeleteEnemy(blockWidth, blockHeight);
        }
        //设置墙体
        public void BuildWall(int width, int height)
        {
            var wall = new GameObject("Wall");
            var col = wall.AddComponent<BoxCollider>();
            wall.transform.SetParent(map, false);
            wall.transform.localPosition = Vector3.zero;
            col.size = new Vector3(width, 10, 0.1f);
            col.center = new Vector3(width / 2, 0, 0);

            wall = new GameObject("Wall");
            col = wall.AddComponent<BoxCollider>();
            wall.transform.SetParent(map, false);
            wall.transform.localPosition = Vector3.zero;
            col.size = new Vector3(width, 10, 0.1f);
            col.center = new Vector3(width / 2, 0, height);

            wall = new GameObject("Wall");
            col = wall.AddComponent<BoxCollider>();
            wall.transform.SetParent(map, false);
            wall.transform.localPosition = Vector3.zero;
            col.size = new Vector3(0.1f, 10, height);
            col.center = new Vector3(0, 0, height / 2);

            wall = new GameObject("Wall");
            col = wall.AddComponent<BoxCollider>();
            wall.transform.SetParent(map, false);
            wall.transform.localPosition = Vector3.zero;
            col.size = new Vector3(0.1f, 10, height);
            col.center = new Vector3(width, 0, height / 2);
        }
    }
}
