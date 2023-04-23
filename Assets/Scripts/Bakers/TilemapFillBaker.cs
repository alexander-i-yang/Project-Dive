using System;
using System.Collections.Generic;
using Clipper2Lib;
using Helpers;
using Mechanics;
using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using World;

namespace Bakers
{
    public class TilemapFillBaker : MonoBehaviour, IBaker
    {
        [SerializeField] private Tilemap fillMap;
        [SerializeField] private Tilemap fillTileRef;
        [SerializeField] private Vector3 fillTilePosRef;

        [SerializeField] private TileBase fillTile;
        [SerializeField] private Vector2Int topLeftCorner;
        [SerializeField] private Vector2Int bottomRightCorner;
        [SerializeField] private bool floodFill = true;
        [SerializeField] private Vector2 pointsOffset;
        [SerializeField] private Vector2Int pointsMargin;

        // [SerializeField] private Room r0;
        // [SerializeField] private Room r1;

        public void SetTile()
        {
            fillTile = GetBaseTile(fillTileRef, fillTilePosRef);
            #if UNITY_EDITOR
                EditorUtility.SetDirty(this);
            #endif
        }

        public void ClearTiles()
        {
            SetFillMap();
            fillMap.ClearAllTiles();
        }

        public void Bake()
        {
            CalculatePoints();
            SetTiles();
            /*foreach (var p in OffsetPath(ret))
            {
                var col = transform.GetChild(0).gameObject.AddComponent<EdgeCollider2D>();
                col.points = PathToPoints(p);
            }*/
        }

        public void CalculatePoints()
        {
            Room[] rooms = FindObjectsOfType<Room>();
            Paths64 ret = new Paths64();
            var initPaths = PointsToPath(
                rooms[0].GetComponent<PolygonCollider2D>().points,
                rooms[0].transform.position
            );
            ret.Add(Clipper.MakePath(initPaths));

            foreach (var room in rooms)
            {
                print(room);
                var roomPts = room.GetComponent<PolygonCollider2D>().points;
                ret = CombinePoints(ret, PointsToPath(roomPts, room.transform.position));
            }

            Paths64 translatedPath = Clipper.TranslatePaths(ret, pointsMargin.x, pointsMargin.y);
            ret = Clipper.Union(ret, translatedPath, FillRule.NonZero);

            var col = gameObject.GetOrAddComponent<EdgeCollider2D>();
            col.points = PathToPoints(ret[0]);
        }

        public void SetTiles()
        {
            SetFillMap();
            ClearTiles();
            var col = GetComponent<EdgeCollider2D>();
            if (col == null)
            {
                CalculatePoints();
                col = GetComponent<EdgeCollider2D>();
            }
            var points = col.points;
            for(int i = 0; i < points.Length; ++i)
            {
                Vector2 p0 = points[i];
                Vector2 p1 = i + 1 >= points.Length ? points[0] : points[i+1];
                var tilep0 = fillMap.WorldToCell(p0);
                var tilep1 = fillMap.WorldToCell(p1);

                var line = LineGenerator(
                    new Vector2Int(tilep0.x, tilep0.y),
                    new Vector2Int(tilep1.x, tilep1.y)
                );
                fillMap.SetTiles(line.tilePts, line.tileObjs);
            }

            SetTileSquare(topLeftCorner, bottomRightCorner);
            if (floodFill) fillMap.FloodFill((Vector3Int)(topLeftCorner + new Vector2Int(2, -2)), fillTile);
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(fillMap);
            #endif
        }

        public void SetFillMap()
        {
            var f = FindObjectOfType<FillTilemap>();
            if (f == null)
            {
                throw new Exception("Must have a fill map present. Add one by creating a tilemap with the FillTilemap component");
            }

            fillMap = f.GetComponent<Tilemap>();
        }

        private void SetTileSquare(Vector2Int corner0, Vector2Int corner2)
        {
            Vector2Int corner1 = new Vector2Int(corner0.x, corner2.y);
            Vector2Int corner3 = new Vector2Int(corner2.x, corner0.y);

            var lines = new[]
            {
                LineGenerator(corner0, corner1),
                LineGenerator(corner1, corner2),
                LineGenerator(corner2, corner3),
                LineGenerator(corner3, corner0),
            };
            
            foreach (var line in lines)
            {
                fillMap.SetTiles(line.tilePts, line.tileObjs);
            }
        }

        private (Vector3Int[] tilePts, TileBase[] tileObjs) LineGenerator(Vector2Int p0, Vector2Int p1)
        {
            List<Vector3Int> tilePts = new();
            List<TileBase> tiles = new();

            Vector2Int diff = p1 - p0;
            int len = 0;

            if (diff.x == 0)
            {
                len = Math.Abs(diff.y);
            }
            else
            {
                len = Math.Abs(diff.x);
            }

            Vector2Int direction = new Vector2Int(Math.Sign(diff.x), Math.Sign(diff.y));
            for (int i = 0; i < len; ++i)
            {
                var add = p0 + direction * i;
                tilePts.Add(new Vector3Int(add.x, add.y));
                tiles.Add(fillTile);
            }

            return (tilePts: tilePts.ToArray(), tileObjs: tiles.ToArray());
        }

        private TileBase GetBaseTile(Tilemap tmap, Vector3 offset)
        {
            return tmap.GetTile(tmap.WorldToCell(offset));
        }

        private Paths64 OffsetPath(Paths64 subj)
        {
            return Clipper.InflatePaths(subj, 8, JoinType.Square, EndType.Square, 0);
        }

        private Paths64 CombinePoints(Paths64 subj, int[] p1)
        {
            Paths64 clip = new Paths64();
            clip.Add(Clipper.MakePath(p1));
            return Clipper.Union(subj, clip, FillRule.NonZero);
        }
        
        private int[] PointsToPath(Vector2[] p1, Vector2 offset) {
            List<int> path1 = new();
            foreach (var v in p1)
            {
                var tempV = v;
                tempV += offset;
                path1.Add((int)Math.Round(tempV.x));
                path1.Add((int)Math.Round(tempV.y));
            }
            return path1.ToArray();
        }
        
        private Vector2[] PathToPoints(Path64 p) {
            List<Vector2> ret = new();
            foreach (Point64 i in p)
            {
                ret.Add(new Vector2(i.X, i.Y) + pointsOffset);
            }
            return ret.ToArray();
        }
    }
}