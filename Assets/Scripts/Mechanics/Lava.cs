using Helpers;
using MyBox;
using Phys;
using UnityEngine;

namespace Mechanics {
    public class Lava : Solid {
        [SerializeField] private float timeToDie = 0.01f;
        private GameTimer _timerToDie;
        private PlayerActor _lastPlayerTouched;

        public override bool Collidable() {
            //return true;
            return false;
        }

        private void Update()
        {
            GameTimer.Update(_timerToDie);
            if (_lastPlayerTouched != null && GameTimer.TimerFinished(_timerToDie))
            {
                _lastPlayerTouched.Die();
                GameTimer.Clear(_timerToDie);
            }
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            _lastPlayerTouched = p;
            _timerToDie = GameTimer.StartNewTimer(timeToDie);

            //Write to lava sim texture
            //Create Impact Particles
            return Collidable();
        }

        public override bool IsGround(PhysObj b) {
            return false;
        }

        /*public void OnDrawGizmosSelected() {
            
        }*/
    }
}