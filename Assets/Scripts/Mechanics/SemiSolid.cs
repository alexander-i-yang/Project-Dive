using Phys;
using UnityEngine;

namespace Mechanics
{
    public class SemiSolid : Solid
    {
        public override bool Collidable()
        {
            return false;
        }

        public override bool IsGround(PhysObj p)
        {
            PlayerActor pa = p as PlayerActor;
            if (pa != null && pa.IsDiving()) return false;
            
            bool pAboveMe = p.ColliderBottomY() >= ColliderTopY();
            return p.velocityY <= 0 && pAboveMe;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction)
        {
            if (p.IsDiving())
            {
                return false;
            }

            if (direction.y >= 0) return false;
            return IsGround(p);
        }
    }
}