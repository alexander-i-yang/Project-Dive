using System;
using Cinemachine;
using UnityEngine;
namespace VFX
{
    // [CreateAssetMenu(fileName = "ScreenShakeData", menuName = "ScriptableObjects/ScreenShakeData", order = 1)]
    [Serializable]
    public class ScreenShakeData
    {
        public NoiseSettings NoiseProfile;
        public float Time;
    }
}