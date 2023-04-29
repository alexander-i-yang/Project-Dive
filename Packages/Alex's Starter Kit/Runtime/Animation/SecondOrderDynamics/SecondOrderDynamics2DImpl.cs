using UnityEngine;

namespace ASK.Helpers.Animation
{
    public abstract class SecondOrderDynamics2DImpl
    {
        public SecondOrderDynamics2DImpl(float frequency, float damping, float response, Vector2 inputCurr) {
            Init(frequency, damping, response, inputCurr);
        }

        public abstract void Init(float frequency, float damping, float response, Vector2 inputCurr);
        public abstract void SetParameters(float frequency, float damping, float response);
        public abstract Vector2 UpdateOutput(float deltaTime, Vector2 input, Vector2? dInputOpt = null);
    }
}