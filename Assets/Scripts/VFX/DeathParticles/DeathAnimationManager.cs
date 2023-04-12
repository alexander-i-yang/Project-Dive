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

        private PlayerActor _actor;
        private PlayerStateMachine _stateMachine;
        private InfluencedParticleSystem _smokeParticles;
        private InfluencedParticleSystem _sparkParticles;

        [SerializeField] private float smokeParticleInheritVWeight = 1f;
        [SerializeField] private int smokeParticleCount = 15;
        
        [SerializeField] private float deathParticleInheritVWeight = 1f;
        [SerializeField] private float deathParticlePersistTime = 1f;
        [SerializeField] private float deathParticleFadeTime = 1f;

        [MinMaxRange(0, 200), SerializeField]
        private RangedInt velocityRange = new(100, 200);

        private DeathParticlePool _deathParticlePool;

        void Awake()
        {
            _parts = GetComponentsInChildren<DeathParticle>();
            _actor = GetComponentInParent<PlayerActor>();
            _stateMachine = GetComponentInParent<PlayerStateMachine>();

            InfluencedParticleSystem[] psystems = GetComponentsInChildren<InfluencedParticleSystem>();
            _smokeParticles = psystems[0];
            _sparkParticles = psystems[1];
            
            _deathParticlePool = FindObjectOfType<DeathParticlePool>();
            
            foreach (var part in _parts)
            {
                part.gameObject.SetActive(false);
            }
        }

        public void Launch(Vector3 actorV)
        {
            foreach (var part in _parts)
            {
                float angle = Random.Range(0, 360);
                float magnitude = Random.Range(velocityRange.Min, velocityRange.Max);
                Vector2 v = new Vector2((float)(Math.Cos(angle) * magnitude), (float)(Math.Sin(angle) * magnitude));
                
                DeathParticle newPart = Instantiate(part.gameObject, _deathParticlePool.transform).GetComponent<DeathParticle>();
                newPart.transform.position = part.transform.position;
                newPart.gameObject.SetActive(true);
                newPart.Init();
                newPart.Launch(
                    v + (Vector2) actorV * deathParticleInheritVWeight,
                    deathParticlePersistTime,
                    deathParticleFadeTime
                );
            }
        }

        public void DeadStop()
        {
            _actor.DeadStop();
        }
        
        //Called in Unity Animator - Do not delete
        #region AnimatorEvents
        public void TriggerParticles()
        {
            Vector3 actorV = _actor.velocity;
            _smokeParticles.Emit(actorV);
            _sparkParticles.Emit(actorV);
            DeadStop();
            Launch(actorV);
        }
        
        public void Respawn()
        {
            _stateMachine.Respawn();
        }
        #endregion
    }
}