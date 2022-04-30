using System;
using UnityEngine;

namespace DefaultNamespace {
    public class Game : MonoBehaviour {
        public static bool IsPaused;

        public static float DeltaTime;
        public static float FixedDeltaTime;

        void Awake() {Application.targetFrameRate = 60;}

        void Update() {
            DeltaTime = IsPaused ? 0 : Time.deltaTime;
        }

        private void FixedUpdate() {
            FixedDeltaTime = IsPaused ? 0 : Time.fixedDeltaTime;
        }
    }
}