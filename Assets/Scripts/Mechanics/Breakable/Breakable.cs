using MyBox;
using Phys;
using UnityEngine;
using World;

namespace Mechanics {
    public class Breakable : Solid, IResettable {
        private GameObject _particles;

        protected virtual string ParticlePath() => "PS_Breakable";

        private void Awake()
        {
            _particles = (GameObject) Resources.Load(ParticlePath());
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

        public void Break() {
            gameObject.SetActive(false);
            Instantiate(_particles, transform.position, Quaternion.identity);
            //Instantiate(ParticlePrefab, transform.position, Quaternion.identity);
        }

        public void Reset()
        {
            gameObject.SetActive(true);
        }
    }
}