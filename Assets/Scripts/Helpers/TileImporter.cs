using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Mechanics;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEngine;


namespace Helpers {

    [AutoCustomTmxImporter()]
    public class TileImporter : CustomTmxImporter {
        Dictionary<string, Type> typings = new Dictionary<string, Type> {
            {"Wall", typeof(Wall)},
            {"Spikes", typeof(Spikes)},
        };
        
        public override void TmxAssetImported(TmxAssetImportedArgs data) {
            ShadowCaster2DFromCollider s = new ShadowCaster2DFromCollider();
            
            SuperMap map = data.ImportedSuperMap;
            var args = data.AssetImporter;
            var layers = map.GetComponentsInChildren<SuperLayer>();
            var doc = XDocument.Load(args.assetPath);

            AddColliderToMap(map.transform.GetChild(0));
            
            foreach (SuperLayer layer in layers) {
                var props = GetLayerProps(doc, layer);
                if (props != null) {
                    string component = GetProp(props, "unity:Component");
                    if (component != null) AddComponentToLayer(layer.transform, component);
                    
                    string layerName = GetProp(props, "unity:Layer");
                    if (layerName != null) SetLayer(layer.transform, layerName);

                    string castShadows = GetProp(props, "unity:CastShadows");
                    if (castShadows == "true") GenerateShadows(layer.transform);

                    string offset = GetProp(props, "unity:anchorOffset");
                    if (offset == "true") AnchorOffset(layer.transform, GetLayer(doc, layer));
                }
            }
        }

        public void AddColliderToMap(Transform t) {
            BoxCollider2D b = t.gameObject.AddComponent<BoxCollider2D>();
            b.size = new Vector2(256, 144);
            b.offset = new Vector2(0, 72);
        }

        public void AnchorOffset(Transform layer, XElement docLayer) {
            int i = 0;
            foreach (var xElement in docLayer.Elements()) {
                Vector2 size = WidthAndHeight(xElement);
                string templatePath = xElement.GetAttributeAs<string>("template");
                Transform transformObj = FindObjByID(layer, xElement.GetAttributeAs<string>("id"));
                if (size != Vector2.zero && transformObj != null) {
                    
                } else if (templatePath != null) {
                    XElement templateX = XElementFromTemplatePath(templatePath);
                    size = WidthAndHeight(templateX);
                    size.y *= -1;
                }
                transformObj.position += new Vector3(size.x/2, size.y/2, 0);

                // layer.GetChild(i).position += new Vector3(s.x, -s.y, 0);
                ++i;
            }
        }

        public Vector2 WidthAndHeight(XElement o) {
            return new Vector2(o.GetAttributeAs<Int16>("width"), o.GetAttributeAs<Int16>("width"));
        }

        public List<Transform> GetEdges(Transform layer) {
            List<Transform> ret = new List<Transform>();
            for (int i = 0; i < layer.childCount; ++i) {
                var parent = layer.GetChild(i).GetChild(0);
                for(int j = 0; j<parent.childCount; ++j) {
                    ret.Add(parent.GetChild(j));
                }
            }
            return ret;
        }

        public void GenerateShadows(Transform layer) {
            foreach (var edgeObj in GetEdges(layer)) {
                ShadowCaster2DFromCollider.Execute(edgeObj.gameObject);
            }

            layer.gameObject.AddComponent<UnityEngine.Rendering.Universal.CompositeShadowCaster2D>();
        }

        public void AddComponentToLayer(Transform layer, string component) {
            for (int i = 0; i < layer.childCount; ++i) {
                var parent = layer.GetChild(i).GetChild(0);
                for(int j = 0; j<parent.childCount; ++j) {
                    parent.GetChild(j).gameObject.AddComponent(typings[component]);
                }
            }
        }

        public void SetLayer(Transform layer, string layerName) {
            for (int i = 0; i < layer.childCount; ++i) {
                var parent = layer.GetChild(i).GetChild(0);
                for(int j = 0; j<parent.childCount; ++j) {
                    parent.GetChild(j).gameObject.layer = LayerMask.NameToLayer(layerName);
                }
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

        public XElement GetLayerProps(XDocument doc, SuperLayer layer) {
            return GetLayer(doc, layer).Element("properties");
        }

        public XElement GetLayer(XDocument doc, SuperLayer layer) {
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