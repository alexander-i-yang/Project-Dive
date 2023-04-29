using System.Collections.Generic;

using UnityEngine;

namespace ASK.Animation
{
    [System.Serializable]
    //Source: https://www.cubic.org/docs/hermite.htm and CS4496/7496 Notes
    public class HermiteCurve2D : CubicCurve2D
    {
        [SerializeField] private Vector2 startPos;
        [SerializeField] private Vector2 startVel;
        [SerializeField] private Vector2 endPos;
        [SerializeField] private Vector2 endVel;

        //private Matrix4x4 hermite_basis = new Matrix4x4(
        //    new Vector4(2, -3, 0, 1),
        //    new Vector4(-2, 3, 0, 0),
        //    new Vector4(1, -2, 1, 0),
        //    new Vector4(1, -1, 0, 0)
        //);

        public HermiteCurve2D(Vector2 startPos, Vector2 endPos, Vector2 startVel, Vector2 endVel) : 
            base(2 * startPos - 2 * endPos + startVel + endVel,
                -3 * startPos + 3 * endPos - 2 * startVel - 1 * endVel,
                startVel,
                startPos)
        {
            this.startPos = startPos;
            this.endPos = endPos;
            this.startVel = startVel;
            this.endVel = endVel;
        }
    }
}
