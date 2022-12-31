using UnityEngine;

using Player;

namespace Mechanics
{
    public class PlayerCrystalResponse : MonoBehaviour, ICrystalResponse
    {
        [SerializeField] private int bounceHeight;

        private IPlayerActionHandler _playerAction;
        private PlayerStateMachine _playerSM;

        private void Awake()
        {
            _playerAction = GetComponent<IPlayerActionHandler>();
            _playerSM = GetComponent<PlayerStateMachine>();
        }

        public void OnCrystalEnter(Crystal crystal)
        {
            if (_playerSM.IsOnState<PlayerStateMachine.Diving>())
            {
                _playerAction.Bounce(bounceHeight);
                _playerSM.RefreshAbilities();
                _playerSM.Transition<PlayerStateMachine.Airborne>();
                crystal.Break();
            }
        }
    }
}