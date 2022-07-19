using Enemy;
using GridSystem;
using Items;
using Place;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
        public ItemTableSO itemTableSO;
        public PlaceTableSO placeTableSO;
        public EnemyTableSO enemyTableSO;

        private void Start()
        {
            var bag = PlayerManager.I.PlayerBag.Bag;
            bag.AddBag(new Item.Serialization(2, 0f));
            bag.AddBag(new Item.Serialization(1, 64));

            enemyTableSO.table[1].Create(new Vector3(10, 0, 10), new Vector3(0, 0, 0));
        }
    }
}
