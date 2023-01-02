using UnityEngine;
using MyBox;
using UnityEngine.U2D;

using Audio;

namespace Core
{
    public class Game : Singleton<Game>
    {
        [Range(0, 1)] public float TimeScale = 1;
        public bool IsPaused;

        public float DeltaTime;
        public float FixedDeltaTime;

        private Camera _mainCamera;
        public Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    return FindObjectOfType<Camera>();
                }
                return _mainCamera;
            }

            private set { }
        }

        public delegate void ResetNFOAction();
        public event ResetNFOAction ResetNextFrameOffset;
        // public AudioClip music;



        void Awake()
        {
            Application.targetFrameRate = 60;
            InitializeSingleton();

            _mainCamera = FindObjectOfType<Camera>();
        }

        private void Start()
        {
            AudioManager.PlayMusic("Mus_Level_S");  //TEMPORARY
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