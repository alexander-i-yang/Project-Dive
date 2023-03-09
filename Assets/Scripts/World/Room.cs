using System.Collections;

using UnityEngine;
using Cinemachine;

using Helpers;
using Player;


namespace World {
    public class Room : MonoBehaviour, IFilterLoggerTarget {
        private Collider2D _roomCollider;
        private CinemachineVirtualCamera _vCam;
        private PlayerSpawnManager _player;
        private CinemachineBrain _cmBrain;

        public bool StopTime = true;

        private Spawn[] _spawns;
        private IResettable[] _resettables = new IResettable[0];
        private GameObject _grid;
        public Spawn[] Spawns
        {
            get
            {
                if (_spawns == null)
                {
                    _spawns = GetComponentsInChildren<Spawn>();
                }
                return _spawns;
            }
        }
        private static Coroutine _transitionRoutine;

        public delegate void OnRoomTransition(Room roomEntering);
        public static event OnRoomTransition RoomTransitionEvent;

        private void Awake()
        {
            _roomCollider = GetComponent<Collider2D>();
            _player = FindObjectOfType<PlayerSpawnManager>(true);
            _cmBrain = FindObjectOfType<CinemachineBrain>(true);

            _vCam = GetComponentInChildren<CinemachineVirtualCamera>(true);
            _vCam.Follow = _player.transform;
            _resettables = GetComponentsInChildren<IResettable>();

            _grid = transform.GetChild(0).gameObject;
        }

        void Start()
        {
            SetRoomGridEnabled(false);
        }

        private void OnValidate()
        {
            Spawn spawn = GetComponentInChildren<Spawn>();
            if (spawn == null)
            {
                FilterLogger.LogWarning(this, $"The room {gameObject.name} does not have a spawn point. Every room should have at least one spawn point.");
            }
        }

        private void Update()
        {
            float dist2CameraToRoomCenter = Vector3.SqrMagnitude(Camera.main.transform.position - _roomCollider.transform.position);
            bool shouldEnable = dist2CameraToRoomCenter < _roomCollider.bounds.size.sqrMagnitude * 3;   //Generous enabling range
            if (shouldEnable != _grid.gameObject.activeSelf)
            {
                SetRoomGridEnabled(shouldEnable);
            }
        }

        //Source: https://answers.unity.com/questions/501893/calculating-2d-camera-bounds.html
        public static Bounds OrthograpicBounds(Camera camera)
        {
            float screenAspect = (float) Screen.width / (float) Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            Bounds bounds = new Bounds(
                camera.transform.position,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
            return bounds;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            bool isPlayer = other.gameObject == _player.gameObject;
            bool needTransition = _player.CurrentRoom != this;
            if (isPlayer && needTransition) 
            {
                /*
                 * This check ensures that the player can only ever be in one room at a time.
                 * It says that not only does the player need to collide, but the entire bounding box needs to be in the room.
                 */
                bool boundsCheck = _roomCollider.bounds.Contains(other.bounds.min) && _roomCollider.bounds.Contains(other.bounds.max);
                if (boundsCheck)
                {
                    TransitionToThisRoom();
                }
            }
        }

        public virtual void TransitionToThisRoom()
        {
            SetRoomGridEnabled(true);
            FilterLogger.Log(this, $"Transitioned to room: {gameObject.name}");
            if (_transitionRoutine != null)
            {
                StopCoroutine(_transitionRoutine);
            }
            _transitionRoutine = StartCoroutine(TransitionRoutine());
        }

        private IEnumerator TransitionRoutine()
        {
            SwitchCamera();
            if (StopTime) Time.timeScale = 0f;
            /*
             * This is kinda "cheating". Instead of waiting for the camera to be done switching,
             * we're just waiting for the same amount of time as the blend time between cameras.
             */
            yield return new WaitForSecondsRealtime(_cmBrain.m_DefaultBlend.BlendTime);
            if (StopTime) Time.timeScale = 1f;
            _transitionRoutine = null;
            RoomTransitionEvent?.Invoke(this);
        }

        private void SwitchCamera()
        {
            //L: Inefficient, but not terrible?
            this._vCam.gameObject.SetActive(true);
            Reset();
            foreach (Room room in RoomList.Rooms)
            {
                if (room != this)
                {
                    room._vCam.gameObject.SetActive(false);
                }
            }
        }
        
        public void Reset()
        {
            foreach (var r in _resettables)
            {
                if (r != null) r.Reset();
            }
        }
        
        public void SetRoomGridEnabled(bool setActive)
        {
            _grid.SetActive(setActive);
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Error;
        }
    }
}