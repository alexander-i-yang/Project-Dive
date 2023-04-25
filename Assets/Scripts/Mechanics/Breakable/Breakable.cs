using Cinemachine.Utility;
using Phys;
using UnityEngine;
using World;

namespace Mechanics {
    public class Breakable : Solid, IResettable {
        private GameObject _particles;
        private Vector2 _boxSize;
        [SerializeField] private float particleDensity = 0.01f;
        
        protected virtual string ParticlePath() => "PS_Breakable";

        private void Awake()
        {
            _particles = (GameObject) Resources.Load(ParticlePath());
            
            var colliderPts = GetComponent<EdgeCollider2D>().points;
            _boxSize = (colliderPts[2] - colliderPts[0]).Abs();
        }

        public override bool Collidable() {
            Resources.Load(ParticlePath());
            return true;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            if (p.IsDiving() && direction.y < 0) {
                //p.SpawnDrillingParticles();
                Break();
                return false;
            }
            
            return base.PlayerCollide(p, direction);
        }

        public override bool IsGround(PhysObj whosAsking) {
            PlayerActor p = whosAsking.GetComponent<PlayerActor>();
            if (p != null) {
                return !p.IsDiving();
            }
            return true;
        }

        public virtual void Break() {
            gameObject.SetActive(false);
            var particles = Instantiate(_particles, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            var particlesShape = particles.shape;
            particlesShape.scale = _boxSize;
            particles.Emit((int)(_boxSize.x * _boxSize.y * particleDensity));
            //Instantiate(ParticlePrefab, transform.position, Quaternion.identity);
        }

        public void Reset()
        {
            gameObject.SetActive(true);
        }
        
        public virtual bool CanReset()
        {
            return gameObject != null;
        }
    }
}