using MyBox;
using Phys;
using UnityEngine;

namespace Mechanics
{
    [RequireComponent(typeof(Actor))]
    public abstract class LauncherResponse : MonoBehaviour
    {
        public abstract void OnLauncherEnter(Launcher l);
        
        [SerializeField, AutoProperty] protected Actor _myActor;

        public abstract void AttemptLaunch(Launcher l, Vector2 v);

        public abstract void Bounce(int bounceHeight);
    }
}