using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapon
{
    public class Axe : WeaponObject
    {
        public override void SetTransform(Transform parent)
        {
            base.SetTransform(parent);
            transform.localEulerAngles = new Vector3(-100, 180, 20);
        }
    }
}
