using System;
using Helpers;
using UnityEngine;

public class CameraFollow : MonoBehaviour

{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 locationOffset;

    private Vector2 _subpixels;
    private Vector3 velocity = Vector3.zero;
    
    private Vector3 MaxDist = Vector3.one * 8;
    private void Start()
    {
        transform.position = target.position;
    }

    void Update()
    {
        float step = smoothSpeed * Time.deltaTime;
        //If Damping is "on", the Camera will Damp depending on the Smooth value
        if(true){
            transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothSpeed);
            if (Math.Abs(transform.position.x - target.position.x) > MaxDist.x)
            {
                float newX = target.position.x - MaxDist.x;
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            }
            return;
        }
        //transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        BoxDrawer.DrawBox(transform.position, new Vector3(1, 1, 0), Quaternion.identity, Color.red);
    }
#endif
}