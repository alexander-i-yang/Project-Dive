using Cinemachine;
using Core;
using UnityEngine;

namespace VFX
{
    public abstract class ScreenShakeActivator : MonoBehaviour
    {
        public void ScreenShakeBurst(CinemachineVirtualCamera vcam, ScreenShakeDataBurst d)
        {
            Game.Instance.ScreenShakeManagerInstance.ScreenshakeBurst(vcam, d);
        }
        
        public void ScreenShakeContinuousOn(CinemachineVirtualCamera vcam, ScreenShakeDataContinuous d)
        {
            Game.Instance.ScreenShakeManagerInstance.ScreenShakeContinuousOn(vcam, d);
        }
        
        public void ScreenShakeContinuousOff(CinemachineVirtualCamera vcam, ScreenShakeDataContinuous d)
        {
            Game.Instance.ScreenShakeManagerInstance.ScreenShakeContinuousOff(vcam, d);
        }
    }
}