using System;
using System.Collections;
using UnityEngine;
using FMODUnity;

using MyBox;
using UI;

namespace Audio
{
    //Source: Slider by Daniel Carr
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private Music[] music;
        private static Music[] _music;

        [SerializeField] private string sfxBusPath;
        [SerializeField] private string musicBusPath;

        private static float sfxVolume = 1f; // [0..1]
        private static float musicVolume = 1f;
        private static float musicVolumeMultiplier = 1; // for music effects

        private static FMOD.Studio.Bus sfxBus;
        private static FMOD.Studio.Bus musicBus;

        void Awake()
        {
            InitializeSingleton();
            DontDestroyOnLoad(gameObject);

            _music = music;

            foreach (Music m in _music)
            {
                m.emitter = gameObject.AddComponent<StudioEventEmitter>();
                m.emitter.EventReference = m.fmodEvent;
            }

            if (FMODUnity.RuntimeManager.HasBankLoaded("Master Bank"))
            {
                sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/" + sfxBusPath);
                musicBus = FMODUnity.RuntimeManager.GetBus("bus:/" + musicBusPath);
                SetSFXVolume(sfxVolume);
                SetMusicVolume(musicVolume);
            }
        }

        /*private void OnEnable()
        {
            OptionsController.SFXToggleEvent += SetSFXMute;
        }
        
        private void OnDisable()
        {
            OptionsController.SFXToggleEvent -= SetSFXMute;
        }*/

        private void SetSFXMute(bool e)
        {
            SetSFXVolume(e ? 1 : 0);
            print("SFX volume: " + sfxVolume);
        }
        
        private void SetMusicMute(bool e)
        {
            musicBus.setMute(!e);
        }

        private static Music GetMusic(string name)
        {
            if (_music == null)
            {
                Debug.LogWarning("Music Array is null");
                return null;
            }

            Music m = Array.Find(_music, music => music.name == name);

            if (m == null)
            {
                Debug.LogWarning("Music is null");
                return null;
            }

            return m;
        }

        public static void PlayMusic(string name)
        {
            PlayMusic(name, true);
        }

        public static void PlayMusic(string name, bool stopOtherTracks = true)
        {
            Music m = GetMusic(name);
            Debug.Log($"Playing Music: {m.name}");

            if (m == null)
                return;

            if (stopOtherTracks)
            {
                foreach (Music music in _music)
                {
                    music.emitter.Stop();
                }
            }

            m.emitter.Play();
        }

        public static void StopMusic(string name)
        {
            Music m = GetMusic(name);

            if (m == null)
                return;

            m.emitter.Stop();
        }

        public static void StopAllMusic()
        {
            foreach (Music m in _music)
            {
                if (m.emitter.IsPlaying())
                {
                    m.emitter.Stop();
                }
            }
        }

        public static void SetMusicParameter(string name, string parameterName, float value)
        {
            // for global parameters
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(parameterName, value);

            Music m = GetMusic(name);

            if (m == null)
                return;

            // for track-specific parameters
            m.emitter.SetParameter(parameterName, value);
        }

        public static void SetMusicVolume(float value)
        {
            musicVolume = Mathf.Clamp(value, 0, 1);
            UpdateMusicVolume();
        }

        public static void SetSFXVolume(float value)
        {
            sfxVolume = Mathf.Clamp(value, 0, 1);
            UpdateSFXVolume();
        }

        public static void SetMusicVolumeMultiplier(float value)
        {
            musicVolumeMultiplier = value;
            UpdateMusicVolume();
        }

        private static void UpdateMusicVolume()
        {
            float vol = Mathf.Clamp(musicVolume * musicVolumeMultiplier, 0, 1);

            if (_music == null)
                return;

            musicBus.setVolume(vol);
        }

        private static void UpdateSFXVolume()
        {
            float vol = Mathf.Clamp(sfxVolume, 0, 1);

            sfxBus.setVolume(vol);
        }

        public static float GetSFXVolume()
        {
            return sfxVolume;
        }

        public static float GetMusicVolume()
        {
            return musicVolume;
        }
    }
}