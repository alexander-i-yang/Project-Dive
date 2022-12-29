﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

using Mechanics;

using SuperTiled2Unity;
using SuperTiled2Unity.Editor;

using Cinemachine;
using MyBox;
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
        
        public override void TmxAssetImported(TmxAssetImportedArgs data) {
            var typePrefabReplacements = data.AssetImporter.SuperImportContext.Settings.PrefabReplacements;
            _prefabReplacements = typePrefabReplacements.ToDictionary(so => so.m_TypeName, so => so.m_Prefab);

            SuperMap map = data.ImportedSuperMap;
            var args = data.AssetImporter;
            var layers = map.GetComponentsInChildren<SuperLayer>();
            var objects = map.GetComponentsInChildren<SuperObject>();
            var doc = XDocument.Load(args.assetPath);

            AddRoomComponents(map.transform);
            //AddColliderToMap(map.transform.GetChild(0));
            foreach (SuperLayer layer in layers) {
                AddCustomPropertiesToLayer(layer, GetLayerXNode(doc, layer));
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

        private void AddCustomPropertiesToLayer(SuperLayer layer, XElement layerNode)
        {
            var customProps = layer.GetComponent<SuperCustomProperties>();
            if (customProps != null) {
                Dictionary<String, Action<CustomProperty>> propActions = new() {
                    {"Component", (prop) => {
                        AddComponentToCollidersInLayer(layer.transform, prop.GetValueAsString());
                    }},
                    {"Layer", (prop) => {
                        SetLayer(layer.transform, prop.GetValueAsString());
                    }},
                    {"CastShadows", (prop) => {
                        if (prop.GetValueAsBool()) GenerateShadows(layer.transform);
                    }},
                    {"AnchorOffset", (prop) => {
                        if (prop.GetValueAsBool()) AnchorOffset(layer, layerNode);
                    }}, 
                    {"AddFreeformLightPrefab", (prop) => {
                        AddFreeformLightPrefab(layer.transform, prop.GetValueAsString());
                    }},
                    {"PrefabReplace", (prop) => {
                        ReplacePrefab(layer.transform, prop.GetValueAsString());
                    }}
                };

                foreach (var kv in propActions) {
                    string propName = kv.Key;
                    Action<CustomProperty> act = kv.Value;
                    CustomProperty prop;
                    if (customProps.TryGetCustomProperty("unity:" + propName, out prop)) {
                        act(prop);
                    }
                }
            }   
        }

        public void AddColliderToMap(Transform t) {
            BoxCollider2D b = t.gameObject.AddComponent<BoxCollider2D>();
            b.size = new Vector2(256, 144);
            b.offset = new Vector2(128, -72);
        }

        public void AnchorOffset(SuperLayer layer, XElement docLayer) {
            foreach (var xElement in docLayer.Elements()) {
                Vector2 size = WidthAndHeight(xElement);
                string templatePath = xElement.GetAttributeAs<string>("template");
                Transform transformObj = FindObjByID(layer.transform, xElement.GetAttributeAs<string>("id"));
                if (size != Vector2.zero && transformObj != null)
                {

                }
                else if (templatePath != null)
                {
                    XElement templateX = XElementFromTemplatePath(templatePath);
                    size = WidthAndHeight(templateX);
                    size.y *= -1;
                }
                if (transformObj != null) transformObj.position += new Vector3(size.x / 2, size.y / 2, 0);

                // layer.GetChild(i).position += new Vector3(s.x, -s.y, 0);
            }
        }

        public Vector2 WidthAndHeight(XElement xNode) {
            return new Vector2(xNode.GetAttributeAs<Int16>("width"), xNode.GetAttributeAs<Int16>("height"));
        }

        public EdgeCollider2D[] GetEdges(Transform layer) {
            return layer.GetComponentsInChildren<EdgeCollider2D>();
        }

        public void GenerateShadows(Transform layer) {
            foreach (var edgeObj in GetEdges(layer)) {
                ComponentFromCollider.AddShadowCaster2D(edgeObj.gameObject);
            }

            layer.gameObject.AddComponent<UnityEngine.Rendering.Universal.CompositeShadowCaster2D>();
        }
        
        public void AddFreeformLightPrefab(Transform layer, String prefabName) {
            GameObject prefab = _prefabReplacements[prefabName];
            foreach (var edgeObj in GetEdges(layer)) {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.transform.SetParent(edgeObj.transform);
                Debug.Log(instance);
                Debug.Log(edgeObj.gameObject);
                instance.transform.localPosition = Vector3.zero;
                Light2D l = instance.GetComponent<Light2D>();

                if (l == null) {
                    throw new ConstraintException($"Prefab {prefabName} must have Freeform light attached");
                }
                l.SetShapePath(ComponentFromCollider.GetColliderPoints(edgeObj).ToArray());
            }
        }
        
        private void ReplacePrefab(Transform layer, string prefabName) {
            GameObject prefab = _prefabReplacements[prefabName];
            int i = 0;
            foreach (var edgeObj in GetEdges(layer)) {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.transform.SetParent(edgeObj.transform.parent);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.name = $"{instance.transform.name} ({i++})";

                Vector2[] points = ComponentFromCollider.GetColliderPointsVec2(edgeObj);
                if (points.Length != 4) {
                    throw new ConstraintException($"All objects in layer {layer} must be rectangular");
                }
                
                ComponentFromCollider.SetPrefabPoints(instance, points);
            }
        }

        public void AddComponentToCollidersInLayer(Transform layer, string component) {
            foreach (EdgeCollider2D edge in GetEdges(layer))
            {
                edge.gameObject.AddComponent(typings[component]);
            }
        }

        public void SetLayer(Transform layer, string layerName) {
            foreach (EdgeCollider2D edge in GetEdges(layer))
            {
                edge.gameObject.layer = LayerMask.NameToLayer(layerName);
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
        

        public Transform FindObjByID(Transform parent, string id) {
            for (var i=0; i < parent.childCount; i++){
                if(parent.GetChild(i).name.Contains("Object_" + id)) {
                    return parent.GetChild(i);
                }
            }

            return null;
        }

        public XElement XElementFromTemplatePath(String path) {
            return XDocument.Load("Assets/Tiles/" + path).Descendants("object").First();
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Warning;
        }
    }
}