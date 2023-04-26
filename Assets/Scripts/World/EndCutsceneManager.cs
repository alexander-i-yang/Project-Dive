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

            PlayerCore.SpawnManager.ActivateLastVCam(lastVCam);
        }

        public static void StartCutscene()
        {
            IsEndCutscene = true;
            EndCutsceneEvent?.Invoke();
        }
    }
}