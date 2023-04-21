using UnityEngine;

using Player;

namespace Mechanics
{
    public class PlayerCrystalResponse : MonoBehaviour, ICrystalResponse
    {
        [SerializeField] private int bounceHeight;

        public void OnCrystalEnter(Crystal crystal)
        {
            if (PlayerCore.StateMachine.IsOnState<PlayerStateMachine.Diving>())
            {
                PlayerCore.Actor.Bounce(bounceHeight);
                PlayerCore.StateMachine.RefreshAbilities();
                PlayerCore.StateMachine.Transition<PlayerStateMachine.Airborne>();
                crystal.Break();
            }
        }
    }
}