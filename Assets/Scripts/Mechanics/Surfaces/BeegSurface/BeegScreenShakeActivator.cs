using System;
using Cinemachine;
using Helpers;
using MyBox;
using UnityEngine;
using VFX;
using World;

namespace Player
{
    public class BeegScreenShakeActivator : ScreenShakeActivator
    {
        [SerializeField] private ScreenShakeDataBurst _bounceData;
        [SerializeField] private ScreenShakeDataBurst _diveData;
        
        [SerializeField] private ScreenShakeDataBurst _finalData;
        [SerializeField] private float _finalShakeDelay;

        public void ScreenShakeBurst(ScreenShakeDataBurst d)
        {
            base.ScreenShakeBurst(PlayerCore.SpawnManager.CurrentVCam, d);
        }

        public void BounceShake() => ScreenShakeBurst(_bounceData);
        
        public void DiveShake() => ScreenShakeBurst(_diveData);

        public void FinalShake()
        {
            StartCoroutine(Helper.DelayAction(_finalShakeDelay, () => ScreenShakeBurst(_finalData)));
        }
    }
}