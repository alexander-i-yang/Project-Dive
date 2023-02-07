using UnityEngine;
using Player;

namespace Mechanics
{
    public class PlayerSurfaceResponse : MonoBehaviour, IHardSurfaceResponse, IBouncySurfaceResponse
    {
        [SerializeField] private int hardSurfaceBounceHeight;
        [SerializeField] private int bouncySurfaceNeutralBounceHeight;
        [SerializeField] private int bouncySurfaceDiveBounceHeight;

        public void OnHardSurfaceCollide(HardSurface hardSurface)
        {
            PlayerStateMachine playerState = PlayerCore.StateMachine;
            if (playerState.IsOnState<PlayerStateMachine.Diving>() || playerState.IsOnState<PlayerStateMachine.Dogoing>())
            {
                PlayerCore.Actor.Jump(hardSurfaceBounceHeight);
                PlayerCore.StateMachine.RefreshAbilities();
                PlayerCore.StateMachine.Transition<PlayerStateMachine.Airborne>();
            }
        }

        public void OnBouncySurfaceCollide(BouncySurface bouncySurface)
        {
            PlayerStateMachine playerState = PlayerCore.StateMachine;

            if (playerState.IsOnState<PlayerStateMachine.Diving>() || playerState.IsOnState<PlayerStateMachine.Dogoing>())
            {
                PlayerCore.Actor.Jump(bouncySurfaceDiveBounceHeight);
            } else
            {
                PlayerCore.Actor.Jump(bouncySurfaceNeutralBounceHeight);
            }
            PlayerCore.StateMachine.RefreshAbilities();
            PlayerCore.StateMachine.Transition<PlayerStateMachine.Airborne>();
        }
    }
}
