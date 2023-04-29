using UnityEngine;

namespace ASK.Helpers.Animation
{
    public class SecondOrderDynamics2DImplEuler : SecondOrderDynamics2DImpl
    {
        //y + k1 * ydot + k2 * ydot2 = x + k3 * xdot
        private Vector2 inputPrev;
        private Vector2 output, dOutput;
        private float k1, k2, k3;

        public SecondOrderDynamics2DImplEuler(float frequency, float damping, float response, Vector2 inputCurr) : base(frequency, damping, response, inputCurr)
        {
        }

        public override void Init(float frequency, float damping, float response, Vector2 inputCurr)
        {
            SetParameters(frequency, damping, response);   

            inputPrev = inputCurr;
            output = inputCurr;
            dOutput = Vector3.zero;
        }

        public override void SetParameters(float frequency, float damping, float response)
        {
            float frequencyRad = (2 * Mathf.PI * frequency);
            k1 = 2 * damping / frequencyRad;
            k2 = 1 / (frequencyRad * frequencyRad);
            k3 = response * damping / frequencyRad;
        }

        public override Vector2 UpdateOutput(float deltaTime, Vector2 input, Vector2? dInputOpt = null)
        {
            Vector2 dInput = ApproxDInput(deltaTime, input, dInputOpt);

            float tCrit = GetCriticalTimestep();
            int iterations = Mathf.CeilToInt(deltaTime / tCrit);
            float timestepDivision = deltaTime / iterations;
            for (int i = 0; i < iterations; i++)
            {
                //Update using implicit Euler method
                UpdateTimestep(timestepDivision, input, dInput);
            }

            return output;
        }

        private Vector2 ApproxDInput(float deltaTime, Vector2 input, Vector2? dInputOpt)
        {
            Vector2 dInput;
            if (dInputOpt == null)
            {
                //Estimate input velocity
                dInput = deltaTime == 0f ? Vector2.zero : (input - inputPrev) / deltaTime;
                inputPrev = input;
            }
            else
            {
                dInput = dInputOpt.Value;
            }
            return dInput;
        }

        private void UpdateTimestep(float timestep, Vector2 input, Vector2 dInput)
        {
            output = output + timestep * dOutput;
            dOutput = dOutput + timestep * (input + k3 * dInput - output - k1 * dOutput) / k2;
        }

        private float GetCriticalTimestep()
        {
            return Mathf.Sqrt(4 * k2 + k1 * k1) - k1;
        }

        private void LogParams(float frequency, float damping, float response)
        {
            Debug.Log($"frequency: {frequency}");
            Debug.Log($"Damping: {damping}");
            Debug.Log($"Response: {response}");
            Debug.Log($"k1: {k1}");
            Debug.Log($"k2: {k2}");
            Debug.Log($"k3: {k3}");
        }

        private void LogState()
        {
            Debug.Log($"Output: {output}");
            Debug.Log($"dOutput: {dOutput}");
        }
    }
}