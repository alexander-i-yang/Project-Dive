using UnityEngine;

namespace Collectibles
{
    public abstract class Collectible : MonoBehaviour
    {
        public abstract string ID { get; }

        public virtual void OnTouched(Collector collector) { }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Collector collector = other.GetComponent<Collector>();
            if (collector != null)
            {
                OnTouched(collector);
                collector.OnTouched(this);
            }
        }
    }
}
