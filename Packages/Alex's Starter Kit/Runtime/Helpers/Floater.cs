using ASK.Core;
using UnityEngine;

namespace ASK.Helpers
{
    public class Floater : MonoBehaviour {
        //[SerializeField] private float degreesPerSecond = 15.0f;
        [SerializeField] private float amplitude = 0.5f;
        [SerializeField] private float frequency = 1f;

        void Update () {
            // Spin object around Y-Axis
            // transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
 
            // Float up/down with a Sin()
            Vector3 tempPos = transform.parent.position;
            tempPos.y += Mathf.Sin (
                Game.Instance.Time * Mathf.PI * frequency +
                tempPos.x * 0.1f + tempPos.y * 0.1f
            ) * amplitude;
 
            transform.position = tempPos;
        }
    }
}