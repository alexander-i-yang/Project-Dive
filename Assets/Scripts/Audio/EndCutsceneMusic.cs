using System;
using FMODUnity;
using UnityEngine;
using World;

namespace Audio
{
    [RequireComponent(typeof(StudioEventEmitter))]
    public class EndCutsceneMusic : MonoBehaviour
    {
        private StudioEventEmitter _emitter;

        private void Awake()
        {
            _emitter = GetComponent<StudioEventEmitter>();
        }

        private void OnEnable()
        {
            EndCutsceneManager.EndCutsceneEvent += PlayMusic;
        }

        private void OnDisable()
        {
            EndCutsceneManager.EndCutsceneEvent -= PlayMusic;
        }

        private void PlayMusic()
        {
            _emitter.Play();
        }
    }
}