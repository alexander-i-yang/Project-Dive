using Helpers.Animation;

using MyBox;

using UnityEngine;
using UnityEngine.U2D;

//L: This is a replacement for Cinemachine movement because I'm stubborn.
[RequireComponent(typeof(PixelPerfectCamera))]
public class PixelCameraMover : MonoBehaviour
{
    [SerializeField, AutoProperty] PixelPerfectCamera _ppCamera;
    [SerializeField] private bool useUnscaledTime;

    [Foldout("Follow", true)]
    [SerializeField] private Transform followTarget;
    [SerializeField] private float followSpeed;
    [SerializeField] private float initialResponse;

    private SecondOrderDynamics2D secondOrderDynamics;

    private void Awake()
    {
        secondOrderDynamics = new SecondOrderDynamics2D(followSpeed, 3f, initialResponse, followTarget.position);
    }

    private void Update()
    {
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        Vector2 newPos = secondOrderDynamics.Update(dt, followTarget.position);
        transform.position = new Vector3(newPos.x,  newPos.y, transform.position.z);
    }
}
