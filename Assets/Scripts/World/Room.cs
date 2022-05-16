using System;
using UnityEngine;

namespace World {
    public class Room : MonoBehaviour {
        private void OnCollisionEnter(Collision other) {
            Debug.Log("heyo");
        }
    }
}