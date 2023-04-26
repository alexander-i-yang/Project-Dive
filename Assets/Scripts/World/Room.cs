using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

using Helpers;
using Phys;
using Player;
using UnityEditor;
using UnityEngine.Serialization;


namespace World {
    public class Room : MonoBehaviour, IFilterLoggerTarget {
        private Collider2D _roomCollider;
        public CinemachineVirtualCamera VCam { get; private set; }
        private PlayerSpawnManager _player;
        private CinemachineBrain _cmBrain;

        public bool StopTime = true;

        private Spawn[] _spawns;
        private IResettable[] _resettables = new IResettable[0];
        private GameObject _grid;
        public Spawn[] Spawns => _spawns;
        private static Coroutine _transitionRoutine;

        private int _numEnables;
        [SerializeField] private int _maxEnables = 4;

        public delegate void OnRoomTransition(Room roomEntering);
        public static event OnRoomTransition RoomTransitionEvent;

        public Room[] AdjacentRooms;
        private EndCutsceneManager _endCutsceneManager;

        private void Awake()
        {
            _roomCollider = GetComponent<Collider2D>();
            _player = FindObjectOfType<PlayerSpawnManager>(true);
            _cmBrain = FindObjectOfType<CinemachineBrain>(true);

            _endCutsceneManager = FindObjectOfType<EndCutsceneManager>();

            VCam = GetComponentInChildren<CinemachineVirtualCamera>(true);
            VCam.Follow = _player.transform;
            
            FetchMechanics();
            _grid = transform.GetChild(0).gameObject;
        }

        /*private void OnEnable()
        {
            EndCutsceneManager.BeegBounceStartEvent += TurnOffStopTime;
        }

        private void OnDisable()
        {
            EndCutsceneManager.BeegBounceStartEvent -= TurnOffStopTime;
        }*/

        void TurnOffStopTime()
        {
            StopTime = false;
        }

        void FetchMechanics()
        {
            _resettables = GetComponentsInChildren<IResettable>(includeInactive:true);
            _spawns = GetComponentsInChildren<Spawn>(includeInactive:true);
        }

        void Start()
        {
            if (this == _player.CurrentRoom)
            {
                //Don't disable current room
                /*foreach (var r in AdjacentRooms)
                {
                    r.RoomSetEnable(true);
                }*/
            }
            else
            {
                SetRoomGridEnabled(false);
            }
        }
        private void OnValidate()
        {
            Spawn spawn = GetComponentInChildren<Spawn>();
            if (spawn == null)
            {
                FilterLogger.LogWarning(this, $"The room {gameObject.name} does not have a spawn point. Every room should have at least one spawn point.");
            }
        }

        /*private void Update()
        {
            float dist2CameraToRoomCenter = Vector3.SqrMagnitude(Camera.main.transform.position - _roomCollider.transform.position);
            bool shouldEnable = dist2CameraToRoomCenter < _roomCollider.bounds.size.sqrMagnitude * 1.1;
            if (_player.CurrentRoom == this) SetRoomGridEnabled(true);
            if (shouldEnable != _grid.gameObject.activeSelf)
            {
                SetRoomGridEnabled(shouldEnable);
            }
        }*/

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

        public float GetRoomSize()
        {
            Vector3 dims = _roomCollider.bounds.size;
            return dims.x * dims.y;
        }

        public virtual void TransitionToThisRoom()
        {
            SetRoomGridEnabled(true);
            Reset();
            FilterLogger.Log(this, $"Transitioned to room: {gameObject.name}");
            if (_transitionRoutine != null)
            {
                StopCoroutine(_transitionRoutine);
            }
            _transitionRoutine = StartCoroutine(TransitionRoutine());
        }

