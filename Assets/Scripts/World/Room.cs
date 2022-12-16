
using Cinemachine;
using MyBox;

using System.Collections;

using UnityEngine;

namespace World {
    public class Room : MonoBehaviour {
        [SerializeField, AutoProperty(AutoPropertyMode.Children)] private CinemachineVirtualCamera vCamera;
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
                Debug.Log($"Initialized Room List: Found {_roomList.Length} rooms.");
            }
        }

        private void OnValidate()
        {
            Spawn spawn = GetComponentInChildren<Spawn>();
            if (spawn == null)
            {
                Debug.LogWarning($"The room {gameObject.name} does not have a spawn point. Every room should have at least one spawn point.");
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            Debug.Log($"Transitioned to room: {this.transform.parent.name}");
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
            foreach (Room room in _roomList)
            {
                bool isCorrectRoom = (this == room);
                room.vCamera.gameObject.SetActive(isCorrectRoom);
                room.vCamera.enabled = isCorrectRoom;
            }
        }
    }
}