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

        public struct Serialization
        {

        }

        //序列化
        public Serialization ToSerialization()
        {
            return default;
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            enemyMovement = GetComponent<EnemyMovement>();
            enemyAttack = GetComponent<EnemyAttack>();
            enemyValue = GetComponent<EnemyValue>();

            bloodPoint = Utils.FindChildByName(gameObject, "Blood");
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
        }
        //受到攻击处理
        public virtual void BeAttack(WeaponSO weapon)
        {
            enemyValue.Blood -= CalHeart(weapon);
            enemyState = EnemyState.Attract;
            animator.SetTrigger("isAttract");
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
            return weapon.attack + PlayerManager.I.PlayerValue.BaseAttack - enemySO.defence;
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
            Destroy(bloodObject);
            Destroy(gameObject);
        }
        //掉落
        public void Drop()
        {
            PlayerManager.I.PlayerValue.Experience += enemySO.experience;
        }
    }
}
