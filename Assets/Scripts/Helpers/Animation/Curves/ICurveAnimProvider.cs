using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Helpers.Animation
{
    public enum CurveRelativeTo
    {
        World,
        Viewport,
        Screen
    }

    public interface ICurveAnimProvider
    {

        public CubicCurve2D GetCurve();
        public float GetAnimSpeed();
        public CurveRelativeTo GetRelativeTo();
    }
}