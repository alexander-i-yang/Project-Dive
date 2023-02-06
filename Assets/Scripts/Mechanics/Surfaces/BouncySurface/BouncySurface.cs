using Phys;
using UnityEngine;

namespace Mechanics
{
    public class BouncySurface : Solid
    {
        public override bool Collidable()
        {
            return false;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction)
        {
            IBouncySurfaceResponse response = p.GetComponent<IBouncySurfaceResponse>();
            response.OnBouncySurfaceCollide(this);
            return true;
        }
    }
}
