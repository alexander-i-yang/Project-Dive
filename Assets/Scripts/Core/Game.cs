using UnityEngine;
using MyBox;

using Audio;
using FMODUnity;

namespace Core
{
    public class Game : Singleton<Game>
    {
        [Range(0, 1)] public float TimeScale = 1;
        public bool IsPaused;

        public float DeltaTime { get; private set; }
        public float FixedDeltaTime { get; private set; }
        public float Time { get; private set; } = 0;

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

            FMODUnity.RuntimeManager.LoadBank("Master", true);
        }

        private void Start()
        {
            AudioManager.PlayMusic("Cave_Music");  //TEMPORARY
        }

        void Update()
        {
            DeltaTime = IsPaused ? 0 : UnityEngine.Time.deltaTime * TimeScale;
            Time += DeltaTime;
        }

        private void FixedUpdate()
        {
            if (ResetNextFrameOffset != null) ResetNextFrameOffset();
            FixedDeltaTime = IsPaused ? 0 : UnityEngine.Time.fixedDeltaTime * TimeScale;
        }
    }
}