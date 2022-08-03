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
        [Header("设置")]
        public bool isDebug = false;
        public string groundLayerName = "Ground";
        [Header("资源")]
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

            LoadMap();

            if (isDebug)
                grid.InitializeDebug();
        }

        private void Update()
        {
            var playerGrid = grid.GetGridObject(PlayerManager.Player.transform.position);
            PlayerGridNow?.Invoke(playerGrid);
            UpdateGround(playerGrid);
        }
        //绘制地面
        public void BuildGround()
        {
            map = new GameObject("Map").transform;
            map.transform.localPosition = Vector3.zero;
            ground = new GameObject("Ground").transform;
            ground.localPosition = Vector3.zero;
            ground.parent = map.transform;

            var player = GameManager.I.ArchiveObject.Player;
            var playerGrid = grid.GetGridObject(new Vector3(player.position[0], 0, player.position[2]));
            UpdateGround(playerGrid);
        }
        //更新地面
        public void UpdateGround(GridObject playerGrid)
        {
            for (int i = Mathf.Clamp(playerGrid.x - 30, 0, grid.Width - 1) ; 
                i < Mathf.Clamp(playerGrid.x + 30, 0, grid.Width - 1); i++)
                for (int j = Mathf.Clamp(playerGrid.z - 30, 0, grid.Height - 1); 
                    j < Mathf.Clamp(playerGrid.z + 30, 0, grid.Height - 1); j++)
                {
                    var gridObject = grid.GridArray[i, j];
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
        public void LoadPlace()
        {
            foreach (var place in GameManager.I.ArchiveObject.PlaceList)
            {
                if (!placeList.ContainsKey(place.origin[0]))
                    placeList.Add(place.origin[0], new Dictionary<int, PlaceObject.Serialization>());
                placeList[place.origin[0]].Add(place.origin[1], place);
            }
        }
        //存储放置物
        public List<PlaceObject.Serialization> DumpPlace()
        {
            var ll = new List<PlaceObject.Serialization>();
            for (int i = 0; i < placeList.Count; i++)
            {
                var item1 = placeList.ElementAt(i);
                for (int j = 0; j < item1.Value.Count; j++)
                {
                    var item2 = item1.Value.ElementAt(i);
                    ll.Add(item2.Value);
                }
            }
            return ll;
        }
        //加载宝箱
        public void LoadChest()
        {
            foreach (var chest in GameManager.I.ArchiveObject.ChestList)
            {
                if (!chestList.ContainsKey(chest.place.origin[0]))
                    chestList.Add(chest.place.origin[0], new Dictionary<int, Chest.ChestSerialization>());
                chestList[chest.place.origin[0]].Add(chest.place.origin[1], chest);
            }
        }
        //存储宝箱
        public List<Chest.ChestSerialization> DumpChest()
        {
            var ll = new List<Chest.ChestSerialization>();
            for (int i = 0; i < chestList.Count; i++)
            {
                var item1 = chestList.ElementAt(i);
                for (int j = 0; j < item1.Value.Count; j++)
                {
                    var item2 = item1.Value.ElementAt(i);
                    ll.Add(item2.Value);
                }
            }
            return ll;
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
        public void LoadMap()
        {
            var gridSerialization = GameManager.I.ArchiveObject.GridXZ;
            var cellSize = gridSerialization.CellSize;
            var rowCount = gridSerialization.Width;
            var columnCount = gridSerialization.Height;
            var StartOrigin = new Vector3(gridSerialization.originPosition[0], 
                gridSerialization.originPosition[1], gridSerialization.originPosition[2]);

            grid = new GridXZ(rowCount, columnCount, cellSize, StartOrigin);
            for (int x = 0; x < gridSerialization.Width; x++)
                for (var y = 0; y < gridSerialization.Height; y++)
                    grid.GridArray[x, y].gridEnvironment = gridSerialization.GridArray[x, y].ge;

            LoadPlace();
            LoadChest();
            BuildGround();
        }
    }
}
