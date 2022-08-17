using Enemy;
using GridSystem;
using Items;
using Manager;
using Place;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    //创建世界
    static public void CreateWorld(object data)
    {
        loadPer = 0;
        var archiveData = (ArchiveData) data;
        var name = archiveData.name;
        var mapWidth = archiveData.width;
        var mapHeight = archiveData.height;
        var seed = archiveData.seed;
        archiveData.GridXZ = new GridXZ.Serialization(mapWidth, mapHeight, cellSize, StartOrigin);
        if (!Directory.Exists(Path.Combine(Utils.originSavePath, "Archive/" + name)))
            Directory.CreateDirectory(Path.Combine(Utils.originSavePath, "Archive/" + name));

        //人物初始化
        archiveData.Player = new PlayerMessage(new float[3] { mapWidth / 2, 0, mapHeight / 2 });
        archiveData.Player.bag.Add(new Item.Serialization(10));
        archiveData.Player.bag.Add(new Item.Serialization(15));
        archiveData.Player.bag.Add(new Item.Serialization(19));
        Utils.SaveObjectAsJson("Archive/" + name + "/archiveData.json", archiveData);

        //柏林噪声初始化
        var noiseMap = CreateNoiseMap(mapWidth, mapHeight, seed, 0.1);
        loadPer = 0.1;

        //环境初始化
        var blockWidth = Mathf.Ceil(mapWidth / 100f);
        var blockHeight = Mathf.Ceil(mapHeight / 100f);
        var blockStep = 0.9 / (blockWidth * blockHeight);
        for (int i = 0; i < blockWidth; i++)
        {
            var width = 100;
            if (i == blockWidth - 1)
                width = mapWidth - i * 100;

            for (int j = 0; j < blockHeight; j++)
            {
                var height = 100;
                if (j == blockHeight - 1)
                    height = mapHeight - j * 100;

                var mapData = CreateMap(width, height, i, j, noiseMap, blockStep);
                Utils.SaveObjectAsJson("Archive/" + name + "/" + i + "-" + j + ".json", mapData);
            }
        }
        loadPer = 1;
    }
    //创建二维柏林噪声
    static public float[,] CreateNoiseMap(int mapWidth, int mapHeight, int seed, double loadStep)
    {
        System.Random rand = new System.Random(seed);
        float xOrg = rand.Next(0, mapWidth);
        float yOrg = rand.Next(0, mapHeight);
        var step = loadStep / (mapWidth * mapHeight) / 2;

        var map = new float[mapWidth, mapHeight];
        float maxValue = float.MinValue;
        float minValue = float.MaxValue;

        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float xCoord = xOrg + x / scale * frequency;
                    float yCoord = yOrg + y / scale * frequency;
                    float perlinValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxValue)
                    maxValue = noiseHeight;
                else if (noiseHeight < minValue)
                    minValue = noiseHeight;
                map[x, y] = noiseHeight;

                loadPer += step;
            }

        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                map[x, y] = Mathf.InverseLerp(minValue, maxValue, map[x, y]);
                loadPer += step;
            }

        return map;
    }
    //Map区块生成逻辑
    static public MapData CreateMap(int mapWidth, int mapHeight, int blockWidth, int blockHeight, float[,] noiseMap, double loadStep)
    {
        //初始化
        var mapData = default(MapData);
        mapData.EnemyList = new List<EnemyObject.Serialization>();
        mapData.ChestList = new List<Chest.ChestSerialization>();
        mapData.PlaceList = new List<PlaceObject.Serialization>();
        mapData.GridArray = new GridObject.Serialization[mapWidth, mapHeight];

        //乱序二维坐标
        double step = loadStep * 0.3 / (mapWidth * mapHeight);
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

        //环境初始化
        step = loadStep * 0.3 / (mapWidth * mapHeight);
        var xOrg = blockWidth * 100;
        var yOrg = blockHeight * 100;
        for (int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
            {
                CreateGround(mapData, noiseMap[xOrg + i, yOrg + j], i, j);
                loadPer += step;
            }

        //宝箱生成
        for (int i = start; i < start + pointList.Length / 2000; i++)
            CreateChest(mapData, blockWidth, blockHeight, pointList[i] / mapWidth, pointList[i] % mapWidth);
        start += pointList.Length / 2000;

        //物体生成
        step = loadStep * 0.2 / (pointList.Length / 10);
        for (int i = start; i < start + pointList.Length / 20; i++)
        {
            CreatePlace(mapData, blockWidth, blockHeight, pointList[i] / mapWidth, pointList[i] % mapWidth);
            loadPer += step;
        }
        start += pointList.Length / 20;

        //敌人生成
        step = loadStep * 0.2 / (pointList.Length / 50);
        for (int i = start; i < start + (pointList.Length / 80); i++)
        {
            CreateEnemy(mapData, blockWidth, blockHeight, pointList[i] / mapWidth, pointList[i] % mapWidth);
            loadPer += step;
        }

        return mapData;
    }
    //环境生成逻辑
    static public void CreateGround(MapData archiveObject, float hight, int i, int j)
    {
        if (hight >= 0.5) archiveObject.GridArray[i, j] = new GridObject.Serialization(GridEnvironment.GRASS);
        else if (hight >= 0.4) archiveObject.GridArray[i, j] = new GridObject.Serialization(GridEnvironment.SOIL);
        else archiveObject.GridArray[i, j] = new GridObject.Serialization(GridEnvironment.WATER);
    }
    //物体生成逻辑
    static public void CreatePlace(MapData archiveObject, int blockWidth, int blockHeight, int x, int z)
    {
        var gridObject = archiveObject.GridArray[x, z];
        var random = new System.Random();
        var num = 0;

        if (gridObject.ge == GridEnvironment.GRASS)
            num = random.Next(1, 24);
        else if (gridObject.ge == GridEnvironment.SOIL)
            num = random.Next(10, 20);
        var dir = random.Next(0, 4);

        if (num == 0)
            return;

        archiveObject.PlaceList.Add(
            new PlaceObject.Serialization(num, new Vector2Int(
                blockWidth * 100 + x, blockHeight * 100 + z), (Dir)dir));
            
    }
    //敌人生成逻辑
    static public void CreateEnemy(MapData archiveObject, int blockWidth, int blockHeight, int x, int z)
    {
        var gridObject = archiveObject.GridArray[x, z];
        var random = new System.Random();

        var num = random.Next(1, 7);
        var angle = random.Next(0, 360);
        if (gridObject.ge != GridEnvironment.WATER)
            archiveObject.EnemyList.Add(new EnemyObject.Serialization(
                num, new float[3] { blockWidth * 100 + x, 0, blockHeight * 100 + z }, angle));
    }
    //宝箱生成逻辑
    static public void CreateChest(MapData archiveObject, int blockWidth, int blockHeight, int x, int z)
    {
        var gridObject = archiveObject.GridArray[x, z];
        var random = new System.Random();
        var dir = random.Next(0, 4);

        if (gridObject.ge != GridEnvironment.WATER)
        {
            var po = new PlaceObject.Serialization(30, new Vector2Int(
                    blockWidth * 100 + x, blockHeight * 100 + z), (Dir)dir);
            archiveObject.PlaceList.Add(po);
            archiveObject.ChestList.Add(new Chest.ChestSerialization(po, null));
        }
    }
}
