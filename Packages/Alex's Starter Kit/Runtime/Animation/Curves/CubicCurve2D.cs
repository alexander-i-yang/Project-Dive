using System.Collections.Generic;
using UnityEngine;

namespace ASK.Animation
{
    public abstract class CubicCurve2D
    {
        protected Vector2 t3Coeff;
        protected Vector2 t2Coeff;
        protected Vector2 tCoeff;
        protected Vector2 bias;

        protected CubicCurve2D(Vector2 t3Coeff, Vector2 t2Coeff, Vector2 tCoeff, Vector2 bias)
        {
            this.t3Coeff = t3Coeff;
            this.t2Coeff = t2Coeff;
            this.tCoeff = tCoeff;
            this.bias = bias;
        }

        public Vector2 Evaluate(float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            return t3 * t3Coeff + t2 * t2Coeff + t * tCoeff + bias;
        }

        public List<Vector2> SamplePoints(int numSamples)
        {
            if (numSamples < 2)
            {
                Debug.LogError("Must sample at least 2 points");
                return new List<Vector2>();
            }

            List<Vector2> positions = new List<Vector2>();
            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / (numSamples - 1);
                positions.Add(Evaluate(t));
            }

            return positions;
        }
    }
}
