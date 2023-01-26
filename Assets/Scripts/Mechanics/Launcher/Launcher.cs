using System.Collections;
using Helpers;
using MyBox;
using Phys;
using UnityEngine;

namespace Mechanics
{
    public class Launcher : Solid
    {
        [AutoProperty, SerializeField] private SpriteRenderer _mySR;

        [SerializeField] private int chargeTime;
        [SerializeField] private Vector2 launchVector;
        [SerializeField] private int bounceVelocity;


        public override bool Collidable()
        {
            return false;
        }

        private bool _charging = false;

        private void SetCharging(bool c)
        {
            _charging = c;
            _mySR.color = c ? Color.gray : Color.white;
        }

        public override bool OnCollide(PhysObj p, Vector2 direction) {
            LauncherResponse response = p.GetComponent<LauncherResponse>();

            if (response != null && !_charging)
            {
                response.OnLauncherEnter(this);
            }

            return base.OnCollide(p, direction);
        }

        public void StartLaunch(LauncherResponse launchedObj)
        {
            SetCharging(true);
            StartCoroutine(
                Helper.DelayAction(chargeTime, () =>
                {
                    launchedObj.AttemptLaunch(this, launchVector);
                    SetCharging(false);
                })
            );
        }

        public void Bounce(LauncherResponse response)
        {
            response.Bounce(bounceVelocity);
        }
    }
}