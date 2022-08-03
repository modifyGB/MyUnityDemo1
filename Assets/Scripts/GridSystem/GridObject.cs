using Newtonsoft.Json;
using Place;
using UnityEngine;

namespace GridSystem
{
    public enum GridEnvironment { GRASS, SOIL, WATER };

    //网格单元类，用来存放一些单元信息
    public class GridObject
    {
        public int x;
        public int z;
        private GridXZ grid;
        private PlaceObject placeObject;
        public Ground groundObject = null;

        public bool IsWalkable = true;
        public bool CanBuild { get { return placeObject == null; } }
        public PlaceObject PlaceObject
        {
            get => placeObject;
            set
            {
                IsWalkable = false;
                placeObject = value;
                grid.TriggerGridObjectChanged(x, z);
            }
        } //此处有输入单元信息
        public GridEnvironment gridEnvironment = GridEnvironment.GRASS;
        public struct Serialization //序列化结构体
        {
            public GridEnvironment ge;
            public Serialization(GridEnvironment gridEnvironment) { this.ge = gridEnvironment; }
        }

        public GridObject(GridXZ grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }
        public GridObject(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
        //序列化
        public Serialization ToSerialization()
        {
            return new Serialization(gridEnvironment);
        }
        //返回单元对应世界坐标
        public Vector3 GetWorldPosition()
        {
            return new Vector3(x, 0, z) * grid.CellSize + grid.originPosition;
        }
        //清除单元信息
        public void ClearPlaceableObject()
        {
            IsWalkable = true;
            placeObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }
        //string序列化
        public override string ToString()
        {
            return $"{x}, {z}";
        }
    }
}
