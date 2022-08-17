using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using Place;
using MySocket;

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

        public GridObject[,][,] GridArrayList;
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

            var blockWidth = Mathf.CeilToInt(width / 100f);
            var blockHeight = Mathf.CeilToInt(height / 100f);
            GridArrayList = new GridObject[blockWidth, blockHeight][,];
        }
        public struct Serialization //序列化结构体
        {
            public float[] originPosition;
            public int Width;
            public int Height;
            public float CellSize;
            public Serialization(int Width, int Height, float CellSize, Vector3 originPosition)
            {
                this.Width = Width;
                this.Height = Height;
                this.CellSize = CellSize;
                this.originPosition = new float[3] {originPosition.x, originPosition.y, originPosition.z};
            }
        }

        //序列化
        public Serialization ToSerialization()
        {
            return new Serialization(Width, Height, CellSize, originPosition);
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
                GridArrayList[x / 100, z / 100][x % 100, z % 100] = value;
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
            if (x > 0 && z > 0 && x < Width && z < Height)
                return GridArrayList[x / 100, z / 100][x % 100, z % 100];
            return null;
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
        //判断此坐标是否可放置
        public bool isCanBuild(int x, int z)
        {
            var gridObject = GetGridObject(x, z);
            return gridObject.CanBuild;
        }
        //Grid序列化
        public GridObject.Serialization[,] GridToSerialization(int blockWidth, int blockHeight)
        {
            var gridArray = GridArrayList[blockWidth, blockHeight];
            var _out = new GridObject.Serialization[gridArray.GetLength(0), gridArray.GetLength(1)];
            for (int i = 0; i < gridArray.GetLength(0); i++)
                for (int j = 0; j < gridArray.GetLength(1); j++)
                    _out[i, j] = gridArray[i, j].ToSerialization();
            return _out;
        }
    }
}