using Phys;
using UnityEngine;

namespace Mechanics {
    public class Wall : Solid {
        public override bool OnCollide(PhysObj p, Vector2 direction) {
            return false;
        }
        
        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            if (direction.y > 0) {
                p.BonkHead();
            }
            return true;
        }
    }
}