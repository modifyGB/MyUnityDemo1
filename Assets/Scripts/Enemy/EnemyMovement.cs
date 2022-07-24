using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyMovement : MyScript
    {
        Rigidbody rb;
        Animator animator;

        private EnemySO enemySO;
        private EnemyObject enemyObject;
        private List<Vector3> patrolPoints = new List<Vector3>();
        public List<Vector3> PatrolPoints { get { return patrolPoints; } }

        private int nowPatrolNum = 0;
        public int NowPatrolNum
        {
            get { return nowPatrolNum; }
            set { nowPatrolNum = value; }
        }
        private float nowMoveSpeed = 0;
        private float stayTimer = 0;
        private bool isStay = false;
        private float angle;
        public float Angle { get { return angle; } }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            enemyObject = GetComponent<EnemyObject>();
        }

        private void FixedUpdate()
        {
            Move();
            Animate();

            stayTimer += Time.fixedDeltaTime;
        }

        public void Initialization(EnemySO enemySO, Vector3 rotation)
        {
            this.enemySO = enemySO;
            angle = rotation.y;
            foreach (var i in enemySO.PatrolPoints)
            {
                var vec = Quaternion.AngleAxis(rotation.y, Vector3.up) * i + transform.position;
                PatrolPoints.Add(vec);       
            }
            PatrolPoints.Add(new Vector3(transform.position.x, 0, transform.position.z));
        }

        private void Move()
        {
            if (enemyObject.EnemyState == EnemyState.PeaceMove)
            {
                var nowPos = new Vector3(transform.position.x, 0, transform.position.z);
                if (Vector3.Distance(PatrolPoints[nowPatrolNum], nowPos) > 0.1)
                {
                    isStay = false;
                    nowMoveSpeed = enemySO.walkSpeed;
                    var angle = Vector3.Angle(Vector3.forward, PatrolPoints[nowPatrolNum] - nowPos);
                    var cross = Vector3.Cross(Vector3.forward, PatrolPoints[nowPatrolNum] - nowPos);
                    var turn = cross.y >= 0 ? 1f : -1f;
                    float x = turn * nowMoveSpeed * Mathf.Sin(angle * Mathf.Deg2Rad);
                    float z = nowMoveSpeed * Mathf.Cos(angle * Mathf.Deg2Rad);
                    rb.velocity = new Vector3(x, rb.velocity.y, z);
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.Euler(transform.rotation.eulerAngles.x, turn * angle,
                        transform.rotation.eulerAngles.z), 0.2f);
                }
                else
                {
                    if (!isStay)
                    {
                        stayTimer = 0;
                        rb.velocity = new Vector3(0, rb.velocity.y, 0);
                        nowMoveSpeed = 0;
                        isStay = true;
                    }
                    if (stayTimer > 5)
                    {
                        nowPatrolNum += 1;
                        if (nowPatrolNum >= PatrolPoints.Count)
                            nowPatrolNum = 0;
                        isStay = false;
                    }
                }
            }
            else if (enemyObject.EnemyState == EnemyState.AttractMove)
            {
                nowMoveSpeed = enemySO.walkSpeed + enemySO.runSpeedPlus;
                var nowPos = new Vector3(transform.position.x, 0, transform.position.z);
                var angle = Vector3.Angle(Vector3.forward, PlayerManager.Player.transform.position - nowPos);
                var cross = Vector3.Cross(Vector3.forward, PlayerManager.Player.transform.position - nowPos);
                var turn = cross.y >= 0 ? 1f : -1f;
                float x = turn * nowMoveSpeed * Mathf.Sin(angle * Mathf.Deg2Rad);
                float z = nowMoveSpeed * Mathf.Cos(angle * Mathf.Deg2Rad);
                rb.velocity = new Vector3(x, rb.velocity.y, z);
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.Euler(transform.rotation.eulerAngles.x, turn * angle,
                    transform.rotation.eulerAngles.z), 0.2f);
            }
        }

        private void Animate()
        {
            var playerManager = PlayerManager.I;
            if (nowMoveSpeed == 0)
            {
                animator.SetFloat("moveSpeed", 1);
                animator.SetFloat("moveSpeedRaw", 0);
            }
            else if (nowMoveSpeed >= enemySO.walkSpeed + enemySO.runSpeedPlus)
            {
                animator.SetFloat("moveSpeed", (enemySO.walkSpeed + enemySO.runSpeedPlus) / 5);
                animator.SetFloat("moveSpeedRaw", 1);
            }
            else
            {
                animator.SetFloat("moveSpeed", enemySO.walkSpeed / 5);
                animator.SetFloat("moveSpeedRaw", 0.5f);
            }
        }
    }
}
