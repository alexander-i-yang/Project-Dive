using System;
using System.Linq;
using Helpers;
using MyBox;
using Phys;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Mechanics {
    public class Spike : Solid {
        public bool Collidable { get; private set; } = true;
        private Tilemap _tilemap;
        private Vector3Int _currentCell;
        private Coroutine _reEnableCoroutine;

        protected new void Start() {
            _tilemap = transform.parent.parent.GetComponent<Tilemap>();
            Vector2 tilePosition = GetComponent<EdgeCollider2D>().points[0];
            _currentCell = _tilemap.WorldToCell(tilePosition);
            _currentCell.y -= 1;
            
            base.Start();
        }

        public override bool OnCollide(PhysObj p, Vector2 direction) {
            return false;
        }

        public override bool PlayerCollide(PlayerActor p, Vector2 direction) {
            return p.EnterSpike(this);
        }

        public override bool IsGround(PhysObj b) {
            return false;
        }

        /*public void OnDrawGizmosSelected() {
            Vector2[] points = GetComponent<EdgeCollider2D>().points;
            Vector2 avg = Vector2.zero;
            for (int i = 0; i < points.Length-1; ++i) {
                var p = points[i];
                avg += p;
            }
            avg /= points.Length-1;
            avg.y -= 8;
            
            _tilemap = transform.parent.parent.GetComponent<Tilemap>();
            Vector2 tilePosition = GetComponent<EdgeCollider2D>().points[0];
            _currentCell = (Vector2Int)_tilemap.WorldToCell(tilePosition);
            _currentCell.y -= 1;

            Vector2 cellCenter = new Vector2(_currentCell.x, _currentCell.y)*8+new Vector2(4, -4);
            Vector2 directionV = cellCenter - avg;
            double directionAngle = Math.Ceiling(Vector2.SignedAngle(directionV, Vector2.right));
            Helper.DrawText(avg, directionAngle+"");
            Helper.DrawArrow(avg, cellCenter-avg , Color.green);
        }*/

        public void DiveEnter() {
            Collidable = false;
            _tilemap.SetColor(_currentCell, new Color(1,1, 1, 0.5f));
            if (_reEnableCoroutine != null) {
                StopCoroutine(_reEnableCoroutine);
                _reEnableCoroutine = null;
            }
        }

        public void DiveReEnable() {
            print("Reenable");
            _reEnableCoroutine = StartCoroutine(Helper.DelayAction(0.2f, () => {
                Collidable = true;
                _tilemap.SetColor(_currentCell, Color.white);
            }));
        }
    }
}