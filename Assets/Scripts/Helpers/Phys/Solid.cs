using UnityEngine;

namespace Phys {
    public abstract class Solid : PhysObj {
        public override void Move(Vector2 velocity) {
            throw new System.NotImplementedException();
        }

        public void FixedUpdate() {
            
        }
    }
}