using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bags
{
    [CreateAssetMenu(menuName = "MySO/DropTableSO")]
    public class DropTableSO : ScriptableObject
    {
        [Serializable]
        public class DropItem
        {
            public int num = 0;
            public int count = 0;
            public int maxCount = 0;
            public int minCount = 0;
            public bool isRandom = false;
        }

        public DropItem[] table;
    }
}
