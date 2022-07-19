using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using Place;

namespace GridSystem
{
    //网格类，用于存放网格单元信息等
    public class GridXZ
    {
        public event EventHandler<GridObjectChangedEventArgs> GridObjectChanged;
        public class GridObjectChangedEventArgs : EventArgs
        {
            public int x;
            public int z;
        }

        public GridObject[,] GridArray;
        public Vector3 originPosition;
        public int Width;
        public int Height;
        public float CellSize;

        public GridXZ() { }
        public GridXZ(int width, int height, float cellSize, Vector3 originPosition)
        {
            this.Width = width;
            this.Height = height;
            this.CellSize = cellSize;
            this.originPosition = originPosition;

            GridArray = new GridObject[width, height];

            for (int x = 0; x < GridArray.GetLength(0); x++)
            {
                for (int z = 0; z < GridArray.GetLength(1); z++)
                {
                    GridArray[x, z] = new GridObject(this, x, z);
                }
            }
        }
        public struct Serialization //序列化结构体
        {
            public GridObject.Serialization[,] GridArray;
            public List<PlaceObject.Serialization> PlaceableArray;
            public float[] originPosition;
            public int Width;
            public int Height;
            public float CellSize;
        }

        //序列化
        public Serialization ToSerialization()
        {
            Serialization serialization = new Serialization();
            serialization.Width = Width;
            serialization.Height = Height;
            serialization.CellSize = CellSize;
            serialization.originPosition = new float[3] { originPosition.x, originPosition.y, originPosition.z };
            serialization.PlaceableArray = new List<PlaceObject.Serialization>();
            serialization.GridArray = new GridObject.Serialization[Width, Height];
            for (int x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    serialization.GridArray[x, y] = GridArray[x, y].ToSerialization();
                    var placeable = GridArray[x, y].PlaceableObject;
                    if (placeable != null && placeable.Origin.x == x && placeable.Origin.y == y)
                        serialization.PlaceableArray.Add(placeable.ToSerialization());
                }
            return serialization;
        }
        //画辅助线
        public void InitializeDebug()
        {
            for (int x = 0; x < GridArray.GetLength(0); x++)
            {
                for (int z = 0; z < GridArray.GetLength(1); z++)
                {
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, Height), GetWorldPosition(Width, Height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(Width, 0), GetWorldPosition(Width, Height), Color.white, 100f);
        }
        //返回单元对应世界坐标
        public Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x, 0, z) * CellSize + originPosition;
        }
        //获得世界坐标对应单元
        public Vector2Int GetXZ(Vector3 worldPosition)
        {
            var x = Mathf.FloorToInt((worldPosition - originPosition).x / CellSize);
            var z = Mathf.FloorToInt((worldPosition - originPosition).z / CellSize);
            return ValidateGridPosition(new Vector2Int(x, z));
        }
        //使用网格坐标设置单元
        public void SetGridObject(int x, int z, GridObject value)
        {
            if (x >= 0 && z >= 0 && x < Width && z < Height)
            {
                GridArray[x, z] = value;
                TriggerGridObjectChanged(x, z);
            }
        }
        //gridObject状态触发器
        public void TriggerGridObjectChanged(int x, int z)
        {
            GridObjectChanged?.Invoke(this, new GridObjectChangedEventArgs { x = x, z = z });
        }
        //使用世界坐标设置单元
        public void SetGridObject(Vector3 worldPosition, GridObject value)
        {
            var xz = GetXZ(worldPosition);
            SetGridObject(xz.x, xz.y, value);
        }
        //使用网格坐标获得单元
        public GridObject GetGridObject(int x, int z)
        {
            if (x >= 0 && z >= 0 && x < Width && z < Height)
            {
                return GridArray[x, z];
            }
            else
            {
                return default(GridObject);
            }
        }
        //使用世界坐标获得单元
        public GridObject GetGridObject(Vector3 worldPosition)
        {
            var xz = GetXZ(worldPosition);
            return GetGridObject(xz.x, xz.y);
        }
        //限制网格坐标的范围
        public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
        {
            return new Vector2Int(
                Mathf.Clamp(gridPosition.x, 0, Width - 1),
                Mathf.Clamp(gridPosition.y, 0, Height - 1)
            );
        }
    }
}