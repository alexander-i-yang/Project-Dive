using Core;
using Helpers;

using System;

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float percentOn = 0.5f;
    [SerializeField] private float updateFrequency = 1f;
    [SerializeField] private AnimationCurve intensityCurve;
    [SerializeField] private float flashSpeed = 1f;
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 2f;

    private bool _lightOn;
    private Light2D _light;
    private GameTimer _updateTimer;

    private float _flashProgress;

    private float TargetIntensity => _lightOn ? maxIntensity : minIntensity;
    private float LastTargetIntensity => _lightOn ? minIntensity : maxIntensity;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
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
        _light.intensity = Mathf.MoveTowards(_light.intensity, TargetIntensity, flashSpeed * Time.deltaTime);
        //float t = Mathf.InverseLerp(LastTargetIntensity, TargetIntensity, _light.intensity);
        //t = Mathf.Clamp01(t + Game.Instance.DeltaTime * flashSpeed);
        //float s = intensityCurve.Evaluate(t);
        //_light.intensity = Mathf.Lerp(LastTargetIntensity, TargetIntensity, s);
    }
}
