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
                // eventInstance.setPaused(true);
                eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                e.Stop();
            }

        }
    }
}