using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

using Mechanics;

using SuperTiled2Unity;
using SuperTiled2Unity.Editor;

using Cinemachine;
using Helpers;
using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

using World;
using Vector2 = UnityEngine.Vector2;
using LIL = TiledUtil.LayerImportLibrary;

namespace TiledUtil {

    [AutoCustomTmxImporter()]
    public class TileImporter : CustomTmxImporter, IFilterLoggerTarget {

        private readonly string vcamPrefabName = "VCam_Room";
        private readonly string vcamPrefabPath = "Assets/Prefabs";

        private Dictionary<String, GameObject> _prefabReplacements;

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
                { "Lava", ImportLavaTilemap },
                { "Ground", ImportGroundTilemap },
                { "Semisolid", ImportSemisolidTilemap },
                { "Water", ImportWaterTilemap },
                { "Dirt", ImportGroundTilemap },
                { "DecorBack", ImportDecorBackTilemap },
                { "Spikes", ImportSpikesTilemap },
                { "Branches", ImportBranchesTilemap },
            };
            
            //Applies to children
            Dictionary<String, Action<GameObject, int>> tileLayerImports = new() {
                { "Ground", ImportGround },
                { "Semisolid", ImportSemisolid },
                { "Dirt", ImportGround },
                { "Breakable", ImportBreakable },
                { "Lava", ImportLava },
                { "Water", ImportWater },
            };
            
            Dictionary<String, Action<Transform, XElement>> objectLayerImports = new() {
                // { "Mechanics", ImportMechanics},
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
                                                $"because it does not contain a Tiled layer named 'Ground' or 'Dirt'.");
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
                if (layer.gameObject.name.Equals("Ground") || layer.gameObject.name.Equals("Dirt"))
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
            Vector2[] points = LIL.EdgeToPoints(g);

            g = LIL.ConvertToPrefab(g, replacer, index);
            
            //Set collider points
            if (g.GetComponent<EdgeCollider2D>() != null)
            {
                LIL.SetEdgeCollider2DPoints(g, points);
            }
            else if (g.GetComponent<BoxCollider2D>() != null)
            {
                Vector2[] rectanglePoints = LIL.ColliderPointsToRectanglePoints(g, points);
                LIL.SetBoxColliderPoints(g, rectanglePoints);
            }
            
            //Set shadowcaster points
            if (g.GetComponent<ShadowCaster2D>() != null) LIL.AddShadowCast(g, points.ToVector3());
            
            //Set Layer (kinda hacky I know)
            LIL.SetLayer(g, "Interactable");
            return new Tuple<GameObject, Vector2[]>(g, points);
        }

        private GameObject AddWaterfalCollision(GameObject g, Vector2[] points)
        {
            GameObject waterfallReplace = _prefabReplacements["WaterfallCollider"];
            waterfallReplace = LIL.CreatePrefab(waterfallReplace, 0, g.transform);
            LIL.SetEdgeCollider2DPoints(waterfallReplace, points);
            return waterfallReplace;
        }

        private void ImportGround(GameObject g, int index) {
            var ret = ImportTileToPrefab(g, index, "Ground");
            AddWaterfalCollision(ret.Item1, ret.Item2);
        }
        
        private void ImportSemisolid(GameObject g, int index) {
            var ret = ImportTileToPrefab(g, index, "Semisolid");
        }

        private void ImportBreakable(GameObject g, int index) {
            var data = ImportTileToPrefab(g, index, "Breakable");
            g = data.Item1;
            Vector2[] colliderPoints = data.Item2;
            Vector2[] spritePoints = LIL.ColliderPointsToRectanglePoints(g, colliderPoints); 
            
            Vector2 avgSpritePoint = spritePoints.ComputeAverage();
            colliderPoints = colliderPoints.ComputeNormalized(avgSpritePoint);
            g.transform.localPosition = avgSpritePoint;
            
            LIL.SetNineSliceSprite(g, spritePoints);
            LIL.SetEdgeCollider2DPoints(g, colliderPoints);
            LIL.AddShadowCast(g, colliderPoints.ToVector3());
            g.GetRequiredComponent<SpriteRenderer>().SetSortingLayer("Interactable");
            AddWaterfalCollision(g, colliderPoints);
        }

        private void ImportLava(GameObject g, int _) {
            g.AddComponent<Lava>();
            LIL.SetLayer(g, "Interactable");
            Vector2[] colliderPoints = LIL.EdgeToPoints(g);
            LIL.AddFreeformLightPrefab(g, _prefabReplacements["LavaLight"], colliderPoints.ToVector3());
        }

        private void ImportWater(GameObject g, int _)
        {
            // g.GetComponentInChildren<>()<SpriteRenderer>().SetSortingLayer("Lava");
            // g.GetComponentInChildren<EdgeCollider2D>();
            AddWaterfalCollision(g, LIL.EdgeToPoints(g));
        }

        private void ImportLavaTilemap(GameObject g)
        {
            LIL.SetMaterial(g, "Lava");
            g.GetRequiredComponent<TilemapRenderer>().SetSortingLayer("Lava");
        }

        private void ImportBranchesTilemap(GameObject g)
        {
            g.GetRequiredComponent<TilemapRenderer>().SetSortingLayer("Above Ground");
            // LayerImportLibrary.SetMaterial(g, "Mask_Graph");
        }
        
        private void ImportSpikesTilemap(GameObject g)
        {
            GameObject.DestroyImmediate(g);
        }
        
        private void ImportDecorBackTilemap(GameObject g)
        {
            g.GetRequiredComponent<TilemapRenderer>().SetSortingLayer("Bg");
        }
        
        private void ImportGroundTilemap(GameObject g)
        {
            g.GetRequiredComponent<TilemapRenderer>().SetSortingLayer("Ground");
        }
        
        private void ImportSemisolidTilemap(GameObject g)
        {
            g.GetRequiredComponent<TilemapRenderer>().SetSortingLayer("Ground");
        }

        private void ImportWaterTilemap(GameObject g)
        {
            g.SetLayerRecursively("Water");
            g.GetRequiredComponent<TilemapRenderer>().SetSortingLayer("Lava");
            LIL.SetMaterial(g, "Mask_Graph");
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