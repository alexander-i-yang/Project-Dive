using Phys;
using UnityEngine;

namespace Mechanics {
    public class Spikes : Solid {
        public override bool OnCollide(PhysObj p) {
            return false;
        }

        public override bool PlayerCollide(PlayerController p) {
            p.Die();
            return false;
        }

        public override bool IsGround(PhysObj b) {
            return false;
        }
    }
}