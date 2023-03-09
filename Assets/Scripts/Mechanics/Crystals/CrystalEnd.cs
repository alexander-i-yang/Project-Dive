using UnityEngine;

namespace Mechanics
{
    public class CrystalEnd : MonoBehaviour
    {
        public void OnBounceAnimationEnd()
        {
            transform.parent.GetComponent<Crystal>().OnBounceAnimationEnd();
        }
    }
}