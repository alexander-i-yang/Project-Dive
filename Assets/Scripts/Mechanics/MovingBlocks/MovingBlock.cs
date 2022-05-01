using DefaultNamespace;
using Phys;
using UnityEngine;

namespace Mechanics {
    public class MovingBlock : Solid {
        private Vector2 _startPos;
        [SerializeField] private Vector2 _direction = Vector2.right;
        [SerializeField] private int decel = -10;

        new void Start() {
            _startPos = transform.position;
            base.Start();
        }

        public override bool OnCollide(PhysObj p, Vector2 direction) {
            if (p.GetComponent<Wall>() != null) {
                velocity = Vector2.zero;
                return true;
            }
            return false;
        }

        public override bool PlayerCollide(PlayerController p, Vector2 direction) {
            if (direction.y < 0 && p.IsDiving()) {
                Zoom();
            }
            return true;
        }

        private void Zoom() {
            velocity = _direction;
            Debug.Log("Zoom");
        }

        public new void FixedUpdate() {
            if (velocity != Vector2.zero) {
                velocity += _direction * (decel * Game.FixedDeltaTime);
            }
            base.FixedUpdate();
        }
    }
}