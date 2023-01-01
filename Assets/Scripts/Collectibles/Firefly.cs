using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectibles {
    public class Firefly : Collectible
    {
        public override string ID => "Firefly";

        public override void OnCollected(ICollector collector)
        {
            Destroy(gameObject);
        }
    }
}