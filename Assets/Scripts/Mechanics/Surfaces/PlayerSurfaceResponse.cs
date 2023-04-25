using UnityEngine;
using Player;
using UnityEngine.Events;

namespace Mechanics
{
    public class PlayerSurfaceResponse : MonoBehaviour, IHardSurfaceResponse, IBouncySurfaceResponse, IBeegSurfaceResponse
    {
        [SerializeField] private int hardSurfaceBounceHeight;
        [SerializeField] private int bouncySurfaceNeutralBounceHeight;
        [SerializeField] private int bouncySurfaceDiveBounceHeight;

        private PlayerStateMachine PlayerSM => PlayerCore.StateMachine;

        public void OnHardSurfaceCollide(HardSurface hardSurface)
        {
            if (PlayerSM.UsingDrill)
            {
                PlayerCore.Actor.Bounce(hardSurfaceBounceHeight);
                PlayerSM.RefreshAbilities();
                PlayerSM.Transition<PlayerStateMachine.Airborne>();
            }
        }

        public void OnBouncySurfaceCollide(BouncySurface bouncySurface)
        {
            if (PlayerCore.StateMachine.UsingDrill)
            {
                PlayerCore.Actor.Bounce(bouncySurfaceDiveBounceHeight);
            } else
            {
                PlayerCore.Actor.Bounce(bouncySurfaceNeutralBounceHeight);
            }
            PlayerSM.RefreshAbilities();
            PlayerSM.Transition<PlayerStateMachine.Airborne>();
        }

        public void OnBeegSurfaceCollide(BouncySurface bouncySurface, UnityEvent onBounce, UnityEvent onDive)
        {
            if (PlayerCore.StateMachine.UsingDrill)
            {
                PlayerCore.Actor.BeegBounce(bouncySurfaceDiveBounceHeight);
                onDive.Invoke();
            } else
            {
                PlayerCore.Actor.SmolBounce(bouncySurfaceNeutralBounceHeight);
                onBounce.Invoke();
            }
            PlayerSM.RefreshAbilities();
            PlayerSM.Transition<PlayerStateMachine.Airborne>();
        }
    }
}
