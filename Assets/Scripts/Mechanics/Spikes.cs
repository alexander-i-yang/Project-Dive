using Phys;
using UnityEngine;

namespace Mechanics {
    public class Spikes : Solid {
        public override bool OnCollide(PhysObj p, Vector2 direction) {
            return false;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            p.Die();
            return false;
        }

        public override bool IsGround(PhysObj b) {
            return false;
        }
    }
}