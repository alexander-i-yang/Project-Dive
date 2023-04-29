using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ASK.Helpers
{
    /// <summary>
    /// This class toggles a light2d component off and on.
    /// This occurs during Awake().
    /// In Dive, it was used on the Player Spotlight to fix a strange lighting bug.
    /// </summary>
    [RequireComponent(typeof(Light2D))]
    public class AwakeLightToggler : MonoBehaviour
    {
        [SerializeField] private float delayTime;
        private void Awake()
        {
            Light2D l = GetComponent<Light2D>();
            bool e = l.enabled;
            l.enabled = !e;
            StartCoroutine(
                Helper.DelayAction(delayTime, () => l.enabled = e)
            );
        }
    }
}