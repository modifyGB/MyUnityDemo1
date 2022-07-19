using Newtonsoft.Json;
using Place;
using System;
using System.IO;
using UnityEngine;

//������
public static class Utils
{
    //�������ָ����������
    public static Vector3 MouseToTerrainPosition(string layerName)
    {
        Vector3 position = Vector3.zero;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit info, 10000, LayerMask.GetMask(layerName)))
            position = info.point;
        return position;
    }
    //�������ָ���������Ϣ
    public static RaycastHit CameraRay()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit info))
        {
            return info;
        }
        return new RaycastHit();
    }
    //ʹ�����ֲ���������
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
    //��meterialӦ�õ����弰��������
    public static void SetMaterial(GameObject go, Material material)
    {
        var meshes = go.GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in meshes)
            mesh.material = material;
    }
    //ʹ��json��������
    public static void SaveObjectAsJson(string savePath, object saveData)
    {
        string json = JsonConvert.SerializeObject(saveData);
        var allSavePath = Path.Combine(Application.persistentDataPath, savePath);
        var fileStream = new FileStream(allSavePath, FileMode.Create);

        using (var writer = new StreamWriter(fileStream))
            writer.Write(json);
        Debug.Log("Save success!");
    }
    //��ȡ����
    public static TSaveObject LoadObject<TSaveObject>(string savePath)
    {
        var allSavePath = Path.Combine(Application.persistentDataPath, savePath);
        if (File.Exists(allSavePath))
            using (var reader = new StreamReader(allSavePath))
            {
                var saveData = reader.ReadToEnd();
                if (saveData == null)
                    return default;
                return JsonConvert.DeserializeObject<TSaveObject>(saveData);
            }
        Debug.LogError("Cannot find the save file!");
        return default;
    }
    //ͨ�����򷵻���ת�Ƕ�
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
    //��ȡ��һ���Ƕ�
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
    //��ȡ��λ�Ƕ�
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
    //ԭ�����
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
    //���������
    public static void ClearChilds(GameObject parent)
    {
        if (parent.transform.childCount > 0)
            for (int i = 0; i < parent.transform.childCount; i++)
                GameObject.Destroy(parent.transform.GetChild(i).gameObject);
    }
}
