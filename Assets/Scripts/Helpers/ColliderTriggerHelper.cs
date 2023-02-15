using System;
using UnityEngine;
using UnityEngine.Events;

namespace Helpers
{
    [RequireComponent(typeof(Collider2D))]
    public class ColliderTriggerHelper : MonoBehaviour
    {
        public UnityEvent Enter;
        public UnityEvent Exit;

        private void OnTriggerEnter(Collider other)
        {
            Enter.Invoke();
        }
        
        private void OnTriggerExit(Collider other)
        {
            Exit.Invoke();
        }
    }
}