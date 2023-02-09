using FMODUnity;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(StudioEventEmitter))]
    public class MusicStopper : MonoBehaviour
    {
        private StudioEventEmitter _emitter;

        void Awake()
        {
            _emitter = GetComponent<StudioEventEmitter>();
        }

        public void Stop()
        {
            FMOD.Studio.EventInstance eventInstance = _emitter.EventInstance;
            // eventInstance.setPaused(true);
            _emitter.Stop();
        }
    }
}