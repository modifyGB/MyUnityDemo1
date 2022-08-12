using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class TurtleShell : EnemyObject
    {
        public override void CreateBloodObject()
        {
            bloodObject = Instantiate(EnemyManager.I.enemyBloodPrefab);
            nowBlood = Utils.FindChildByName(bloodObject, "nowBlood");
            bloodObject.transform.SetParent(UIManager.I.DropItemCanva.transform);
            bloodObject.transform.localScale = new Vector3(0.02f, 0.01f, 0.01f);
            bloodObject.transform.position = bloodPoint.transform.position;
        }
    }
}
