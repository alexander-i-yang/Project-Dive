﻿using Core;

using System.Collections;
using Phys;
using Player;
using UnityEngine;


namespace Mechanics {
    public class Crystal : Solid, IDiveMechanic {
        [SerializeField] private int BounceHeight;
        private bool _broken = false;
        public double rechargeTime = 1;
        private SpriteRenderer _mySR;
        private UnityEngine.Rendering.Universal.Light2D _light;

        new void Start() {
            _mySR = GetComponent<SpriteRenderer>();
            _light = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
            base.Start();
        }
        
        public override bool Collidable() {
            return false;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            if (!_broken) p.EnterCrystal(this);
            return base.OnCollide(p, direction);
        }

        public override bool IsGround(PhysObj p) {
            return false;
        }

        public void Break() {
            _broken = true;
            StartCoroutine(BreakCoroutine());
        }

        public IEnumerator BreakCoroutine() {
            _mySR.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            var oldLightIntensity = _light.intensity;
            _light.intensity = 0;
            for (double i = 0; i < rechargeTime; i += Game.Instance.DeltaTime) {
                yield return null;
            }
            _mySR.color = Color.white;
            _broken = false;
            _light.intensity = oldLightIntensity;
        }

        public bool OnDiveEnter(IPlayerActionHandler p) {
            p.MechanicBounce(BounceHeight);
            Break();
            return true;
        }
    }
}