using Enemy;
using GridSystem;
using Manager;
using Place;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PlayerAttack : MyScript
    {
        Rigidbody rb;
        Animator animator;

        bool isMouse0 = false;
        float attackTimer = 0;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {

        }

        void Update()
        {
            if (!isMouse0)
                isMouse0 = Input.GetMouseButton(0);

            DrawDebug();
        }

        private void FixedUpdate()
        {
            attackTimer += Time.fixedDeltaTime;
            if (isMouse0)
            {
                if (PlayerManager.I.PlayerState != PlayerState.Attack && PlayerManager.I.isWeapon && 
                    UIManager.I.UIState == UIState.Play && attackTimer >= PlayerManager.I.Weapon.
                    WeaponSO.attackInterval && !EventSystem.current.IsPointerOverGameObject())
                {
                    animator.SetFloat("attackRate", PlayerManager.I.Weapon.WeaponSO.attackRate);
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                    animator.SetInteger("attackAnim", PlayerManager.I.Weapon.WeaponSO.attackAnim);
                    animator.SetTrigger("isAttack");
                    PlayerManager.I.PlayerState = PlayerState.Attack;

                    attackTimer = 0;
                }
                isMouse0 = false;
            }
        }

        void EndAttack()
        {
            PlayerManager.I.PlayerState = PlayerState.Move;
            PlayerManager.I.PlayerBag.Bag.DeleteCheck(PlayerManager.I.PlayerBag.UseNum);
        }

        void Attack()
        {
            HashSet<GameObject> gameObjects = new HashSet<GameObject>();
            RaycastHit hit;
            SoundManager.I.Attack(PlayerManager.I.Weapon.WeaponSO.attackAnim);
            foreach (var angle in PlayerManager.I.Weapon.Angles)
            {
                float x1 = Mathf.Sin(Mathf.Deg2Rad * angle);
                float x2 = Mathf.Sin(Mathf.Deg2Rad * -angle);
                float z1 = Mathf.Cos(Mathf.Deg2Rad * angle);
                float z2 = Mathf.Cos(Mathf.Deg2Rad * -angle);
                if (Physics.Raycast(PlayerManager.I.CenterPoint.position, PlayerManager.Player.transform.
                    TransformDirection(new Vector3(x1, 0, z1)), out hit, PlayerManager.I.Weapon.WeaponSO.attackDistance))
                    gameObjects.Add(hit.collider.gameObject);
                if (Physics.Raycast(PlayerManager.I.CenterPoint.position, PlayerManager.Player.transform.
                    TransformDirection(new Vector3(x2, 0, z2)), out hit, PlayerManager.I.Weapon.WeaponSO.attackDistance))
                    gameObjects.Add(hit.collider.gameObject);
            }
            foreach (var gameObject in gameObjects)
            {
                var po = gameObject.GetComponent<PlaceObject>();
                var eo = gameObject.GetComponent<EnemyObject>();
                if (po != null)
                    PlayerManager.I.Weapon.WeaponSO.AttackToPlace(po);
                else if (eo != null)
                    PlayerManager.I.Weapon.WeaponSO.AttackToEnemy(eo);
            }
        }

        void DrawDebug()
        {
            if (PlayerManager.I.isBug && PlayerManager.I.Weapon != null)
            {
                foreach (var angle in PlayerManager.I.Weapon.Angles)
                {
                    float x1 = PlayerManager.I.Weapon.WeaponSO.attackDistance * Mathf.Sin(Mathf.Deg2Rad * angle);
                    float x2 = PlayerManager.I.Weapon.WeaponSO.attackDistance * Mathf.Sin(Mathf.Deg2Rad * -angle);
                    float z1 = PlayerManager.I.Weapon.WeaponSO.attackDistance * Mathf.Cos(Mathf.Deg2Rad * angle);
                    float z2 = PlayerManager.I.Weapon.WeaponSO.attackDistance * Mathf.Cos(Mathf.Deg2Rad * -angle);
                    Debug.DrawRay(PlayerManager.I.CenterPoint.position, PlayerManager.Player.transform.
                    TransformDirection(new Vector3(x1, 0, z1)), Color.red);
                    Debug.DrawRay(PlayerManager.I.CenterPoint.position, PlayerManager.Player.transform.
                    TransformDirection(new Vector3(x2, 0, z2)), Color.red);
                }
            }
        }
    }
}
