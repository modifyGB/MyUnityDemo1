using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttack : MyScript
    {
        Rigidbody rb;
        Animator animator;

        private EnemySO enemySO;
        private EnemyObject enemyObject;

        private Transform CenterPoint;

        private float attackTimer = 0;
        private int attackCount = 0;
        private List<float> angles = new List<float>();
        public List<float> Angles { get { return angles; } }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            enemyObject = GetComponent<EnemyObject>();
            CenterPoint = Utils.FindChildByName(gameObject, "Center").transform;
        }

        void Update()
        {
            DrawDebug();
        }

        private void FixedUpdate()
        {
            attackTimer += Time.fixedDeltaTime;
            if (enemyObject.EnemyState == EnemyState.PeaceMove)
            {
                HashSet<GameObject> gameObjects = new HashSet<GameObject>();
                RaycastHit hit;
                foreach (var angle in Angles)
                {
                    float x1 = Mathf.Sin(Mathf.Deg2Rad * angle);
                    float x2 = Mathf.Sin(Mathf.Deg2Rad * -angle);
                    float z1 = Mathf.Cos(Mathf.Deg2Rad * angle);
                    float z2 = Mathf.Cos(Mathf.Deg2Rad * -angle);
                    if (Physics.Raycast(CenterPoint.position, transform.
                        TransformDirection(new Vector3(x1, 0, z1)), out hit, enemySO.findDistance))
                        gameObjects.Add(hit.collider.gameObject);
                    if (Physics.Raycast(CenterPoint.position, transform.
                        TransformDirection(new Vector3(x2, 0, z2)), out hit, enemySO.findDistance))
                        gameObjects.Add(hit.collider.gameObject);
                }
                foreach (var gameObject in gameObjects)
                {
                    if (gameObject.tag == "Player")
                    {
                        enemyObject.EnemyState = EnemyState.Attract;
                        animator.SetTrigger("isAttract");
                    }
                }
            }
            else if (enemyObject.EnemyState == EnemyState.AttractMove)
            {
                var distance = Vector3.Distance(PlayerManager.Player.transform.position, transform.position);
                if (distance <= enemySO.attackDistance)
                {
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                    animator.SetFloat("moveSpeed", 0);
                    animator.SetFloat("moveSpeedRaw", 0);
                    if (attackTimer >= enemySO.attackInterval)
                    {
                        if (attackCount >= 2)
                        {
                            animator.SetTrigger("isAttack2");
                            attackCount = -1;
                        }
                        else
                            animator.SetTrigger("isAttack1");
                        animator.SetFloat("attackRate", enemySO.attackRate / 10);
                        enemyObject.EnemyState = EnemyState.Attack;
                        attackTimer = 0;
                        attackCount++;
                    }
                }
                else if (distance >= enemySO.loseDistance)
                {
                    enemyObject.EnemyState = EnemyState.PeaceMove;
                    GetComponent<EnemyMovement>().NowPatrolNum = 0;
                }
            }
        }

        public void Initialization(EnemySO enemySO)
        {
            this.enemySO = enemySO;
            for (int i = 0; i <= enemySO.findPrecision; i++)
                angles.Add(enemySO.findAngle / 2 - i * (enemySO.findAngle / enemySO.findPrecision / 2));
        }

        public void Attack1()
        {
            if (PlayerManager.I.PlayerState == PlayerState.Die)
                return;
            PlayerManager.I.PlayerValue.NowBlood -= Mathf.Clamp(
                enemySO.attack - PlayerManager.I.PlayerValue.BaseDefence, 1, enemySO.attack);
        }

        public void Attack2()
        {
            if (PlayerManager.I.PlayerState == PlayerState.Die)
                return;
            PlayerManager.I.PlayerValue.NowBlood -= Mathf.Clamp(
                enemySO.attack - PlayerManager.I.PlayerValue.BaseDefence, 1, enemySO.attack);
            PlayerManager.I.PlayerMovement.GetHit();
        }

        public void EndAttract()
        {
            enemyObject.EnemyState = EnemyState.AttractMove;
        }

        public void EndAttack()
        {
            enemyObject.EnemyState = EnemyState.AttractMove;
        }

        public void DrawDebug()
        {
            if (enemySO.isBug)
            {
                foreach (var angle in Angles)
                {
                    float x1 = enemySO.findDistance * Mathf.Sin(Mathf.Deg2Rad * angle);
                    float x2 = enemySO.findDistance * Mathf.Sin(Mathf.Deg2Rad * -angle);
                    float z1 = enemySO.findDistance * Mathf.Cos(Mathf.Deg2Rad * angle);
                    float z2 = enemySO.findDistance * Mathf.Cos(Mathf.Deg2Rad * -angle);
                    Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(x1, 0, z1)), Color.red);
                    Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(x2, 0, z2)), Color.red);
                }
            }
        }
    }
}
