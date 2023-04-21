using System;
using Cinemachine;
using MyBox;
using UnityEngine;
namespace VFX
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