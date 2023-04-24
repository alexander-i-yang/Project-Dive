using Player;
using UnityEngine;

namespace World
{
    public class EndCutsceneManager : MonoBehaviour
    {
        public static bool IsBeegBouncing = false;
        public static bool IsEndCutscene = false;
        public delegate void OnBeegBounce();
        public static event OnBeegBounce BeegBounceStartEvent;
        
        public delegate void OnEndCutscene();
        public static event OnEndCutscene EndCutsceneEvent;

        [SerializeField] public Room[] roomsToEnable;
        

        //Called in Unity Event
        public void StartBeegBounce()
        {
            if (!IsBeegBouncing)
            {
                IsBeegBouncing = true;
                BeegBounceStartEvent?.Invoke();
                foreach (var r in roomsToEnable)
                {
                    r.SetRoomGridEnabled(true);
                }
            }
        }

        public static void StartCutscene()
        {
            IsEndCutscene = true;
            print("STATrt cutscene");
            EndCutsceneEvent?.Invoke();
        }
    }
}