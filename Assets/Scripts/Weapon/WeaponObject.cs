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
            transform.localPosition = Vector3.zero;
        }
    }
}
