using Enemy;
using GridSystem;
using Items;
using Manager;
using Place;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class World
{
    //存档
    static public string archiveName;
    static public float loadPer = 0;
    //柏林噪声
    static public float cellSize = 1;
    static public Vector3 StartOrigin = Vector3.zero;
    static public float scale = 20;
    static public int octaves = 1;
    static public float persistance = 0.5f;
    static public float lacunarity = 0;
    //加载占比
    static public float stepGround = 0.3f;
    static public float stepPlace = 0.3f;
    static public float stepEnemy = 0.3f;

    //创建世界
    static public void CreateWorld(object data)
    {
        loadPer = 0;
        var archiveData = (ArchiveData) data;
        var name = archiveData.name;
        var mapWidth = archiveData.width;
        var mapHeight = archiveData.height;
        var seed = archiveData.seed;
        //初始化
        var archiveObject = default(GameManager.GameData);
        archiveObject.Player = new PlayerMessage(new float[3] { mapWidth / 2, 0, mapHeight / 2 });
        archiveObject.GridXZ = new GridXZ.Serialization(mapWidth, mapHeight, cellSize, StartOrigin);
        archiveObject.EnemyList = new List<EnemyObject.Serialization>();
        archiveObject.ChestList = new List<Chest.ChestSerialization>();
        archiveObject.PlaceList = new List<PlaceObject.Serialization>();
        //环境初始化
        var map = Utils.CreateNoiseMap(mapWidth, mapHeight, scale, octaves, persistance, lacunarity, seed);
        CreateGround(archiveObject, map);
        //物体生成
        var placeList = Utils.GetRandomPoint2(mapWidth, mapHeight, mapWidth * mapHeight / 100);
        var step = stepPlace / placeList.Count;
        foreach (var x in placeList)
        {
            CreatePlace(archiveObject, x[0], x[1]);
            loadPer += step;
        }
        //敌人生成
        var enemyList = Utils.GetRandomPoint2(mapWidth, mapHeight, mapWidth * mapHeight / 500);
        step = stepEnemy / enemyList.Count;
        foreach (var x in enemyList)
        {
            CreateEnemy(archiveObject, x[0], x[1]);
            loadPer += step;
        }
        //人物初始物品生成
        archiveObject.Player.bag.Add(new Item.Serialization(2));
        archiveObject.Player.bag.Add(new Item.Serialization(4, 10));
        //保存文件
        Utils.SaveObjectAsJson("Archive/" + name + ".json", archiveObject);
        Utils.SaveObjectAsJson("Archive/" + name + ".list.json", archiveData);
        loadPer = 1;
    }
    //环境生成逻辑
    static public void CreateGround(GameManager.GameData archiveObject, float[,] map)
    {
        float step = stepGround / (map.GetLength(0) * map.GetLength(1));
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] >= 0.5) archiveObject.GridXZ.GridArray[i, j] = new GridObject.Serialization(GridEnvironment.GRASS);
                else if (map[i, j] >= 0.4) archiveObject.GridXZ.GridArray[i, j] = new GridObject.Serialization(GridEnvironment.SOIL);
                else archiveObject.GridXZ.GridArray[i, j] = new GridObject.Serialization(GridEnvironment.WATER);
                loadPer += step;
            }
    }
    //物体生成逻辑
    static public void CreatePlace(GameManager.GameData archiveObject, int x, int z)
    {
        var gridObject = archiveObject.GridXZ.GridArray[x, z];
        if (gridObject.ge == GridEnvironment.GRASS)
            archiveObject.PlaceList.Add(
                new PlaceObject.Serialization(1, new Vector2Int(x, z), Dir.Down));
    }
    //敌人生成逻辑
    static public void CreateEnemy(GameManager.GameData archiveObject, int x, int z)
    {
        var gridObject = archiveObject.GridXZ.GridArray[x, z];
        if (gridObject.ge != GridEnvironment.WATER)
            archiveObject.EnemyList.Add(new EnemyObject.Serialization(1, new float[3] { x, 0, z }, 0));
    }
}
