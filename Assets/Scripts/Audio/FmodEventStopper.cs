using FMODUnity;
using UnityEngine;
using System.Collections.Generic;

namespace Audio
{
    public class FmodEventStopper : MonoBehaviour
    {
        [SerializeField] private List<StudioEventEmitter> emitters;

        public void Stop()
        {
            foreach (StudioEventEmitter e in emitters)
            {
                FMOD.Studio.EventInstance eventInstance = e.EventInstance;
                //eventInstance.setPaused(true);
                var result = eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

//#if UNITY_EDITOR
//                Debug.Log(result);

//                if (result != FMOD.RESULT.OK)
//                {
//                    Debug.Break();
//                }
//#endif
                e.Stop();
            }

        }
    }
}