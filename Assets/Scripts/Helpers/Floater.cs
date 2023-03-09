using Core;
using UnityEngine;

namespace Helpers
{
    public class Floater : MonoBehaviour {
        //[SerializeField] private float degreesPerSecond = 15.0f;
        [SerializeField] private float amplitude = 0.5f;
        [SerializeField] private float frequency = 1f;
 
        // Position Storage Variables
        Vector3 posOffset = new Vector3 ();
        Vector3 tempPos = new Vector3 ();
 
        void Awake () {
            // Store the starting position & rotation of the object
            posOffset = transform.localPosition;
        }
     
        void Update () {
            // Spin object around Y-Axis
            // transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
 
            // Float up/down with a Sin()
            tempPos = posOffset;
            tempPos.y += Mathf.Sin (Game.Instance.Time * Mathf.PI * frequency+transform.position.x*0.1f) * amplitude;
 
            transform.localPosition = tempPos;
        }
    }
}