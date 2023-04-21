using System;
using UnityEngine;

namespace Misc
{
    [Serializable]
    public class FakeControl
    {
        public bool Disabled;
        public bool Value;
        private bool prevValue;
        public void Update()
        {
            prevValue = Value;
        }

        public bool WasPressedThisFrame()
        {
            return Value && !prevValue;
        }
        
        public bool WasReleasedThisFrame()
        {
            return !Value && prevValue;
        }
    }
}