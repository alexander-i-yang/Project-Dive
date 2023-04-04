using UnityEngine;

namespace Bakers
{
    public class MegaBaker : MonoBehaviour
    {
        public void BakeAll()
        {
            foreach (var baker in GetComponents<IBaker>())
            {
                baker.Bake();
            }
        }
    }
}