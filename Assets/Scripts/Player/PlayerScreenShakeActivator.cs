using System;
using Cinemachine;
using MyBox;
using VFX;
using World;

namespace Player
{
    public class PlayerScreenShakeActivator : ScreenShakeActivator
    {
        private PlayerSpawnManager _spawnManager;
        
        public ScreenShakeDataBurst DeathData;
        public ScreenShakeDataContinuous DiveData;

        public ScreenShakeDataContinuous CurShake;

        private void Awake()
        {
            _spawnManager = GetComponentInChildren<PlayerSpawnManager>();
        }

        private void OnEnable()
        {
            Room.RoomTransitionEvent += SwitchRooms;
        }
        
        private void OnDisable()
        {
            Room.RoomTransitionEvent -= SwitchRooms;
        }

        public void ScreenShakeBurst(ScreenShakeDataBurst d)
        {
            base.ScreenShakeBurst(_spawnManager.CurrentVCam, d);
        }
        
        public void ScreenShakeContinuousOn(ScreenShakeDataContinuous d)
        {
            base.ScreenShakeContinuousOn(_spawnManager.CurrentVCam, d);
            CurShake = d;
        }
        
        public void ScreenShakeContinuousOff(ScreenShakeDataContinuous d)
        {
            base.ScreenShakeContinuousOff(_spawnManager.CurrentVCam, d);
            CurShake = null;
        }

        public void SwitchRooms(Room r)
        {
            if (CurShake != null)
            {
                if (_spawnManager.CurrentRoom != null)
                {
                    print(_spawnManager);
                    print(_spawnManager.CurrentRoom);
                    print(_spawnManager.CurrentRoom.VCam);
                    base.ScreenShakeContinuousOff(
                        _spawnManager.CurrentRoom.VCam,
                        CurShake
                    );
                }
                
                base.ScreenShakeContinuousOn(
                    r.VCam,
                    CurShake
                );
            }
        }
    }
}