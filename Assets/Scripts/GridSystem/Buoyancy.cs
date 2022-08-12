using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    [RequireComponent(typeof(BoxCollider))]
    public class Buoyancy : MyScript
    {
        private float p;
        private float floor;
        private float friction;

        private void Awake()
        {
            p = MapManager.I.p;
            floor = MapManager.I.floor;
            friction = MapManager.I.friction;
        }

        public void Initialization(int width, int height)
        {
            var col = GetComponent<BoxCollider>();
            col.size = new Vector3(width, 10, height);
            col.center = new Vector3(width / 2, floor - 5, height / 2);
        }

        private void OnTriggerStay(Collider other)
        {
            var rb = other.GetComponent<Rigidbody>();
            if (rb == null)
                return;
            rb.AddForce(Vector3.up * p * Mathf.Clamp(floor - other.transform.position.y, 0, float.MaxValue));
            rb.velocity += Vector3.Lerp(Vector3.zero, -rb.velocity, friction);
        }
    }
}
