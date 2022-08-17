using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapon
{
    public class WeaponObject : MyScript
    {
        private WeaponSO weaponSO;
        public WeaponSO WeaponSO { get { return weaponSO; } }

        private List<float> angles = new List<float>();
        public List<float> Angles { get { return angles; } }

        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;

        //≥ı ºªØ
        public void Initialization(WeaponSO weaponSO)
        {
            this.weaponSO = weaponSO;
            for (int i = 0; i <= weaponSO.attackPrecision; i++)
                angles.Add(weaponSO.attackAngle / 2 - i * (weaponSO.attackAngle / weaponSO.attackPrecision / 2));
        }

        public virtual void SetTransform(Transform parent)
        {
            transform.SetParent(parent, false);
            transform.localPosition = position;
            transform.localRotation = Quaternion.Euler(rotation);
        }
    }
}
