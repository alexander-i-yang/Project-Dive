using System;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    [Serializable]
    public class PlayerEvent : UnityEvent<Vector3>
    {
        [SerializeField]
        PlayerEvent pEvent;

        public void OnEventRaised(Vector3 pos)
        {
            pEvent.Invoke(pos);
        }
    }
}