using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FMODUnity;
using Player;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter diveStartSFX;
    [SerializeField] private StudioEventEmitter diveLoopSFX;
    [SerializeField] private StudioEventEmitter dogoLoopSFX;

    public void DiveStart()
    {
        diveStartSFX.Play();
        diveLoopSFX.Play();
    }

    public void OnPlayerStateChange(PlayerStateMachine sm)
    {
        if (!sm.IsOnState<PlayerStateMachine.Dogoing>())
        {
            if (diveLoopSFX.IsPlaying())
            {
                diveLoopSFX.Stop();
            }
        }
        /*if (sm.IsOnState<PlayerStateMachine.Dogoing>())
        {
            dogoLoopSFX.Play();
        }
        else
        {
            if (dogoLoopSFX.IsPlaying()) dogoLoopSFX.Stop();
        }*/
        
    }
}
