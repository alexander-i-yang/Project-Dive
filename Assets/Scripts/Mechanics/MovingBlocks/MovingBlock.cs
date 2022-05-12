using System;
using DefaultNamespace;
using Mechanics.MovingBlocks;
using Phys;
using UnityEditor;
using UnityEngine;

namespace Mechanics {
    [RequireComponent(typeof(MovingBlockSM))]
    public class MovingBlock : Solid {
        [SerializeField] private Vector2 _direction = new Vector2(1, 0);
        [SerializeField] private int _magnitude = 100;
        [SerializeField] private int _decel = -10;
        private MovingBlockSM _mySM;
        private Vector2 _zoomStartPos;
        private Vector2 _startPos;
        
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

        public override bool PlayerCollide(PlayerController p, Vector2 direction) {
            if (direction.y < 0 && p.IsDiving()) {
                _mySM.Transition<Zooming>();
            }
            return true;
        }

        public void Zoom() {
            _zoomStartPos = transform.position;
            velocity = _direction*_magnitude;
        }

        public bool PositionedAtStart() {
            return DistanceFromPos(_startPos) <= 0;
        }
        
        public bool PositionedAtZoomEnd() {
            return DistanceFromPos(_zoomStartPos) >= 16;
        }

        private int DistanceFromPos(Vector2 p) {
            Vector2 d = _direction * ((Vector2) transform.position - p);
            return (int) (d.x + d.y);
        }

        public void Return() {
            velocity = _direction * -_magnitude;
        }

        public void Stop() {
            velocity = Vector2.zero;
        }

        public void OnDrawGizmosSelected() {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red; 
            Handles.Label(transform.position, "" + velocity, style);
        }
    }
}