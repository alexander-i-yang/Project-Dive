using UnityEngine;
using Player;

namespace Mechanics
{
    public class PlayerSurfaceResponse : MonoBehaviour, IHardSurfaceResponse, IBouncySurfaceResponse
    {
        [SerializeField] private int hardSurfaceBounceHeight;
        [SerializeField] private int bouncySurfaceNeutralBounceHeight;
        [SerializeField] private int bouncySurfaceDiveBounceHeight;

        private PlayerStateMachine PlayerSM => PlayerCore.StateMachine;

        public void OnHardSurfaceCollide(HardSurface hardSurface)
        {
            if (PlayerSM.UsingDrill)
            {
                PlayerCore.Actor.Jump(hardSurfaceBounceHeight);
                PlayerSM.RefreshAbilities();
                PlayerSM.Transition<PlayerStateMachine.Airborne>();
            }
        }

        public void OnBouncySurfaceCollide(BouncySurface bouncySurface)
        {
            if (PlayerCore.StateMachine.UsingDrill)
            {
                PlayerCore.Actor.Jump(bouncySurfaceDiveBounceHeight);
            } else
            {
                PlayerCore.Actor.Jump(bouncySurfaceNeutralBounceHeight);
            }
            PlayerSM.RefreshAbilities();
            PlayerSM.Transition<PlayerStateMachine.Airborne>();
        }
    }
}
