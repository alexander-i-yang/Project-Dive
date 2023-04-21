using Helpers;
using MyBox;
using Phys;
using UnityEngine;

namespace Mechanics {
    public class Lava : Solid {
        [SerializeField] private float timeToDie = 0.1f;

        private GameTimer _timerToDie;
        private PlayerActor _lastPlayerTouched;

        [SerializeField] private float recoilMult = -0.3f;
        private Vector2 _entryV;

        public override bool Collidable() {
            //return true;
            return false;
        }

        private void Update()
        {
            GameTimer.Update(_timerToDie);
            if (_lastPlayerTouched != null && GameTimer.TimerFinished(_timerToDie))
            {
                _lastPlayerTouched.Die(_ => recoilMult*_entryV);
                GameTimer.Clear(_timerToDie);
            }
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            _lastPlayerTouched = p;
            _entryV = p.velocity;
            _timerToDie = GameTimer.StartNewTimer(timeToDie);
            //Offset diePos to ensure particles break out of the lava 

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