using UnityEngine;

namespace Mechanics
{
    public class DirectionalSpike : Spike
    {
        public Vector2 Direction;

        public void SetDirectionFromCode(int code)
        {
            Direction = ReadDirectionCode(code);
        }
        
        private Vector2 ReadDirectionCode(int directionCode)
        {
            Vector2[] directions = { Vector2.right, Vector2.up, Vector2.left, Vector2.down};
            if (directionCode < 0 || directionCode >= 4)
            {
                Debug.LogError($"Invalid Spike Direction code {directionCode}");
                return Vector2.up;
            }
            return directions[directionCode];
        }

        public bool ShouldDieFromVelocity(Vector2 playerVelocity)
        {
            return Vector2.Dot(Direction, playerVelocity) <= 0;
        }
    }
}