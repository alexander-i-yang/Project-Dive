using System.Collections;
using System.Collections.Generic;
using Helpers;
using Player;
using UnityEngine;

public class Dust : MonoBehaviour
{
    [SerializeField] private float emissAirDuration;
    private PlayerSpawnManager _spawnManager;

    private ParticleSystem dust;
    private ParticleSystem.EmissionModule emiss;
    private float emissAmt;

    private GameTimer emissToggleTimer;

    private void Awake()
    {
        dust = GetComponent<ParticleSystem>();
        emiss = dust.emission;
        emissAmt = emiss.rateOverDistance.constant;

        emissToggleTimer = GameTimer.StartNewTimer(emissAirDuration, "Dust Emission Toggle Timer");
        GameTimer.Pause(emissToggleTimer);
    }

    private void OnEnable()
    {
        _spawnManager = GetComponentInParent<PlayerSpawnManager>();
        _spawnManager.OnPlayerRespawn += TurnEmissionOff;
        emissToggleTimer.OnFinished += TurnEmissionOff;
    }

    private void OnDisable()
    {
        PlayerCore.SpawnManager.OnPlayerRespawn -= TurnEmissionOff;
        emissToggleTimer.OnFinished -= TurnEmissionOff;
    }

    void Update()
    {
        if (PlayerCore.Actor.IsGrounded())
        {
            TurnEmissionOn();
            GameTimer.Reset(emissToggleTimer);
            GameTimer.Pause(emissToggleTimer);
        } else
        {
            GameTimer.UnPause(emissToggleTimer);
        }

        GameTimer.Update(emissToggleTimer);

        //if (PlayerCore.Actor.velocityY < 0)
        //{
        //    emiss.rateOverDistanceMultiplier = 0;
        //}
        //else
        //{
        //    emiss.rateOverDistanceMultiplier = emissAmt;
        //}
    }

    private void TurnEmissionOn()
    {
        emiss.rateOverDistanceMultiplier = emissAmt;
    }

    private void TurnEmissionOff()
    {
        emiss.rateOverDistanceMultiplier = 0;
    }
}
