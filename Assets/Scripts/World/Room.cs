
using Cinemachine;
using MyBox;

using Helpers;

using System.Collections;

using UnityEngine;

namespace World {
    public class Room : MonoBehaviour, IFilterLoggerTarget {
        [SerializeField, AutoProperty(AutoPropertyMode.Children)] private CinemachineVirtualCamera _vCam;
        [SerializeField, AutoProperty(AutoPropertyMode.Scene)] private PlayerActor _player;
        [SerializeField, AutoProperty(AutoPropertyMode.Scene)] private CinemachineBrain _cmBrain;

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
            if (_roomList == null || _roomList.Length == 0)
            {
                _roomList = FindObjectsOfType<Room>();
                //Debug.Log($"Initialized Room List: Found {_roomList.Length} rooms.");
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
            FilterLogger.Log(this, $"Transitioned to room: {gameObject.name}");
            TransitionTo(this);
        }

        public static void TransitionTo(Room roomToTransition)
        {
            if (_transitionRoutine != null)
            {
                roomToTransition.StopCoroutine(_transitionRoutine);
            }

            roomToTransition.StartCoroutine(roomToTransition.TransitionRoutine());
        }

        private IEnumerator TransitionRoutine()
        {
            Time.timeScale = 0f;
            StartCameraSwitch();
            yield return new WaitForSecondsRealtime(_cmBrain.m_DefaultBlend.BlendTime);
            Time.timeScale = 1f;
            RoomTransitionEvent?.Invoke(this);
        }

        private void StartCameraSwitch()
        {
            //L: Inefficient but not terrible
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