using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Mechanics;

using SuperTiled2Unity;
using SuperTiled2Unity.Editor;

using UnityEngine;
using UnityEngine.Tilemaps;

using World;

namespace Helpers {

    [AutoCustomTmxImporter()]
    public class TileImporter : CustomTmxImporter {
        Dictionary<string, Type> typings = new() {
            {"Wall", typeof(Wall)},
            {"Spikes", typeof(Spikes)},
        };
        
        public override void TmxAssetImported(TmxAssetImportedArgs data) {
            SuperMap map = data.ImportedSuperMap;
            var args = data.AssetImporter;
            var layers = map.GetComponentsInChildren<SuperLayer>();
            var objects = map.GetComponentsInChildren<SuperObject>();
            var doc = XDocument.Load(args.assetPath);

            AddMapComponents(map.transform);
            //AddColliderToMap(map.transform.GetChild(0));
            Debug.Log(map);
            foreach (SuperLayer layer in layers) {
                Debug.Log(layer);
                AddCustomPropertiesToLayer(layer, GetLayerXNode(doc, layer));
            }
        }

        private void AddMapComponents(Transform parent)
        {

            //Find the main tilemap
            Tilemap mainTilemap = FindGroundLayerTilemap(parent);
            if (mainTilemap == null)
            {
                Debug.LogError("Creating a room collider requires a Tiled layer named 'Ground'. This should probably be fixed in Tiled (or Logan screwed up *wink*).");
                return;
            }

            mainTilemap.CompressBounds();
            Bounds colliderBounds = mainTilemap.localBounds;

            //Add Polygon Collider
            PolygonCollider2D roomCollider = parent.gameObject.AddComponent<PolygonCollider2D>();
            roomCollider.pathCount = 0;
            Vector2 boundsMin = colliderBounds.min;
            Vector2 boundsMax = colliderBounds.max;
            roomCollider.SetPath(0, new Vector2[]
            {
                boundsMin,
                boundsMin + Vector2.right * colliderBounds.extents.x * 2,
                boundsMax,
                boundsMin + Vector2.up * colliderBounds.extents.y * 2,
            });
            roomCollider.offset = mainTilemap.transform.position;

            //Add Room Component
            parent.gameObject.AddComponent<Room>();
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

        private void AddCustomPropertiesToLayer(SuperLayer layer, XElement layerNode)
        {
            var customProps = layer.GetComponent<SuperCustomProperties>();
            if (customProps != null)
            {
                CustomProperty component;
                if (customProps.TryGetCustomProperty("unity:Component", out component))
                {
                    AddComponentToCollidersInLayer(layer.transform, component.GetValueAsString());
                }

                CustomProperty layerName;
                if (customProps.TryGetCustomProperty("unity:Layer", out layerName))
                {
                    SetLayer(layer.transform, layerName.GetValueAsString());
                }

                CustomProperty castShadows;
                if (customProps.TryGetCustomProperty("unity:CastShadows", out castShadows))
                {
                    if (castShadows.GetValueAsBool()) GenerateShadows(layer.transform);
                }

                CustomProperty anchorOffset;
                if (customProps.TryGetCustomProperty("unity:AnchorOffset", out anchorOffset))
                {
                    if (anchorOffset.GetValueAsBool()) AnchorOffset(layer, layerNode);
                }
            }   
        }

        public void AddColliderToMap(Transform t) {
            BoxCollider2D b = t.gameObject.AddComponent<BoxCollider2D>();
            b.size = new Vector2(256, 144);
            b.offset = new Vector2(128, -72);
        }

        public void AnchorOffset(SuperLayer layer, XElement docLayer) {
            foreach (var xElement in docLayer.Elements())
            {
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
                transformObj.position += new Vector3(size.x / 2, size.y / 2, 0);

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
                ShadowCaster2DFromCollider.Execute(edgeObj.gameObject);
            }

            layer.gameObject.AddComponent<UnityEngine.Rendering.Universal.CompositeShadowCaster2D>();
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

        private string GetProp(XElement props, String propName) {
            foreach (var p in props.Elements()) {
                if (p.Attribute("name").Value == propName) {
                    return p.Attribute("value").Value;
                }
            }

            return null;
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
    }
}