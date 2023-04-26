using System.Collections.Generic;
using System.Collections;

using UnityEngine;

using Helpers;
using World;
using System;
using MyBox;
using System.Linq;
using Cinemachine;
using UnityEditor;

namespace Player
{
    public class PlayerSpawnManager : MonoBehaviour, IFilterLoggerTarget
    {
        private Room _currentRoom;
        private Room _prevRoom;
        private Spawn _currentSpawnPoint;
        public Room CurrentRoom => _currentRoom;
        public CinemachineVirtualCamera CurrentVCam
        {
            get
            {
                if (EndCutsceneManager.IsBeegBouncing) return LastVCam;
                else return _currentRoom.VCam;
            }
        }

        public CinemachineVirtualCamera LastVCam { get; private set; }

        private SpriteRenderer _spriteR;
        [SerializeField] private float spawnAnimationTime = .5f;
        [SerializeField] private float roomSizeMaxReverb;

        public event Action OnPlayerRespawn;

        // public event Action OnRoomTransition;

        public Spawn CurrentSpawnPoint
        {
            get
            {
                if (_currentSpawnPoint == null)
                {
                    _currentSpawnPoint = FindClosestSpawnPoint();
                }

                return _currentSpawnPoint;
            }
            set { _currentSpawnPoint = value; }
        }

        void Start()
        {
            _spriteR = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnEnable()
        {
            Room.RoomTransitionEvent += OnRoomTransition;
            OnPlayerRespawn += ShaderRespawn;
        }

        private void OnDisable()
        {
            Room.RoomTransitionEvent -= OnRoomTransition;
            OnPlayerRespawn -= ShaderRespawn;
        }

        public void Respawn()
        {
            if (CurrentSpawnPoint != null)
            {
                _currentRoom.Reset();
                transform.position = CurrentSpawnPoint.transform.position;
            }
            
            OnPlayerRespawn?.Invoke();
        }

        private void OnRoomTransition(Room roomEntering)
        {
            Room[] prevRooms = Array.Empty<Room>();
            if (_currentRoom != null)
            {
                prevRooms = _currentRoom.AdjacentRooms;
            }

            _currentRoom = roomEntering;
            _currentSpawnPoint = FindClosestSpawnPoint();

            //Set Global Reverb Amount for FMOD Events.
            float roomSize = _currentRoom.GetRoomSize();
            float clampedReverb = Mathf.Clamp01(roomSize / roomSizeMaxReverb);

            FilterLogger.Log(this, $"New Room Size is {roomSize}");
            FilterLogger.Log(this, $"Set Reverb to {clampedReverb}");

            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ReverbAmount", clampedReverb);

            Room[] newRooms = roomEntering.AdjacentRooms;

            Room[] disableRooms = prevRooms.Except(newRooms).ToArray();

            foreach (var r in disableRooms)
            {
                //Idk why but except isn't working
                if (r != _currentRoom) r.RoomSetEnable(false);
            }

            foreach (var r in newRooms)
            {
                r.RoomSetEnable(true);
            }
        }

        private IEnumerator ShaderRespawnCo()
        {
            _spriteR.material = new Material(_spriteR.material);
            _spriteR.material.SetFloat("_Progress", 0);
            float timer = 0;
            while (timer < spawnAnimationTime)
            {
                timer += Time.deltaTime;
                _spriteR.material.SetFloat("_Progress", timer / spawnAnimationTime);
                yield return null;
            }

            _spriteR.material.SetFloat("_Progress", 2);
        }

        public void ShaderRespawn() {
            // StartCoroutine(ShaderRespawnCo());
        }

        private Spawn FindClosestSpawnPoint()
        {
            Spawn closest;
            if (_currentRoom == null)
            {
                FilterLogger.LogWarning(this, "Player is not in a room. Choosing arbitrary spawn.");
                Spawn[] allSpawns = FindObjectsOfType<Spawn>();
                closest = FindClosestSpawnPoint(allSpawns);
            }
            else
            {
                closest = FindClosestSpawnPoint(CurrentRoom.Spawns);
            }

            if (closest == null)
            {
                FilterLogger.LogError(this, "No spawn point was found for the player. There must be at least one spawn point in the scene.");
            }
            return closest;
        }

        private Spawn FindClosestSpawnPoint(IEnumerable<Spawn> spawns)
        {
            float closestDist = float.MaxValue;
            Spawn closest = null;
            FilterLogger.Log(this, $"CurrentRoom: {CurrentRoom}");
            FilterLogger.Log(this, $"Spawns: {CurrentRoom.Spawns.Length}");
            foreach (Spawn spawn in spawns)
            {
                FilterLogger.Log(this, $"{spawn}");
                float newDist = Vector2.Distance(transform.position, spawn.transform.position);
                if (newDist < closestDist)
                {
                    closestDist = newDist;
                    closest = spawn;
                }
            }

            return closest;
        }

        public bool IsTouchingRoom(Room r)
        {
            return Physics2D.IsTouching(GetComponent<Collider2D>(), r.GetComponent<Collider2D>());
        }
        
        public LogLevel GetLogLevel()
        {
            return LogLevel.Warning;
        }

        public void ActivateLastVCam(CinemachineVirtualCamera lastVCam, bool e)
        {
            lastVCam.gameObject.SetActive(e);
            LastVCam = lastVCam;
        }
    }
}