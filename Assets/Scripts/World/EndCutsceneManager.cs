using Player;
using UnityEngine;

namespace World
{
    public class EndCutsceneManager : MonoBehaviour
    {
        public static bool IsEndCutscene = false;
        public delegate void OnEndCutscene();
        public static event OnEndCutscene EndCutsceneEvent;

        //Called in Unity Event
        public void StartCutscene()
        {
            IsEndCutscene = true;
            print("START END CUTSCENE");
            EndCutsceneEvent?.Invoke();
        }
    }
}