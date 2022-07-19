using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyValue : MyScript
    {
        Animator animator;

        private EnemySO enemySO;
        private float blood = 1;
        public float Blood
        { 
            get { return blood; } 
            set 
            { 
                if (value < 0)
                    value = 0;
                blood = value;
                DieCheck();
                BloodChange.Invoke();
            } 
        }
        public Action BloodChange;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            BloodChange += GetComponent<EnemyObject>().BloodObjectHandler;
        }

        public void Initialization(EnemySO enemySO)
        {
            this.enemySO = enemySO;
            blood = enemySO.maxBlood;
        }
        //À¿ÕˆºÏ≤È
        public void DieCheck()
        {
            if (blood <= 0)
            {
                animator.SetTrigger("isDie");
                animator.SetBool("Die", true);
            }
        }
    }
}
