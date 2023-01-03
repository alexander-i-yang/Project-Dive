using UnityEngine;

namespace Collectibles
{
    public abstract class Collector : MonoBehaviour
    {
        public virtual void OnTouched(Collectible collectible) { }
        public virtual void OnCollectFinished(Collectible collectible) { }
    }
}
