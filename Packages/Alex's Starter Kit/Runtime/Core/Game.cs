using ASK.ScreenShake;
using UnityEngine;
using MyBox;

namespace ASK.Core
{
    public class Game : Singleton<Game>
    {
        [SerializeField] private string musicName;
        public float TimeScale = 1;
        public bool IsPaused;
        public bool DebugBreak;

        public Vector2 ScreenSize = new Vector2(256, 144);
        
        public float DeltaTime { get; private set; }
        public float FixedDeltaTime { get; private set; }
        public float Time { get; private set; } = 0;

        [SerializeField] private int stepFrames;
        private int _frameCount;

        public int FakeControlsArrows = -2;

        public delegate void OnDebug();
        public event OnDebug OnDebugEvent;

        private bool _isDebug;
        public bool IsDebug
        {
            get => _isDebug;
            set
            {
                _isDebug = value;
                if (value)
                {
                    OnDebugEvent?.Invoke();
                }
            }
        }

        public CameraController CamController;

        public delegate void ResetNFOAction();
        public event ResetNFOAction ResetNextFrameOffset;
        // public AudioClip music;

        void Awake()
        {
            Application.targetFrameRate = 60;
            InitializeSingleton();

            // FMODUnity.RuntimeManager.LoadBank("Master", true);
            CamController = GetComponent<CameraController>();
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
            
            if (DebugBreak && _frameCount % stepFrames == 0) Debug.Break();
            _frameCount = (_frameCount + 1) % 10000;
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}