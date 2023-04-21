using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FMODUnity;
using Player;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter diveStartSFX;
    [SerializeField] private StudioEventEmitter diveLoopSFX;

    public void DiveStart()
    {
        diveStartSFX.Play();
        diveLoopSFX.Play();
    }

    public void OnPlayerStateChange(PlayerStateMachine sm)
    {
        if (!sm.IsOnState<PlayerStateMachine.Diving>())
        {
            if (diveLoopSFX.IsPlaying())
            {
                diveLoopSFX.Stop();
            }
        }
    }
}
