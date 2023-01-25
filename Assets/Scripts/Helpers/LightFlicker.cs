using Helpers;

using System;

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float flashChancePerUpdate = 0.5f;
    [SerializeField] private float updateFrequency = 1f;
    [SerializeField] private AnimationCurve flashCurve;
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 2f;

    private float _targetIntensity;
    private Light2D _light;
    private GameTimer _updateTimer;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _updateTimer = GameTimer.StartNewTimer(updateFrequency);

        _targetIntensity = minIntensity;
    }

    private void Update()
    {
        if (GameTimer.TimerFinished(_updateTimer))
        {
            if (UnityEngine.Random.value < flashChancePerUpdate)
            {
                _targetIntensity = _targetIntensity == minIntensity ? maxIntensity : minIntensity;
            }
            GameTimer.Reset(_updateTimer);
        }


        GameTimer.Update(_updateTimer);
    }
}
