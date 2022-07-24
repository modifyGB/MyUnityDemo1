using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using Manager;

namespace Player
{
    public class PlayerPerspective : MyScript
    {

        bool isMouse1 = false;


        public void Awake()
        {
            CinemachineCore.GetInputAxis += CameraGetAxis;
        }

        void Update()
        {
            GetInput();
        }

        void GetInput()
        {
            isMouse1 = Input.GetMouseButton(1);
        }

        public float CameraGetAxis(string axisName)
        {
            if (axisName == "Mouse X")
            {
                if (isMouse1 && UIManager.I.UIState == UIState.Play)
                    return -Input.GetAxis("Mouse X");
                return 0;
            }
            return Input.GetAxis(axisName);
        }
    }
}
