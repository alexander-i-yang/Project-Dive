using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float parallaxFactor;
    [SerializeField] private float PixelsPerUnit;

    private float _length;
    private Camera _camera;
    private Transform _cam => _camera.transform;

    private void Awake()
    {
        _camera = GetComponentInParent<Camera>();
    }
 
    void Update() {
        // float temp = cam.transform.position.x * (1 - parallaxFactor);
        float distance = _cam.position.x * parallaxFactor;
 
        Vector3 newPosition = new Vector3(-1 * distance, transform.position.y, transform.position.z);
 
        transform.position = PixelPerfectClamp(newPosition, PixelsPerUnit);
 
        // if (temp > startpos + (length / 2))      startpos += length;
        // else if (temp < startpos - (length / 2)) startpos -= length;
    }
 
    private Vector3 PixelPerfectClamp(Vector3 locationVector, float pixelsPerUnit)
    {
        Vector3 vectorInPixels = new Vector3(Mathf.CeilToInt(locationVector.x * pixelsPerUnit), Mathf.CeilToInt(locationVector.y * pixelsPerUnit),Mathf.CeilToInt(locationVector.z * pixelsPerUnit));
        return vectorInPixels / pixelsPerUnit;
    }
      
}