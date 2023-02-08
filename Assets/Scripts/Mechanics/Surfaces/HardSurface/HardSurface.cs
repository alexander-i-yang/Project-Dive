using Phys;
using UnityEngine;

namespace Mechanics
{
    public class HardSurface : Solid
    {
        public override bool Collidable()
        {
            return true;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction)
        {
            IHardSurfaceResponse response = p.GetComponent<IHardSurfaceResponse>();
            response.OnHardSurfaceCollide(this);
            return true;
        }
    }
}
