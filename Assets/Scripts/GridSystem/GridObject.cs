using Newtonsoft.Json;
using Place;
using UnityEngine;

namespace GridSystem
{
    public enum GridEnvironment { GRASS, SOIL, WATER };

    //����Ԫ�࣬�������һЩ��Ԫ��Ϣ
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
        } //�˴������뵥Ԫ��Ϣ
        public GridEnvironment gridEnvironment = GridEnvironment.GRASS;
        public struct Serialization //���л��ṹ��
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
        //���л�
        public Serialization ToSerialization()
        {
            return new Serialization(gridEnvironment);
        }
        //���ص�Ԫ��Ӧ��������
        public Vector3 GetWorldPosition()
        {
            return new Vector3(x, 0, z) * grid.CellSize + grid.originPosition;
        }
        //�����Ԫ��Ϣ
        public void ClearPlaceableObject()
        {
            IsWalkable = true;
            placeObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }
        //string���л�
        public override string ToString()
        {
            return $"{x}, {z}";
        }
    }
}
