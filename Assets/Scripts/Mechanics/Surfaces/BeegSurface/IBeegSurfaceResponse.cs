using UnityEngine.Events;

namespace Mechanics
{
    public interface IBeegSurfaceResponse
    {
        public void OnBeegSurfaceCollide(BouncySurface bouncySurface, UnityEvent onBounce, UnityEvent onDive);
    }
}

