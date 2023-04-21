using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class ParallaxV2Helper : MonoBehaviour
{
    public GameObject target;
    public float PixelsPerUnit;
 
    void Update()
    {
        transform.position = PixelPerfectClamp(target.transform.position, PixelsPerUnit);
    }
 
    private Vector3 PixelPerfectClamp(Vector3 moveVector, float pixelsPerUnit)
    {
        Vector3 vectorInPixels = new Vector3(Mathf.CeilToInt(moveVector.x * pixelsPerUnit), Mathf.CeilToInt(moveVector.y * pixelsPerUnit), Mathf.CeilToInt(moveVector.z * pixelsPerUnit));                                         
        return vectorInPixels / pixelsPerUnit;
    }
}