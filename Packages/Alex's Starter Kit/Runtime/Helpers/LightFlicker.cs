using ASK.Core;
using MyBox;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ASK.Helpers
{
    public class LightFlicker : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float percentOn = 0.5f;
    [SerializeField] private float updateFrequency = 1f;
    [SerializeField] private AnimationCurve intensityCurve;
    [SerializeField] private float flashSpeed = 1f;
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 2f;
    
    [SerializeField]
    [ConstantsSelection(typeof(LightFlickerTypes))]
    private int flickerType = LightFlickerTypes.SINGLE;

    private bool _lightOn;
    private Light2D[] _lights;
    private GameTimer _updateTimer;

    private float _flashProgress;

    private float TargetIntensity => _lightOn ? maxIntensity : minIntensity;
    // private float LastTargetIntensity => _lightOn ? minIntensity : maxIntensity;

    private void Awake()
    {
        if (flickerType == LightFlickerTypes.SINGLE)
        {
            _lights = new []{GetComponent<Light2D>()};
        } else if (flickerType == LightFlickerTypes.COMPOSITE)
        {
            _lights = GetComponentsInChildren<Light2D>();
        }

        _updateTimer = GameTimer.StartNewTimer(updateFrequency);

        _lightOn = false;
    }

    private void Update()
    {
        //Randomly change the intensity to on or off.
        if (GameTimer.TimerFinished(_updateTimer))
        {
            _lightOn = UnityEngine.Random.value < percentOn;
            GameTimer.Reset(_updateTimer);
        }
        GameTimer.Update(_updateTimer);

        //Sample animation curve and update light intensity
        float curIntensity = _lights[0].intensity;
        float newIntensity = Mathf.MoveTowards(curIntensity, TargetIntensity, flashSpeed * Time.deltaTime);
        foreach (var l in _lights)
        {
            l.intensity = newIntensity;
        }
        //float t = Mathf.InverseLerp(LastTargetIntensity, TargetIntensity, _light.intensity);
        //t = Mathf.Clamp01(t + Game.Instance.DeltaTime * flashSpeed);
        //float s = intensityCurve.Evaluate(t);
        //_light.intensity = Mathf.Lerp(LastTargetIntensity, TargetIntensity, s);
    }

    private class LightFlickerTypes
    {
        public const int SINGLE = 0;
        public const int COMPOSITE = 1;
    }
}

}