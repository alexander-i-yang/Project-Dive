using UnityEngine;
using World;

namespace UI
{
    public class ShowDuringRoom : MonoBehaviour
    {
        [SerializeField] private Room roomToShow;
        [SerializeField] private FakeAnimator objToShow;
        
        private void OnEnable()
        {
            Room.RoomTransitionEvent += OnRoomTransition;
        }
        
        private void OnDisable()
        {
            Room.RoomTransitionEvent -= OnRoomTransition;
        }

        private void OnRoomTransition(Room r)
        {
            if (r == roomToShow) objToShow.Show();
            else objToShow.Hide();
        }
    }
}