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
        public struct Serialization //���л��ṹ��
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
        //��ʼ��
        public void Initialization(int num, Vector2Int origin, Dir dir, float blood)
        {
            this.num = num;
            this.origin = origin;
            this.dir = dir;
            this.blood = blood;
            placeSO = GameManager.I.placeTableSO.table[num];
        }
        //���л�
        public Serialization ToSerialization()
        {
            return new Serialization(num, origin, dir);
        }
        //��ȡ�����Ӧ��Ԫλ���б�
        public List<Vector2Int> GetGridPositionList()
        {
            return GameManager.I.placeTableSO.table[num].GetGridPositionList(origin, dir);
        }
        //��ת����
        public void NextDir()
        {
            dir = Utils.GetNextDir(dir);
        }
        //�ܵ���������
        public virtual void BeAttack(WeaponSO weapon)
        {
            Blood -= weapon.attack;
        }
        //����Ʒ
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
        //����
        public void DestroyOut()
        {
            foreach (var gridPos in GetGridPositionList())
                MapManager.I.grid.GetGridObject(gridPos.x, gridPos.y).ClearPlaceableObject();
            Drop();
            DestroySelf();
        }
    }
}
