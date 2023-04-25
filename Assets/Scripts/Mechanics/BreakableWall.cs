using Phys;
using UnityEngine;

namespace Mechanics
{
    public class BreakableWall : Breakable
    {
        protected override string ParticlePath() => "PS_BreakableWall";
        
        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            if (direction.y > 0) {
                if (p.IsBeegBouncing)
                {
                    Break();
                    return false;
                }
                else
                {
                    p.BonkHead();
                }
            }
            return true;
        }

        public override bool IsGround(PhysObj whosAsking)
        {
            return true;
        }

        public override bool CanReset()
        {
            return false;
        }
    }
}