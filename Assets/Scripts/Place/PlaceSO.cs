using Bags;
using GridSystem;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Place
{
    public enum Dir { Down, Left, Up, Right }

    [CreateAssetMenu(menuName = "MySO/PlaceSO")]
    public class PlaceSO : MyScriptable
    {
        [Header("id")]
        public int num = 0;
        public PlaceObject placeObject;
        [Header("��λ")]
        public float Left = 0;
        public float Right = 5;
        public float Up = 5;
        public float Down = 0;
        [Header("����")]
        public float maxBlood = 10;
        public DropTableSO dropTableSO;

        //��ȡ�����Ӧ��Ԫλ���б�
        public List<Vector2Int> GetGridPositionList(Vector2Int origin, Dir dir)
        {
            float cellSize = MapManager.I.grid.CellSize;
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            float right = Right;
            float left = -Left;
            float up = Up;
            float down = -Down;
            switch (dir)
            {
                default:
                case Dir.Down:
                    break;
                case Dir.Up:
                    left = -Right;
                    right = -Left;
                    up = -Down;
                    down = -Up;
                    break;
                case Dir.Left:
                    left = Down;
                    right = Up;
                    up = -Left;
                    down = -Right;
                    break;
                case Dir.Right:
                    left = -Up;
                    right = -Down;
                    up = Right;
                    down = Left;
                    break;
            }
            for (int x = Mathf.FloorToInt(left / cellSize); x < Mathf.CeilToInt(right / cellSize); x++)
            {
                for (int y = Mathf.FloorToInt(down / cellSize); y < Mathf.CeilToInt(up / cellSize); y++)
                {
                    gridPositionList.Add(origin + new Vector2Int(x, y));
                }
            }
            return gridPositionList;
        }
        //��������
        public PlaceObject Create(Vector2Int origin, Dir direction)
        {
            GridXZ grid = MapManager.I.grid;
            var gridPosList = GetGridPositionList(origin, direction);
            var rotOffset = Utils.GetRotationOffset(direction);
            var worldPosition = grid.GetWorldPosition(origin.x, origin.y) + new Vector3(rotOffset.x, 0, rotOffset.y) * grid.CellSize;
            //��������
            var newObject = Instantiate(placeObject, worldPosition, Quaternion.Euler(0, Utils.GetRotationAngle(direction), 0));
            newObject.Initialization(num, origin, direction, maxBlood);
            foreach (var gridPos in gridPosList)
            {
                var gridTobuild = grid.GetGridObject(gridPos.x, gridPos.y);
                gridTobuild.PlaceableObject = newObject;
            }
            //�������
            Utils.FindChildByName(newObject.gameObject, "Area").SetActive(false);
            // Build effects
            // placedObjectTransform.DOShakeScale(.5f, .2f, 10, 90, true);

            return newObject;
        }
        //�ж��Ƿ��ܽ���
        public bool BuildCheck(Vector2Int origin, Dir direction)
        {
            GridXZ grid = MapManager.I.grid;
            var gridPosList = GetGridPositionList(origin, direction);
            var canBuild = true;
            foreach (var gridPos in gridPosList)
            {
                if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= grid.Width || gridPos.y >= grid.Height)
                {
                    canBuild = false;
                    break;
                }
                if (!grid.GetGridObject(gridPos.x, gridPos.y).CanBuild)
                {
                    canBuild = false;
                    break;
                }
            }
            return canBuild;
        }
    }
}
