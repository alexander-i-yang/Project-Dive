using System;
using DefaultNamespace;
using Mechanics.MovingBlocks;
using Phys;
using UnityEngine;

namespace Mechanics {
    [RequireComponent(typeof(MovingBlockSM))]
    public class MovingBlock : Solid {
        [SerializeField] private Vector2 _direction = new Vector2(1, 0);
        [SerializeField] private int _magnitude = 100;
        [SerializeField] private int _decel = -10;
        private MovingBlockSM _mySM;
        private Vector2 _startPos;
        
        new void Start() {
            _startPos = transform.position;
            _mySM = GetComponent<MovingBlockSM>();
            base.Start();
        }

        public override bool OnCollide(PhysObj p, Vector2 direction) {
            if (!(p.GetComponent<Wall>() is null)) {
                velocity = Vector2.zero;
                return true;
            }
            return false;
        }

        public override bool PlayerCollide(PlayerController p, Vector2 direction) {
            if (direction.y < 0 && p.IsDiving()) {
                _mySM.Transition<Zooming>();
            }
            return true;
        }

        public void Zoom() {
            velocity = _direction*_magnitude;
        }

        public new void FixedUpdate() {
            /*if (velocity != Vector2.zero) {
                velocity += _direction * (_decel * Game.FixedDeltaTime);
            }*/
            _mySM.FixedUpdate();
            base.FixedUpdate();
        }

        public bool PositionedAtStart() {
            return DistanceFromStart() <= 0;
        }
        
        public bool PositionedAtEnd() {
            print(DistanceFromStart());
            return DistanceFromStart() >= 16;
        }

        private int DistanceFromStart() {
            Vector2 d = _direction * ((Vector2) transform.position - _startPos);
            return (int) (d.x + d.y);
        }

        public void Decel() {
            velocity += _direction * _decel;
        }

        public void Stop() {
            velocity = Vector2.zero;
        }
    }
}