using System;

using UnityEngine;

namespace ASK.Helpers.Animation
{
    //Reference: https://www.youtube.com/watch?v=KPoeNZZ6H4s
    //This class maps inputs to an output using second order dynamics
    [Serializable]
    public class SecondOrderDynamics2D
    {
        [SerializeField, Range(0f, 10f)] float frequency;
        [SerializeField, Range(0f, 2f)] float damping;
        [SerializeField, Range(-5f, 5f)] float response;

        private SecondOrderDynamics2DImpl strategy;

        public SecondOrderDynamics2D(float frequency, float damping, float response, Vector2 inputCurr)
        {
            this.frequency = frequency;
            this.damping = damping;
            this.response = response;
            Init(inputCurr);
        }

        public void Init(Vector2 inputCurr)
        {
            strategy = new SecondOrderDynamics2DImplEuler(frequency, damping, response, inputCurr);
        }

        public Vector2 Update(float deltaTime, Vector2 input, Vector2? dInput = null)
        {
            return strategy.UpdateOutput(deltaTime, input, dInput);
        }
    }
}
