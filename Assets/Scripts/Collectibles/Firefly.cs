using UnityEngine;
using MyBox;

using Helpers.Animation;

namespace Collectibles {
    public class Firefly : Collectible
    {
        public override string ID => "Firefly";

        public override void OnCollected(ICollector collector)
        {
            MonoBehaviour collectorMB = collector as MonoBehaviour;
            EnableFollowBehaviour(collectorMB.transform);
            Destroy(gameObject);
        }

        private void EnableFollowBehaviour(Transform target)
        {
            //TODO: Do this
        }
    }
}