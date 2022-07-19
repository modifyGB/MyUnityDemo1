using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "MySO/EnemyTableSO")]
    public class EnemyTableSO : MyScriptable
    {
        public EnemySO[] table;
    }
}
