using System;
using Cinemachine;
using Player;
using UnityEngine;

namespace World
{
    public class EndCutsceneManager : MonoBehaviour
    {
        public static bool IsBeegBouncing => PlayerCore.Actor.IsBeegBouncing;
        public static bool IsEndCutscene = false;

        public delegate void OnEndCutscene();
        public static event OnEndCutscene EndCutsceneEvent;

        [SerializeField] public Room[] roomsToEnable;

        [SerializeField] private CinemachineVirtualCamera lastVCam;
        [SerializeField] private CinemachineVirtualCamera preCutsceneCam;

        private void OnEnable()
        {
            EndCutsceneEvent += OnStartCutscene;
        }
        
        private void OnDisable()
        {
            EndCutsceneEvent -= OnStartCutscene;
        }

        //Called in Unity Event
        public void StartBeegBounce()
        {
            /*if (!IsBeegBouncing)
            {
                IsBeegBouncing = true;
                BeegBounceStartEvent?.Invoke();
                
            }*/
            foreach (var r in roomsToEnable)
            {
                r.SetRoomGridEnabled(true);
                r.VCam.gameObject.SetActive(false);
            }

            PlayerCore.SpawnManager.ActivateLastVCam(lastVCam, true);
        }

        public static void StartCutscene()
        {
            IsEndCutscene = true;
            EndCutsceneEvent?.Invoke();
        }

        private void OnStartCutscene()
        {
            PlayerCore.SpawnManager.ActivateLastVCam(lastVCam, false);
            PlayerCore.SpawnManager.ActivateLastVCam(preCutsceneCam, true);
            print(preCutsceneCam.gameObject.activeSelf);
        }
        
        //Called in Unity Event
        public void OnRestart()
        {
            IsEndCutscene = false;
        }
    }
}