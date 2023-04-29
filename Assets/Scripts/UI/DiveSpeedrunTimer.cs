using ASK.UI;
using UnityEngine;
using World;

namespace UI
{
    public class DiveSpeedrunTimer : SpeedrunTimer
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            Room.RoomTransitionEvent += OnRoomTransition;
        }
        
        protected override void OnDisable()
        {
            base.OnEnable();
            Room.RoomTransitionEvent -= OnRoomTransition;
        }
        
        [SerializeField] private Room _endRoom;
        private void OnRoomTransition(Room r)
        {
            if (r == _endRoom) StopTimer();
        }
    }
}