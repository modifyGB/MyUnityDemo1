using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager 
{
    public class CameraManager : Singleton<CameraManager>
    {
        CinemachineVirtualCamera cvc;

        public override void Awake()
        {
            base.Awake();

            cvc = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            cvc.Follow = PlayerManager.Player.transform;
            cvc.LookAt = PlayerManager.Player.transform;
        }
    } 
}
