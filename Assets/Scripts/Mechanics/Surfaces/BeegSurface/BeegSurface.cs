using Phys;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics
{
    public class BeegSurface : BouncySurface
    {
        [SerializeField] private UnityEvent onBounce;
        [SerializeField] private UnityEvent onDive;
        
        public override bool Collidable()
        {
            return false;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction)
        {
            IBeegSurfaceResponse response = p.GetComponent<IBeegSurfaceResponse>();
            response.OnBeegSurfaceCollide(this, onBounce, onDive);
            return true;
        }
    }
}
