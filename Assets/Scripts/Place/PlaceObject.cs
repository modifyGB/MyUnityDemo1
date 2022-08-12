using GridSystem;
using Items;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;
using DG.Tweening;

namespace Place
{
    public class PlaceObject : MyScript
    {
        private PlaceSO placeSO;
        private int num = 0;
        public int Num { get { return num; } }
        protected Vector2Int origin;
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
        protected Transform dropPoint;

        public virtual void Awake()
        {
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
        //�Ҽ��¼�
        public virtual void Mouse1Event()
        {

        }
        //�ܵ���������
        public virtual void BeAttackBefore()
        {
            transform.DOShakePosition(0.1f, new Vector3(0.1f, 0, 0.1f), 100, 0, false, false);
        }
        public virtual void BeAttackNow(WeaponSO weapon)
        {
            Blood -= 1;
        }
        public virtual void BeAttackAfter() { }
        public void BeAttack(WeaponSO weapon)
        {
            BeAttackBefore();
            BeAttackNow(weapon);
            BeAttackAfter();
        }
        //����Ʒ
        public virtual void Drop()
        {
            if (placeSO.dropTableSO == null)
                return;
            foreach (var drop in placeSO.dropTableSO.table)
            {
                var item = GameManager.I.itemTableSO.table[drop.num];
                if (Random.Range(0, 1f) > drop.itemRandom)
                    continue;
                if (!item.isCountable)
                {
                    for (int i = 0; i < drop.count; i++)
                    {
                        var dropItem = new Item(drop.num);
                        dropItem.Throw(dropPoint, new Vector3(0, 3, 0));
                    }
                }
                else
                {
                    if (drop.countRandom)
                    {
                        var dropItem = new Item(drop.num, Random.Range(drop.minCount, drop.maxCount + 1));
                        dropItem.Throw(dropPoint, new Vector3(0, 3, 0));
                    }
                    else
                    {
                        var dropItem = new Item(drop.num, drop.count);
                        dropItem.Throw(dropPoint, new Vector3(0, 3, 0));
                    }
                }
            }
        }
        //����
        public virtual void DestroyOut()
        {
            MapManager.I.placeList[origin.x].Remove(origin.y);
            if (MapManager.I.chestList.ContainsKey(origin.x) 
                && MapManager.I.chestList[origin.x].ContainsKey(origin.y))
                MapManager.I.chestList[origin.x].Remove(origin.y);

            Drop();
            DestroySelf();
        }

        public override void DestroySelf()
        {
            foreach (var gridPos in GetGridPositionList())
                MapManager.I.grid.GetGridObject(gridPos.x, gridPos.y).ClearPlaceableObject();

            base.DestroySelf();
        }
    }
}
