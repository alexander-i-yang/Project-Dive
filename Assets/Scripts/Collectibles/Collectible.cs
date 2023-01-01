using UnityEngine;

namespace Collectibles
{
    public abstract class Collectible : MonoBehaviour
    {
        public abstract string ID { get; }
        public abstract void OnCollected(ICollector collector);
    }
}
