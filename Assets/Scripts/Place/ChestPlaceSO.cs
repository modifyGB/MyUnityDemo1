using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Place
{
    [CreateAssetMenu(menuName = "MySO/ChestPlaceSO")]
    public class ChestPlaceSO : PlaceSO
    {
        public int bagCapacity = 10;

        public override PlaceObject Create(Vector2Int origin, Dir direction)
        {
            Chest newObject = (Chest)base.Create(origin, direction);
            newObject.Initialization(bagCapacity);
            return newObject;
        }
    }
}
