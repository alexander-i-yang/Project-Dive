using System;
using Core;
using Helpers;
using UnityEngine;
using UnityEngine.UI;
using World;

namespace UI
{
    public class SpeedrunTimer : MonoBehaviour
    {
        private Text _text;

        [SerializeField] private Room _endRoom;
        private bool _running = true;
        private float _curTime;

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        private void OnEnable()
        {
            Room.RoomTransitionEvent += OnRoomTransition;
        }

        private void OnDisable()
        {
            Room.RoomTransitionEvent -= OnRoomTransition;
        }

        private void Update()
        {
            if (_running)
            {
                _curTime = Game.Instance.Time;
                _text.text = Helper.FormatTime(_curTime);
            }
        }

        private void OnRoomTransition(Room r)
        {
            if (r == _endRoom) _running = false;
        }
    }
}