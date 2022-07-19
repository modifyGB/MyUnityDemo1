using GridSystem;
using Items;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Place
{
    public class PlaceObject : MyScript
    {
        private PlaceSO placeSO;
        private int num = 0;
        public int Num { get { return num; } }
        private Vector2Int origin;
        public Vector2Int Origin { get { return origin; } }
        private Dir dir = Dir.Down;
        public Dir Dir { get { return dir; } }
        private float blood = 1;
        public float Blood
        {
            get { return blood; }
            set
            {
                blood = value;
                if (blood <= 0)
                    DestroyOut();
            }
        }
        public struct Serialization //序列化结构体
        {
            public int num;
            public int[] origin;
            public Dir dir;
            public Serialization(int num, Vector2Int origin, Dir dir)
            { this.num = num; this.origin = new int[2] { origin.x, origin.y }; this.dir = dir; }
        }
        private Transform dropPoint;

        private void Awake()
        {
            //blood = maxBlood;
            dropPoint = Utils.FindChildByName(gameObject, "Drop").transform;
        }
        //初始化
        public void Initialization(int num, Vector2Int origin, Dir dir, float blood)
        {
            this.num = num;
            this.origin = origin;
            this.dir = dir;
            this.blood = blood;
            placeSO = GameManager.I.placeTableSO.table[num];
        }
        //序列化
        public Serialization ToSerialization()
        {
            return new Serialization(num, origin, dir);
        }
        //获取物体对应单元位置列表
        public List<Vector2Int> GetGridPositionList()
        {
            return GameManager.I.placeTableSO.table[num].GetGridPositionList(origin, dir);
        }
        //旋转物体
        public void NextDir()
        {
            dir = Utils.GetNextDir(dir);
        }
        //受到攻击处理
        public virtual void BeAttack(WeaponSO weapon)
        {
            Blood -= weapon.attack;
        }
        //爆物品
        public void Drop()
        {
            foreach (var drop in placeSO.dropTableSO.table)
            {
                var item = GameManager.I.itemTableSO.table[drop.num];
                if (item.isCountable)
                {
                    var dropItem = new Item(drop.num, drop.count);
                    dropItem.Throw(dropPoint, new Vector3(0, 3, 0));
                }
                else
                {

                }

            }
        }
        //销毁
        public void DestroyOut()
        {
            foreach (var gridPos in GetGridPositionList())
                MapManager.I.grid.GetGridObject(gridPos.x, gridPos.y).ClearPlaceableObject();
            Drop();
            DestroySelf();
        }
    }
}
