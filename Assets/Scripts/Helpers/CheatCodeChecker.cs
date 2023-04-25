using System;
using Core;
using Player;
using UnityEngine;

namespace Helpers
{
    public class CheatCodeChecker : MonoBehaviour
    {
        private Func<bool>[] cheatCode;
        private int index;
        private GameTimer _keyTimer;

        [SerializeField] private float keyDelay;
 
        void Start()
        {
            var player = PlayerCore.Input;
            cheatCode = new Func<bool>[] {
                player.JumpStarted,
                player.JumpStarted,
                player.DiveStarted,
                player.DiveStarted,
                () => player.GetMovementInput() == -1,
                () => player.GetMovementInput() == 1,
                () => player.GetMovementInput() == -1,
                () => player.GetMovementInput() == 1,
                player.JumpStarted,
                player.DiveStarted,
            };
            index = 0;    
        }
 
        void Update() {
            // Check if any key is pressed
            if (cheatCode[index]()) {
                // Add 1 to index to check the next key in the code
                index++;
                _keyTimer = GameTimer.StartNewTimer(keyDelay);
            }
            else if (PlayerCore.Input.AnyKeyPressed())
            {
                index = 0;
            }

            if (!GameTimer.TimerFinished(_keyTimer))
            {
                GameTimer.Update(_keyTimer);
                if (GameTimer.TimerFinished(_keyTimer))
                {
                    index = 0;
                }
            }

            // If index reaches the length of the cheatCode string, 
            // the entire code was correctly entered
            if (index == cheatCode.Length)
            {
                Game.Instance.IsDebug = true;
                index = 0;
            }
        }
    }
}