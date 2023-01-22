using Helpers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float percentOn = 0.5f;
    [SerializeField] private float updateFrequency = 1f;
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 2f;

    private Light2D _light;
    private GameTimer _updateTimer;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _updateTimer = GameTimer.StartNewTimer(updateFrequency);
    }

    private void Update()
    {
        if (GameTimer.TimerFinished(_updateTimer))
        {
            _light.intensity = Random.value < percentOn ? maxIntensity : minIntensity;
            GameTimer.Reset(_updateTimer);
        }


        GameTimer.Update(_updateTimer);
    }
}
