using Cinemachine;
using ASK.Core;
using UnityEngine;

namespace ASK.ScreenShake
{
    public abstract class ScreenShakeActivator : MonoBehaviour
    {
        public void ScreenShakeBurst(CinemachineVirtualCamera vcam, ScreenShakeDataBurst d)
        {
            Game.Instance.CamController.ScreenShakeMan.ScreenshakeBurst(vcam, d);
        }
        
        public void ScreenShakeContinuousOn(CinemachineVirtualCamera vcam, ScreenShakeDataContinuous d)
        {
            Game.Instance.CamController.ScreenShakeMan.ScreenShakeContinuousOn(vcam, d);
        }
        
        public void ScreenShakeContinuousOff(CinemachineVirtualCamera vcam, ScreenShakeDataContinuous d)
        {
            Game.Instance.CamController.ScreenShakeMan.ScreenShakeContinuousOff(vcam, d);
        }
    }
}