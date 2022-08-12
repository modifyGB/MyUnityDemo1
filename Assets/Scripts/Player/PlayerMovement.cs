using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PlayerMovement : MyScript
    {
        Rigidbody rb;
        Animator animator;

        float horizontal;
        float vertical;
        bool isShiftButton;
        bool isJumpButton;
        bool canJump = true;
        Vector3 cameraAngles;
        string currentAnimState = "";
        float nowMoveSpeed;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            isShiftButton = Input.GetKey(KeyCode.LeftShift);
            isJumpButton = Input.GetKey(KeyCode.Space);
            cameraAngles = Camera.main.transform.rotation.eulerAngles;
        }

        void FixedUpdate()
        {
            Move();
            Jump();
            Animate();
        }

        void Move()
        {
            var playerManager = PlayerManager.I;
            if (playerManager.PlayerState != PlayerState.Move ||
                UIManager.I.UIState != UIState.Play || 
                (horizontal == 0 && vertical == 0))
            {
                nowMoveSpeed = 0;
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                return;
            }
            else if (playerManager.PlayerState == PlayerState.GetHit)
                return;

            if (isShiftButton)
                nowMoveSpeed = playerManager.walkSpeed + playerManager.runSpeedPlus;
            else
                nowMoveSpeed = playerManager.walkSpeed;

            float angles = cameraAngles.y + Utils.GetAngleByXZ(horizontal, vertical);
            float x = nowMoveSpeed * Mathf.Sin(angles * Mathf.Deg2Rad);
            float z = nowMoveSpeed * Mathf.Cos(angles * Mathf.Deg2Rad);
            rb.velocity = new Vector3(x, rb.velocity.y, z);

            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.Euler(transform.rotation.eulerAngles.x, angles,
                transform.rotation.eulerAngles.z), 0.5f);
        }

        void Jump()
        {
            if (isJumpButton)
            {
                if (canJump)
                {
                    rb.velocity += new Vector3(0, PlayerManager.I.jumpSpeed, 0);
                    canJump = false;
                }
                isJumpButton = false;
            }

            if (transform.position.y < 0.2 && rb.velocity.y < 0)
                canJump = true;
        }

        void Animate()
        {
            var playerManager = PlayerManager.I;
            if (nowMoveSpeed == 0)
            {
                animator.SetFloat("moveSpeed", 1);
                animator.SetFloat("moveSpeedRaw", 0);
            }
            else if (nowMoveSpeed >= playerManager.walkSpeed + playerManager.runSpeedPlus)
            {
                animator.SetFloat("moveSpeed", (playerManager.walkSpeed + playerManager.runSpeedPlus) / 5);
                animator.SetFloat("moveSpeedRaw", 1);
            }
            else
            {
                animator.SetFloat("moveSpeed", playerManager.walkSpeed / 5);
                animator.SetFloat("moveSpeedRaw", 0.5f);
            }
        }

        public void GetHit()
        {
            PlayerManager.I.PlayerState = PlayerState.GetHit;
            animator.SetTrigger("isGetHit");
        }

        public void EndGetHit()
        {
            PlayerManager.I.PlayerState = PlayerState.Move;
        }

        public void ChangAnimationState(string animName)
        {
            if (currentAnimState == animName)
                return;
            animator.Play(animName);
            currentAnimState = animName;
        }
    }
}
