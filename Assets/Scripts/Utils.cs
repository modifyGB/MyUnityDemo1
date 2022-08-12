using Bags;
using Items;
using Newtonsoft.Json;
using Place;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//工具类
public static class Utils
{
    public static string originSavePath = Application.persistentDataPath;

    //返回鼠标指向地面的坐标
    public static Vector3 MouseToTerrainPosition(string layerName)
    {
        Vector3 position = Vector3.zero;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit info, 10000, LayerMask.GetMask(layerName)))
            position = info.point;
        return position;
    }
    //返回鼠标指向物体的信息
    public static RaycastHit CameraRay()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit info))
            return info;
        return new RaycastHit();
    }
    //使用名字查找子物体
    public static GameObject FindChildByName(GameObject parent, string name)
    {
        foreach (var child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.name == name)
            {
                return child.gameObject;
            }
        }
        return null;
    }
    //将meterial应用到物体及子物体上
    public static void SetMaterial(GameObject go, Material material)
    {
        var meshes = go.GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in meshes)
            mesh.material = material;
    }
    //使用json保存物体
    public static void SaveObjectAsJson(string savePath, object saveData)
    {
        string json = JsonConvert.SerializeObject(saveData);
        var allSavePath = Path.Combine(originSavePath, savePath);
        var fileStream = new FileStream(allSavePath, FileMode.Create);

        using (var writer = new StreamWriter(fileStream))
            writer.Write(json);
    }
    //读取物体
    public static TSaveObject LoadObject<TSaveObject>(string savePath)
    {
        var allSavePath = Path.Combine(originSavePath, savePath);
        if (File.Exists(allSavePath))
            using (var reader = new StreamReader(allSavePath))
            {
                var saveData = reader.ReadToEnd();
                if (saveData == null)
                    return default;
                return JsonConvert.DeserializeObject<TSaveObject>(saveData);
            }
        return default;
    }
    //删除文件
    public static void DeleteFile(string deletePath)
    {
        var allSavePath = Path.Combine(originSavePath, deletePath);
        if (File.Exists(allSavePath))
            File.Delete(allSavePath);
    }
    //返回路径下的文件
    public static FileInfo[] FindFile(string findPath)
    {
        var allFindPath = Path.Combine(Application.persistentDataPath, findPath);
        return new DirectoryInfo(allFindPath).GetFiles();
    }
    //通过朝向返回旋转角度
    public static float GetAngleByXZ(float horizontal, float vertical)
    {
        if (horizontal > 0 && vertical > 0)
            return 45;
        else if (horizontal > 0 && vertical == 0)
            return 90;
        else if (horizontal > 0 && vertical < 0)
            return 135;
        else if (horizontal < 0 && vertical > 0)
            return -45;
        else if (horizontal < 0 && vertical == 0)
            return -90;
        else if (horizontal < 0 && vertical < 0)
            return -135;
        else if (horizontal == 0 && vertical > 0)
            return 0;
        else if (horizontal == 0 && vertical < 0)
            return 180;
        return 0;
    }
    //获取下一个角度
    public static Dir GetNextDir(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }
    //获取方位角度
    public static int GetRotationAngle(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return 90;
            case Dir.Up: return 180;
            case Dir.Right: return 270;
        }
    }
    //原点矫正
    public static Vector2Int GetRotationOffset(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, 1);
            case Dir.Up: return new Vector2Int(1, 1);
            case Dir.Right: return new Vector2Int(1, 0);
        }
    }
    //清空子物体
    public static void ClearChilds(GameObject parent)
    {
        if (parent.transform.childCount > 0)
            for (int i = 0; i < parent.transform.childCount; i++)
                GameObject.Destroy(parent.transform.GetChild(i).gameObject);
    }
    //Production转Serialization
    public static Item.Serialization ProductionToSerialization(MakeItemSO.Production p)
    {
        return new Item.Serialization(p.num, p.count);
    }
    //创建二维柏林噪声
    public static float[,] CreateNoiseMap(int mapWidth, int mapHeight, float scale, 
        int octaves, float persistance, float lacunarity, float xOrg, float yOrg)
    {
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
            }

        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
                map[x, y] = Mathf.InverseLerp(minValue, maxValue, map[x, y]);

        return map;
    }
    public static float[,] CreateNoiseMap(int mapWidth, int mapHeight, float scale,
        int octaves, float persistance, float lacunarity, int seed)
    {
        System.Random rand = new System.Random(seed);
        float xOrg = rand.Next(0, mapWidth);
        float yOrg = rand.Next(0, mapHeight);

        return CreateNoiseMap(mapWidth, mapHeight, scale, octaves, persistance, lacunarity, xOrg, yOrg);
    }
    //二维随机取样
    public static List<int[]> GetRandomPoint2(int Width, int Height, int Count)
    {
        List<int[]> output = new List<int[]>();
        int[] pointList = new int[Width * Height];
        for (int i = 0; i < pointList.Length; i++)
            pointList[i] = i;

        int end = pointList.Length;
        int num = 0;
        for (int i = 0; i < Count; i++)
        {
            var random = new System.Random();
            num = random.Next(0, end);
            output.Add(new int[2] { pointList[num] / Width, pointList[num] % Width });
            pointList[num] = pointList[end - 1];
            end--;
        }
        return output;
    }
}
