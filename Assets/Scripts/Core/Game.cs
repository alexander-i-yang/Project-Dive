using UnityEngine;
using MyBox;

using Audio;

namespace Core
{
    public class Game : Singleton<Game>
    {
        [Range(0, 1)] public float TimeScale = 1;
        public bool IsPaused;

        public float DeltaTime;
        public float FixedDeltaTime;
        public delegate void ResetNFOAction();
        public event ResetNFOAction ResetNextFrameOffset;
        // public AudioClip music;

        void Awake()
        {
            Application.targetFrameRate = 60;
            InitializeSingleton();
        }

        private void Start()
        {
            AudioManager.PlayMusic("Mus_Level_S");
        }

        void Update()
        {
            DeltaTime = IsPaused ? 0 : Time.deltaTime * TimeScale;
        }

        private void FixedUpdate()
        {
            if (ResetNextFrameOffset != null) ResetNextFrameOffset();
            FixedDeltaTime = IsPaused ? 0 : Time.fixedDeltaTime * TimeScale;
        }
    }
}