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

        private Spawn[] _spawns;
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

        private static Room[] _roomList;
        private static Coroutine _transitionRoutine;

        public delegate void OnRoomTransition(Room roomEntering);
        public static event OnRoomTransition RoomTransitionEvent;

        private void Awake()
        {
            _roomCollider = GetComponent<Collider2D>();
            _vCam = GetComponentInChildren<CinemachineVirtualCamera>(true);
            _player = FindObjectOfType<PlayerSpawnManager>(true);
            _cmBrain = FindObjectOfType<CinemachineBrain>(true);

            if (_roomList == null || _roomList.Length == 0)
            {
                _roomList = FindObjectsOfType<Room>(true);
                FilterLogger.Log(this, $"Initialized Room List: Found {_roomList.Length} rooms.");
            }

            _vCam.Follow = _player.transform;
        }

        private void OnValidate()
        {
            Spawn spawn = GetComponentInChildren<Spawn>();
            if (spawn == null)
            {
                FilterLogger.LogWarning(this, $"The room {gameObject.name} does not have a spawn point. Every room should have at least one spawn point.");
            }
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

        public void TransitionToThisRoom()
        {
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
            Time.timeScale = 0f;
            /*
             * This is kinda "cheating". Instead of waiting for the camera to be done switching,
             * we're just waiting for the same amount of time as the blend time between cameras.
             */
            yield return new WaitForSecondsRealtime(_cmBrain.m_DefaultBlend.BlendTime);
            Time.timeScale = 1f;
            _transitionRoutine = null;
            RoomTransitionEvent?.Invoke(this);
        }

        private void SwitchCamera()
        {
            //L: Inefficient, but not terrible?
            this._vCam.gameObject.SetActive(true);
            foreach (Room room in _roomList)
            {
                if (room != this)
                {
                    room._vCam.gameObject.SetActive(false);
                }
            }
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Error;
        }
    }
}