        private IEnumerator TransitionRoutine()
        {
            if (!EndCutsceneManager.IsEndCutscene && !EndCutsceneManager.IsBeegBouncing)
            {
                SwitchCamera();
            }
            // SwitchCamera();
            // Door curDoor = Helper.OnComponent<Door>(_player.transform.position, LayerMask.GetMask("Default"));
            // print(curDoor);
            bool shouldStopTime = StopTime && !EndCutsceneManager.IsBeegBouncing;
            
            if (shouldStopTime) Time.timeScale = 0f;
            /*
             * This is kinda "cheating". Instead of waiting for the camera to be done switching,
             * we're just waiting for the same amount of time as the blend time between cameras.
             */
            yield return new WaitForSecondsRealtime(_cmBrain.m_DefaultBlend.BlendTime);
            if (shouldStopTime) Time.timeScale = 1f;
            _transitionRoutine = null;
            RoomTransitionEvent?.Invoke(this);
        }

        protected void SwitchCamera()
        {
            //L: Inefficient, but not terrible?
            this.VCam.gameObject.SetActive(true);
            foreach (Room room in RoomList.Rooms)
            {
                if (room != this)
                {
                    room.VCam.gameObject.SetActive(false);
                }
            }
        }
        
        public void Reset()
        {
            foreach (var r in _resettables)
            {
                if (r != null && r.CanReset()) r.Reset();
            }
        }
        
        public void SetRoomGridEnabled(bool setActive)
        {
            if (!EndCutsceneManager.IsEndCutscene || _endCutsceneManager == null)
            {
                _grid.SetActive(setActive);
            }
            else
            {
                bool shouldEnable = _endCutsceneManager.roomsToEnable.Contains(this);
                _grid.SetActive(shouldEnable);
            }
        }

        private void DestroyAndRecreateGrid()
        {
            GameObject gridObj = _grid.gameObject; 
            var newGrid = Instantiate(
                gridObj,
                gridObj.transform.position,
                Quaternion.identity,
                gridObj.transform.parent
            );

            gridObj.transform.parent = null; //This is so the mechanics in _grid aren't counted in FetchMechanics.
            Destroy(gridObj);
            _grid = newGrid;
            FetchMechanics();
        }

        public void RoomSetEnable(bool enable)
        {
            if (enable)
            {
                _numEnables++;
                if (_numEnables > _maxEnables)
                {
                    DestroyAndRecreateGrid();
                }
            }
            SetRoomGridEnabled(enable);
            Reset();
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Error;
        }

        public Door[] CalcAdjacentDoors(Vector2 doorAdjacencyTolerance, LayerMask doorLayerMask)
        {
            if (_roomCollider == null) _roomCollider = GetComponent<Collider2D>();
            var bounds = _roomCollider.bounds;
            Vector2 pointB = (Vector2)bounds.max + doorAdjacencyTolerance;
            Vector2 pointA = (Vector2)bounds.min - doorAdjacencyTolerance;

            var innerDoors = GetComponentsInChildren<Door>();

            var hits = Physics2D.OverlapAreaAll(pointA, pointB, doorLayerMask);
            List<Door> ret = new();
            foreach (var hit in hits)
            {
                Door d = hit.GetComponent<Door>();
                if (d != null && !innerDoors.Contains(d))
                {
                    ret.Add(d);
                }
            }

            return ret.ToArray();
        }

        public Room[] CalcAdjacentRooms(Vector2 roomAdjacencyTolerance, LayerMask roomLayerMask)
        {
            if (_roomCollider == null) _roomCollider = GetComponent<Collider2D>();
            var bounds = _roomCollider.bounds;
            Vector2 pointB = (Vector2)bounds.max + roomAdjacencyTolerance;
            Vector2 pointA = (Vector2)bounds.min - roomAdjacencyTolerance;

            var hits = Physics2D.OverlapAreaAll(pointA, pointB, roomLayerMask);
            List<Room> ret = new();
            foreach (var hit in hits)
            {
                Room r = hit.GetComponent<Room>();
                if (r != null && r != this)
                {
                    ret.Add(r);
                }
            }

            return ret.ToArray();
        }
    }
}