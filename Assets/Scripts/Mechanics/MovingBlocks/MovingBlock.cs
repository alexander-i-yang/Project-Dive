﻿using Mechanics.MovingBlocks;
using Phys;
using UnityEngine;

namespace Mechanics {
    [RequireComponent(typeof(MovingBlockSM))]
    public class MovingBlock : Solid {
        [SerializeField] private Vector2 _direction = new Vector2(1, 0);
        [SerializeField] private int _zoomVelocity = 100;
        [SerializeField] private int _returnVelocity = 25;
        [SerializeField] private int _decel = 10;
        [SerializeField] private int _distance = 16;
        private MovingBlockSM _mySM;
        private Vector2 _zoomStartPos;
        private Vector2 _startPos;

        public override bool Collidable() {
            return true;
        }

        new void Start() {
            _startPos = transform.position;
            _mySM = GetComponent<MovingBlockSM>();
            base.Start();
        }

        public override bool OnCollide(PhysObj p, Vector2 direction) {
            if (!(p.GetComponent<Wall>() is null)) {
                velocity = Vector2.zero;
                _mySM.HitWall();
                return true;
            }

            return false;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            if (direction.y < 0 && p.IsDiving()) {
                _mySM.Transition<Zooming>();
            }

            return true;
        }

        public void Zoom() {
            _zoomStartPos = transform.position;
            velocity = _direction * _zoomVelocity;
        }

        public bool PositionedAtStart() {
            return DistanceFromPos(_startPos) <= 0;
        }

        public bool PositionedAtZoomEnd() {
            return DistanceFromPos(_zoomStartPos) >= _distance;
        }
        
        public bool PositionedAtDecelStart() {
            return DistanceFromPos(_zoomStartPos) >= 12;
        }

        private int DistanceFromPos(Vector2 p) {
            Vector2 d = _direction * ((Vector2) transform.position - p);
            return (int) (d.x + d.y);
        }

        public void Return() {
            velocity = _direction * -_returnVelocity;
        }

        public void Stop() {
            velocity = Vector2.zero;
        }

        public void Decel() {
            velocity -= _direction * _decel;
        }
    }
}