namespace ASK.Animation
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