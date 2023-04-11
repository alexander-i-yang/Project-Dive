using System.Collections;
using UnityEngine;

public class PlayerInWaterSFX : MonoBehaviour
{
    [SerializeField] private float dampSpeed;
    private PlayerWaterInteraction _waterCheck;

    private float inWater = 0f;

    private void Awake()
    {
        _waterCheck = GetComponent<PlayerWaterInteraction>();

        inWater = 0f;
    }

    private void Update()
    {
        float dampDir = _waterCheck.InWater ? 1f : -1f;
        inWater = Mathf.Clamp01(inWater + dampDir * dampSpeed);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("InWater", inWater);

        //Debug.Log($"New In Water: {inWater}");
    }

    public void WaterEnter()
    {
    }

    public void WaterExit()
    {

    }

    public void WaterStay()
    {

    }
}
