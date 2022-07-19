using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerValue : MyScript
    {
        Animator animator;

        private float baseBlood;
        public float BaseBlood { get { return baseBlood; } set { baseBlood = value; } }
        private float baseAttack;
        public float BaseAttack { get { return baseAttack; } set { baseAttack = value; } }
        private float baseDefence;
        public float BaseDefence { get { return baseDefence; } set { baseDefence = value; } }
        private int level = 1;
        public int Level { get { return level; } }
        private float nowExperience;
        public float NowExperience { get { return nowExperience; }}
        private float upExperience;
        public float UpExperience { get { return upExperience; } }
        private float experience = 0;
        public float Experience 
        { 
            get { return experience; } 
            set 
            {
                experience = value;
                UpdateLevel();
                ExperienceChange.Invoke();
            } 
        }
        private float nowBlood = 100;
        public float NowBlood 
        { 
            get { return nowBlood; } 
            set 
            { 
                if (value < 0)
                    value = 0;
                nowBlood = value;
                DieCheck();
                BloodChange.Invoke();
            } 
        }

        public Action ExperienceChange;
        public Action BloodChange;

        private void Awake()
        {
            animator = GetComponent<Animator>();

            UpdateLevel();
            UpdateValue();
        }

        private void Start()
        {
            ExperienceChange.Invoke();
            BloodChange.Invoke();
        }
        //更新level
        public void UpdateLevel()
        {
            var l = Mathf.FloorToInt(experience / 100) + 1;
            nowExperience = experience % 100;
            upExperience = 100;
            if (l != level)
            {
                level = l;
                UpdateValue();
                nowBlood = baseBlood;
            }
        }
        //更新角色数值
        public void UpdateValue()
        {
            baseBlood = 100 + (level - 1) * 10;
            baseAttack = 5 + (level - 1) * 2;
            baseDefence = 0 + (level - 1) * 1;
        }
        //死亡检查
        public void DieCheck()
        {
            if (nowBlood <= 0)
            {
                animator.SetTrigger("isDie");
                //animator.SetBool("Die", true);
            }
        }
        //死亡
        public void Die()
        {

        }
    }
}
