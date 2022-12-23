using Cinemachine;

using Helpers;

using System.Collections;

using UnityEngine;

namespace World {
    public class Room : MonoBehaviour, IFilterLoggerTarget {
        private CinemachineVirtualCamera _vCam;
        private PlayerActor _player;
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
            _vCam = GetComponentInChildren<CinemachineVirtualCamera>(true);
            _player = FindObjectOfType<PlayerActor>(true);
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

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.GetComponent<PlayerRoomManager>() != null)
            {
                TransitionToThisRoom();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            //This is a failsafe in case the player is between two rooms and exits one of them while the camera is still on it.
            var playerTrigger = other.GetComponent<PlayerRoomManager>();
            if (playerTrigger != null && playerTrigger.CurrentRoom == this)
            {
                foreach (Room room in playerTrigger.FindRoomsTouching())
                {
                    if (room != this)
                    {
                        room.TransitionToThisRoom();
                    }
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