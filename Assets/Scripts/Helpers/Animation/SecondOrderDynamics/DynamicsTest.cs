using Animation;

using MyBox;

using UnityEngine;

public class DynamicsTest : MonoBehaviour
{
    [SerializeField] SecondOrderDynamics2D dynamics;
    [SerializeField, MustBeAssigned] Transform target;

    // Start is called before the first frame update
    void Start()
    {
        dynamics.Init(target.position);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = dynamics.Update(Time.deltaTime, target.position);
    }
}
