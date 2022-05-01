using Phys;
using UnityEngine;

namespace Mechanics {
    public class MovingBlock : Solid {
        private Vector2 _startX;
        void Start() {
            _startX = (int) transform.position.y;
            base.Start();
        }

        public override bool OnCollide(PhysObj p, Vector2 direction) {
            return true;
        }

        public override bool PlayerCollide(PlayerController p, Vector2 direction) {
            return true;
        }

        void FixedUpdate() {
            if (transform.position.y >= _startX + 16) {
                velocityY = -vx;
            } else if (transform.position.y <= _startX) {
                velocityY = vx;
            }
            base.FixedUpdate();
        }
    }
}