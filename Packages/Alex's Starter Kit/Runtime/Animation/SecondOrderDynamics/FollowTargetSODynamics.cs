using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASK.Helpers.Animation
{
    public class FollowTargetSODynamics : MonoBehaviour
    {
        [SerializeField] private SecondOrderDynamics2D dynamics;
        [SerializeField] private Transform target;

        // Start is called before the first frame update
        private void Start()
        {
            dynamics.Init(target.position);
        }

        // Update is called once per frame
        private void Update()
        {
            transform.position = dynamics.Update(Time.deltaTime, target.position);
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}


