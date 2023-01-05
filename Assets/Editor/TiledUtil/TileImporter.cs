﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

using Mechanics;

using SuperTiled2Unity;
using SuperTiled2Unity.Editor;

using Cinemachine;
using TiledUtil;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

using World;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Helpers {

    [AutoCustomTmxImporter()]
    public class TileImporter : CustomTmxImporter, IFilterLoggerTarget {

        private readonly string vcamPrefabName = "VCam_Room";
        private readonly string vcamPrefabPath = "Assets/Prefabs";

        private Dictionary<String, GameObject> _prefabReplacements;
        
        private static Dictionary<string, Type> typings = new() {
            {"Wall", typeof(Wall)},
            {"Spikes", typeof(Spike)},
            {"Lava", typeof(Lava)},
            {"Breakable", typeof(Breakable)},
        };
        
        public override void TmxAssetImported(TmxAssetImportedArgs data)
        {
            var typePrefabReplacements = data.AssetImporter.SuperImportContext.Settings.PrefabReplacements;
            _prefabReplacements = typePrefabReplacements.ToDictionary(so => so.m_TypeName, so => so.m_Prefab);

            SuperMap map = data.ImportedSuperMap;
            var args = data.AssetImporter;
            var layers = map.GetComponentsInChildren<SuperLayer>();
            var objects = map.GetComponentsInChildren<SuperObject>();
            var doc = XDocument.Load(args.assetPath);

            AddRoomComponents(map.transform);

            //Applies to the entire tilemap
            Dictionary<String, Action<GameObject>> tilemapLayerImports = new()
            {
                { "Lava", ImportLavaTilemap }
            };
            
            //Applies to children
            Dictionary<String, Action<GameObject, int>> tileLayerImports = new() {
                { "Ground", ImportGround },
                { "Breakable", ImportBreakable },
                { "Lava", ImportLava },
            };
            
            Dictionary<String, Action<Transform, XElement>> objectLayerImports = new() {
                { "Mechanics", ImportMechanics},
            };
            
            foreach (SuperLayer layer in layers) {
                string layerName = layer.name;
                if (tilemapLayerImports.ContainsKey(layerName))
                {
                    tilemapLayerImports[layerName](layer.gameObject);
                }

                if (tileLayerImports.ContainsKey(layerName)) {
                  
                    ResolveTileLayerImports(layer.transform, tileLayerImports[layerName]);
                } else if (objectLayerImports.ContainsKey(layerName)) {
                    objectLayerImports[layerName](layer.transform, GetLayerXNode(doc, layer));
                }
            }
        }

        private void AddRoomComponents(Transform room)
        {
            room.gameObject.AddComponent<Room>();

            Tilemap mainTilemap = FindGroundLayerTilemap(room);
            if (mainTilemap == null)
            {
                FilterLogger.LogWarning(this,   $"Room bounds and components not added to tiled map {room.gameObject.name} " +
                                                $"because it does not contain a Tiled layer named 'Ground'.");
                return;
            }
            mainTilemap.CompressBounds();
            PolygonCollider2D bounds = AddPolygonColliderToRoom(room, mainTilemap);
            AddVCamToRoom(room, bounds);
        }

        private Tilemap FindGroundLayerTilemap(Transform parent)
        {
            SuperTileLayer[] layers = parent.GetComponentsInChildren<SuperTileLayer>();
            foreach (SuperTileLayer layer in layers)
            {
                if (layer.gameObject.name.Equals("Ground"))
                {
                    return layer.GetComponent<Tilemap>();
                }
            }

            return null;
        }

        private PolygonCollider2D AddPolygonColliderToRoom(Transform room, Tilemap mainTilemap)
        {
            Bounds colliderBounds = mainTilemap.localBounds;

            PolygonCollider2D roomCollider = room.gameObject.AddComponent<PolygonCollider2D>();
            roomCollider.pathCount = 0;
            Vector2 boundsMin = colliderBounds.min;
            Vector2 boundsMax = colliderBounds.max;
            float alpha = 0.01f;
            roomCollider.SetPath(0, new Vector2[]
            {
                boundsMin - Vector2.one * alpha,
                boundsMin + Vector2.right * colliderBounds.extents.x * 2 + new Vector2(alpha, -alpha),
                boundsMax + Vector2.one * alpha,
                boundsMin + Vector2.up * colliderBounds.extents.y * 2 + new Vector2(-alpha, alpha),
            });
            roomCollider.offset = mainTilemap.transform.position;
            roomCollider.isTrigger = true;

            return roomCollider;
        }

        private void AddVCamToRoom(Transform room, PolygonCollider2D boundingShape)
        {
            string[] guids = AssetDatabase.FindAssets("t:prefab " + vcamPrefabName, new string[] { vcamPrefabPath });
            if (guids.Length == 0)
            {
                Debug.LogError("Could not find VCam_Room Prefab. Make sure the prefab exists in the Assets/Prefabs directory and is named correctly.");
                return;
            }

            GameObject vcamPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0]));
            GameObject instance = InstantiationExtension.InstantiateKeepPrefab(vcamPrefab);
            instance.transform.SetParent(room);

            var confiner = instance.GetComponent<CinemachineConfiner2D>();
            confiner.m_BoundingShape2D = boundingShape;
        }

        private Tuple<GameObject, Vector2[]> ImportTileToPrefab(GameObject g, int index, String prefabName) {
            GameObject replacer = _prefabReplacements[prefabName];
            Vector2[] points = LayerImportLibrary.EdgeToPoints(g);

            g = LayerImportLibrary.ConvertToPrefab(g, replacer, index);
            
            LayerImportLibrary.SetEdgeCollider2DPoints(g, points);
            LayerImportLibrary.AddShadowCast(g, points.ToVector3());
            LayerImportLibrary.SetLayer(g, "Interactable");
            return new Tuple<GameObject, Vector2[]>(g, points);
        }

        private void ImportGround(GameObject g, int index) {
            ImportTileToPrefab(g, index, "Ground");
        }

        private void ImportBreakable(GameObject g, int index) {
            var data = ImportTileToPrefab(g, index, "Breakable");
            g = data.Item1;
            Vector2[] colliderPoints = data.Item2;
            Vector2[] spritePoints = LayerImportLibrary.ColliderPointsToSpritePoints(g, colliderPoints); 
            
            Vector2 avgSpritePoint = spritePoints.ComputeAverage();
            colliderPoints = colliderPoints.ComputeNormalized(avgSpritePoint);
            g.transform.localPosition = avgSpritePoint;
            
            LayerImportLibrary.SetNineSliceSprite(g, spritePoints);
            LayerImportLibrary.SetEdgeCollider2DPoints(g, colliderPoints);
            LayerImportLibrary.AddShadowCast(g, colliderPoints.ToVector3());
        }

        private void ImportLava(GameObject g, int _) {
            g.AddComponent<Lava>();
            LayerImportLibrary.SetLayer(g, "Interactable");
            Vector2[] colliderPoints = LayerImportLibrary.EdgeToPoints(g);
            LayerImportLibrary.AddFreeformLightPrefab(g, _prefabReplacements["LavaLight"], colliderPoints.ToVector3());
        }

        private void ImportLavaTilemap(GameObject g)
        {
            Debug.Log($"HELLOOOOO");
            LayerImportLibrary.SetMaterial(g, "Lava");
        }
        
        private void ImportMechanics(Transform layer, XElement element) {
            // LayerImportLibrary.AnchorOffset(layer, element);
        }

        private void ResolveTileLayerImports(Transform layer, Action<GameObject, int> import) {
            if (layer.childCount > 0) {
                Transform t = layer.GetChild(0);
                t.ForEachChild(import);
            }
        }

        public XElement GetLayerXNode(XDocument doc, SuperLayer layer) {
            foreach (XElement xNode in doc.Element("map").Elements()) {
                XAttribute curName = xNode.Attribute("name");
                if (curName != null && curName.Value == layer.name) {
                    return xNode;
                }
            }

            return null;
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Warning;
        }
    }
}