using System;
using UnityEngine;

namespace Collectibles
{
    public class FireflyPoint : MonoBehaviour
    {
        public GameObject Next;

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 2);
        }
        #endif
    }
}