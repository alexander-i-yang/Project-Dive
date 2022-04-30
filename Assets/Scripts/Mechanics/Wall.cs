using Phys;

namespace Mechanics {
    public class Wall : Solid {
        public override bool OnCollide(PhysObj p) {
            return false;
        }

        public override bool PlayerCollide(PlayerController p) {
            return true;
        }
    }
}