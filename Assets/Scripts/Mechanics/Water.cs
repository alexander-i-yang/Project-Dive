using Phys;
using UnityEngine;

namespace Mechanics
{
    public class Water : Solid
    {
        private bool _playerOverlapping;
        public override bool Collidable()
        {
            return true;
        }

        public override void FixedUpdate()
        {
            if (_playerOverlapping)
            {
                
            }
        }

        public override bool IsGround(PhysObj whosAsking)
        {
            PlayerActor p = whosAsking as PlayerActor;
            if (p != null)
            {
                if (p.IsOverlapping(this)) return false;
                return !p.IsDiving();
            }

            return true;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction)
        {
            bool overlapping = p.IsOverlapping(this);
            if (overlapping || (p.IsDiving() && direction.y < 0))
            {
                p.FlipGravity();
                return false;
            }
            return true;
        }
    }
}