using Helpers;
using Phys;
using UnityEngine;

namespace Mechanics
{
    public class MushroomUnlockCollider : Solid
    {
        private bool _unlocked;
        [SerializeField] private float DelayTime;
        
        public override bool Collidable()
        {
            return false;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction)
        {
            if (!_unlocked)
            {
                _unlocked = true;
                MushroomUnlockLogic l = GetComponentInChildren<MushroomUnlockLogic>();
                StartCoroutine(Helper.DelayAction(DelayTime, l.Unlock));
            }
            return false;
        }
    }
}