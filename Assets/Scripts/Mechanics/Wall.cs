using Phys;
using UnityEngine;
using World;

namespace Mechanics {
    public class Wall : Solid {
        public override bool Collidable() {
            return true;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            if (direction.y > 0) {
                if (EndCutsceneManager.IsEndCutscene)
                {
                    Destroy(gameObject);
                    return false;
                }
                p.BonkHead();
            }
            return true;
        }
    }
}