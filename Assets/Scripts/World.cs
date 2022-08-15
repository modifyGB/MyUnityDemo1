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
    static public double loadPer = 0;
    //柏林噪声
    static public float cellSize = 1;
    static public Vector3 StartOrigin = Vector3.zero;
    static public float scale = 20;
    static public int octaves = 1;
    static public float persistance = 0.5f;
    static public float lacunarity = 0;
    //加载占比
    static public double stepInit = 0.3;
    static public double stepGround = 0.2;
    static public double stepPlace = 0.2;
    static public double stepEnemy = 0.2;

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

        //乱序二维坐标
        double step = stepInit / (mapWidth * mapHeight);
        int[] pointList = new int[mapWidth * mapHeight];
        for (int i = 0; i < pointList.Length; i++)
            pointList[i] = i;

        int start = 0;
        int num = 0;
        int _ = 0;
        for (int i = 0; i < pointList.Length / 2; i++)
        {
            num = new System.Random().Next(start, pointList.Length);
            _ = pointList[num];
            pointList[num] = pointList[start];
            pointList[start] = _;
            start++;
            loadPer += step;
        }
        start = 0;
        loadPer = stepInit;

        //环境初始化
        var map = Utils.CreateNoiseMap(mapWidth, mapHeight, scale, octaves, persistance, lacunarity, seed);
        step = stepGround / (map.GetLength(0) * map.GetLength(1));
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
            {
                CreateGround(archiveObject, map[i, j], i, j);
                loadPer += step;
            }
        loadPer = stepInit + stepGround;

        //物体生成
        step = stepPlace / (pointList.Length / 10);
        for (int i = start; i < start + pointList.Length / 10; i++)
        {
            CreatePlace(archiveObject, pointList[i] / mapWidth, pointList[i] % mapWidth);
            loadPer += step;
        }
        loadPer = stepInit + stepGround + stepPlace;

        //敌人生成
        step = stepEnemy / (pointList.Length / 50);
        for (int i = start; i < start + (pointList.Length / 50) ; i++)
        {
            CreateEnemy(archiveObject, pointList[i] / mapWidth, pointList[i] % mapWidth);
            loadPer += step;
        }
        loadPer = stepInit + stepGround + stepPlace + stepEnemy;

        //人物初始物品生成
        archiveObject.Player.bag.Add(new Item.Serialization(11));
        archiveObject.Player.bag.Add(new Item.Serialization(15));
        archiveObject.Player.bag.Add(new Item.Serialization(19));

        //保存文件
        Utils.SaveObjectAsJson("Archive/" + name + ".json", archiveObject);
        Utils.SaveObjectAsJson("Archive/" + name + ".list.json", archiveData);
        loadPer = 1;
    }
    //环境生成逻辑
    static public void CreateGround(GameManager.GameData archiveObject, float hight, int i, int j)
    {
        if (hight >= 0.5) archiveObject.GridXZ.GridArray[i, j] = new GridObject.Serialization(GridEnvironment.GRASS);
        else if (hight >= 0.4) archiveObject.GridXZ.GridArray[i, j] = new GridObject.Serialization(GridEnvironment.SOIL);
        else archiveObject.GridXZ.GridArray[i, j] = new GridObject.Serialization(GridEnvironment.WATER);
    }
    //物体生成逻辑
    static public void CreatePlace(GameManager.GameData archiveObject, int x, int z)
    {
        var gridObject = archiveObject.GridXZ.GridArray[x, z];
        var random = new System.Random();
        var num = 0;

        if (gridObject.ge == GridEnvironment.GRASS)
            num = random.Next(1, 24);
        else if (gridObject.ge == GridEnvironment.SOIL)
            num = random.Next(10, 20);

        var dir = random.Next(0, 4);

        archiveObject.PlaceList.Add(
            new PlaceObject.Serialization(num, new Vector2Int(x, z), (Dir)dir));
            
    }
    //敌人生成逻辑
    static public void CreateEnemy(GameManager.GameData archiveObject, int x, int z)
    {
        var gridObject = archiveObject.GridXZ.GridArray[x, z];
        var random = new System.Random();

        var num = random.Next(1, 7);
        var angle = random.Next(0, 360);
        if (gridObject.ge != GridEnvironment.WATER)
            archiveObject.EnemyList.Add(new EnemyObject.Serialization(num, new float[3] { x, 0, z }, angle));
    }
}
