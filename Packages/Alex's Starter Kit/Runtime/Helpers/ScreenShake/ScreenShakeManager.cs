using Cinemachine;
using ASK.Helpers;
using UnityEngine;

namespace ASK.ScreenShake
{

    /*public enum NoiseProfiles
    {
        DEATH_NOISE = 
    }*/

    public class ScreenShakeManager : MonoBehaviour
    {
        private Coroutine _shakeRoutine;
        
        public void ScreenshakeBurst(CinemachineVirtualCamera vcam, ScreenShakeDataBurst s)
        {
            var c = vcam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
            c.m_NoiseProfile = s.NoiseProfile;

            if (_shakeRoutine != null)
            {
                StopCoroutine(_shakeRoutine);
            }
            
            _shakeRoutine = StartCoroutine(Helper.DelayAction(s.Time, () =>
            {
                c.m_NoiseProfile = null;
            }));
        }

        public void ScreenShakeContinuousOn(CinemachineVirtualCamera vcam, ScreenShakeDataContinuous s)
        {
            var c = vcam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
            c.m_NoiseProfile = s.NoiseProfile;
        }
        
        public void ScreenShakeContinuousOff(CinemachineVirtualCamera vcam, ScreenShakeDataContinuous s)
        {
            var c = vcam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
            c.m_NoiseProfile = null;
        }
    }
}