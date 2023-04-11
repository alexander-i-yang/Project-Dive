using Cinemachine;
using Helpers;
using UnityEngine;

namespace VFX
{

    /*public enum NoiseProfiles
    {
        DEATH_NOISE = 
    }*/

    public class ScreenShakeManager : MonoBehaviour
    {
        public NoiseSettings myNoiseProfile;

        public void Screenshake(CinemachineVirtualCamera vcam, float time, float strength)
        {
            var c = vcam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
            c.m_NoiseProfile = myNoiseProfile;


            /*Helper.DelayAction(() =>
            {
                
            });*/
        }
    }
}