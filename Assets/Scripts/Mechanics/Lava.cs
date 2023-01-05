using Helpers;
using MyBox;
using Phys;
using UnityEngine;

namespace Mechanics {
    public class Lava : Solid {
        public override bool Collidable() {
            return true;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            p.Die();

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