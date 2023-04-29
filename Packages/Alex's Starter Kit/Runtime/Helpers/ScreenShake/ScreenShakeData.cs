using System;
using Cinemachine;
using MyBox;
using UnityEngine;
namespace ASK.ScreenShake
{
    public abstract class ScreenShakeData
    {
        public NoiseSettings NoiseProfile;
    }
    
    [Serializable]
    public class ScreenShakeDataBurst : ScreenShakeData
    {
        public float Time;
    }
    
    [Serializable]
    public class ScreenShakeDataContinuous : ScreenShakeData
    {
    }
}