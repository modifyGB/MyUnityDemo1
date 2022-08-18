using Items;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace Enemy
{
    public enum EnemyType { Slime }
    public enum EnemyState { PeaceMove, AttractMove, Attract, Attack, Die, GetHit }

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class EnemyObject : MyScript
    {
        Animator animator;

        protected EnemySO enemySO;
        public EnemySO EnemySO { get { return enemySO; } }
        protected EnemyState enemyState = EnemyState.PeaceMove;
        public EnemyState EnemyState { get { return enemyState; } set { enemyState = value; } }

        protected EnemyMovement enemyMovement;
        protected EnemyAttack enemyAttack;
        protected EnemyValue enemyValue;

        protected GameObject bloodObject;
        protected GameObject nowBlood;
        protected GameObject bloodPoint;
        protected Transform dropPoint;

        public struct Serialization
        {
            public int num;
            public float[] position;
            public float angle;
            public Serialization(int num, float[] position, float angle) 
            { this.num = num; this.position = position; this.angle = angle; }
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            enemyMovement = GetComponent<EnemyMovement>();
            enemyAttack = GetComponent<EnemyAttack>();
            enemyValue = GetComponent<EnemyValue>();

            bloodPoint = Utils.FindChildByName(gameObject, "Blood");
            dropPoint = Utils.FindChildByName(gameObject, "Drop").transform;
            CreateBloodObject();
        }

        private void Update()
        {
            bloodObject.transform.position = bloodPoint.transform.position;
            bloodObject.transform.rotation = Camera.main.transform.rotation;
        }

        public void Initialization(EnemySO enemySO, Vector3 rotation)
        {
            this.enemySO = enemySO;
            enemyMovement.Initialization(enemySO, rotation);
            enemyAttack.Initialization(enemySO);
            enemyValue.Initialization(enemySO);
            transform.SetParent(EnemyManager.I.Enemy);
        }
        //序列化
        public Serialization ToSerialization()
        {
            var position = enemyMovement.PatrolPoints[enemyMovement.PatrolPoints.Count - 1];
            var vec = new float[3] { position[0], position[1], position[2] };
            return new Serialization(enemySO.num, vec, enemyMovement.Angle);
        }
        //受到攻击处理
        public virtual void BeAttack(WeaponSO weapon)
        {
            if (EnemyState == EnemyState.Die)
                return;

            if (weapon.weaponPrefab.GetType() == typeof(Sword))
                enemyValue.Blood -= CalHeart(weapon);
            else
                enemyValue.Blood -= 1;

            if (enemyState == EnemyState.PeaceMove)
            {
                animator.SetTrigger("isAttract");
                enemyState = EnemyState.Attract;
            }
            else
            {
                if (Random.Range(0, 1f) < 0.3)
                {
                    enemyState = EnemyState.GetHit;
                    animator.SetTrigger("isGetHit");
                }
            }
        }
        //初始化bloodObject
        public virtual void CreateBloodObject()
        {
            bloodObject = Instantiate(EnemyManager.I.enemyBloodPrefab);
            nowBlood = Utils.FindChildByName(bloodObject, "nowBlood");
            bloodObject.transform.SetParent(UIManager.I.DropItemCanva.transform);
            bloodObject.transform.localScale = Vector3.one * 0.01f;
            bloodObject.transform.position = bloodPoint.transform.position;
        }
        //计算伤害
        public virtual float CalHeart(WeaponSO weapon)
        {
            return Mathf.Clamp(weapon.attack + PlayerManager.I.PlayerValue.BaseAttack - enemySO.defence, 1, float.MaxValue);
        }
        //BloodObject触发器
        public void BloodObjectHandler()
        {
            nowBlood.transform.localScale = new Vector3(enemyValue.Blood * 1.0f / enemySO.maxBlood, 1, 1);
            nowBlood.transform.localPosition = new Vector3((enemySO.maxBlood - enemyValue.Blood) / enemySO.maxBlood * 90f * -0.5f, 0, 0);
        }
        //死亡
        public void EndDie()
        {
            var pos = enemyMovement.PatrolPoints[enemyMovement.PatrolPoints.Count - 1];
            var xz = MapManager.I.grid.GetXZ(pos);
            EnemyManager.I.EnemyList[xz.x].Remove(xz.y);
            DestroySelf();
        }
        //掉落
        public void Drop()
        {
            PlayerManager.I.PlayerValue.Experience += enemySO.experience;

            foreach (var drop in enemySO.dropTableSO.table)
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

        public override void DestroySelf()
        {
            Destroy(bloodObject);
            base.DestroySelf();
        }
    }
}
