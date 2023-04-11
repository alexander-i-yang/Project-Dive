using System;
using Helpers;
using MyBox;
using Player;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace VFX
{
    public class DeathAnimationManager : MonoBehaviour
    {
        private DeathParticle[] _parts;
        
        private Material _whiteMaterial;

        [MinMaxRange(0, 1000)]
        [SerializeField]
        private RangedInt velocityRange = new RangedInt(100, 200);

        void Awake()
        {
            // _whiteMaterial = Resources.Load("White", typeof(Material)) as Material;
            _parts = GetComponentsInChildren<DeathParticle>();
            SetParticlesActive(false);
            // Launch();
            // StartCoroutine(Helper.DelayAction(3f, Reset));
        }

        public void Launch()
        {
            foreach (var part in _parts)
            {
                float angle = Random.Range(0, 360);
                float magnitude = Random.Range(velocityRange.Min, velocityRange.Max);
                Vector2 v = new Vector2((float)(Math.Cos(angle) * magnitude), (float)(Math.Sin(angle) * magnitude));
                part.Launch(v);
            }
        }

        public void Reset()
        {
            SetParticlesActive(true);
            foreach (var part in _parts)
            {
                part.Reset();
            }
            Launch();
        }

        public void SetParticlesActive(bool setActive)
        {
            foreach (var part in _parts)
            {
                part.gameObject.SetActive(setActive);
            }
        }

        public void DisableParticles() => SetParticlesActive(false);

        public void Respawn()
        {
            PlayerCore.StateMachine.Respawn();
        }
    }
}