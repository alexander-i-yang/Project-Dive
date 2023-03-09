using System.Collections;
using System.Collections.Generic;
using Audio;
using Player;
using UnityEngine;

using FMODUnity;
using Helpers;

public class DrillingSFX : MonoBehaviour
{
    public PlayerStateMachine SM => PlayerCore.StateMachine;

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SM.StateTransition += OnStateTransition;
    }

    void OnStateTransition()
    {
        //if (SM.UsingDrill)
        //{
        //    AudioManager.Play("Drilling");
        //}
        //else
        //{
        //    AudioManager.StopSound("Drilling");
        //}
    }
}
