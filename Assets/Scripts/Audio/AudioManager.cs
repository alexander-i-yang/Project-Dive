using System;
using System.Collections;
using UnityEngine;
using FMODUnity;

using MyBox;

namespace Audio
{
    //Source: Slider by Daniel Carr
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private Sound[] sounds;
        private static Sound[] _sounds;

        [SerializeField] private Music[] music;
        private static Music[] _music;

        [SerializeField] private string musicBusPath;
        [SerializeField] private string sfxBusPath;

        [SerializeField] private AnimationCurve soundDampenCurve;
        private static AnimationCurve _soundDampenCurve;
        private static Coroutine soundDampenCoroutine;

        private static float sfxVolume = 1f; // [0..1]
        private static float musicVolume = 1f;
        private static float musicVolumeMultiplier = 1; // for music effects

        private static FMOD.Studio.Bus sfxBus;
        private static FMOD.Studio.Bus musicBus;

        void Awake()
        {
            InitializeSingleton();
            DontDestroyOnLoad(gameObject);

            _sounds = sounds;
            _music = music;
            _soundDampenCurve = soundDampenCurve;

            foreach (Sound s in _sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }

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

        public static void Play(string name)
        {
            if (_sounds == null)
                return;
            Sound s = Array.Find(_sounds, sound => sound.name == name);

            if (s == null)
            {
                Debug.LogError("Sound: " + name + " not found!");
                return;
            }

            if (s.doRandomPitch)
                s.source.pitch = s.pitch * UnityEngine.Random.Range(.95f, 1.05f);
            else
                s.source.pitch = s.pitch;

            s.source.Play();
        }

        public static void PlayWithPitch(string name, float pitch) //Used In Ocean Scene
        {
            if (_sounds == null)
                return;
            Sound s = Array.Find(_sounds, sound => sound.name == name);

            if (s == null)
            {
                Debug.LogError("Sound: " + name + " not found!");
                return;
            }

            if (s.doRandomPitch)
                s.source.pitch = s.pitch * UnityEngine.Random.Range(.95f, 1.05f) * pitch;
            else
                s.source.pitch = s.pitch * pitch;

            s.source.Play();
        }


        public static void PlayWithVolume(string name, float volumeMultiplier)
        {
            if (_sounds == null)
                return;
            Sound s = Array.Find(_sounds, sound => sound.name == name);

            if (s == null)
            {
                Debug.LogError("Sound: " + name + " not found!");
                return;
            }

            if (s.doRandomPitch)
                s.source.pitch = s.pitch * UnityEngine.Random.Range(.95f, 1.05f);
            else
                s.source.pitch = s.pitch;

            s.source.volume = s.volume * sfxVolume * volumeMultiplier;
            s.source.Play();
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

        public static void StopSound(string name)
        {
            if (_sounds == null)
                return;
            Sound s = Array.Find(_sounds, sound => sound.name == name);

            if (s == null)
            {
                Debug.LogError("Sound: " + name + " not found!");
                return;
            }

            s.source.Stop();
        }

        public static void StopAllSoundAndMusic()
        {
            foreach (Music m in Instance.music)
            {
                m.emitter.Stop();
            }
            foreach (Sound s in Instance.sounds)
            {
                s.source.Stop();
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


        public static void SetSFXVolume(float value)
        {
            value = Mathf.Clamp(value, 0, 1);
            sfxVolume = value;

            if (_sounds == null)
                return;
            foreach (Sound s in _sounds)
            {
                if (s == null || s.source == null)
                    continue;
                s.source.volume = s.volume * value;
            }

            sfxBus.setVolume(value);
        }

        public static void SetMusicVolume(float value)
        {
            musicVolume = Mathf.Clamp(value, 0, 1);
            UpdateMusicVolume();
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

        public static void DampenMusic(float amount, float length)
        {
            StopDampen();
            soundDampenCoroutine = Instance.StartCoroutine(_DampenMusic(amount, length));
        }

        public static void StopDampen()
        {
            if (soundDampenCoroutine != null)
            {
                Instance.StopCoroutine(soundDampenCoroutine);
                soundDampenCoroutine = null;
            }
        }

        private static IEnumerator _DampenMusic(float amount, float length)
        {
            float t = 0;

            while (t < length)
            {
                SetMusicVolumeMultiplier(Mathf.Lerp(amount, 1, _soundDampenCurve.Evaluate(t / length)));

                yield return null;
                t += Time.deltaTime;
            }

            SetMusicVolumeMultiplier(1);
            soundDampenCoroutine = null;
        }

        public static float GetSFXVolume()
        {
            return sfxVolume;
        }

        public static float GetMusicVolume()
        {
            return musicVolume;
        }

        public static void SetSFXPitch(float value)
        {
            value = Mathf.Clamp(value, 0.3f, 3f);

            if (_sounds == null)
                return;
            foreach (Sound s in _sounds)
            {
                if (s == null || s.source == null)
                    continue;
                s.source.pitch = s.pitch * value;
            }
        }
    }
}