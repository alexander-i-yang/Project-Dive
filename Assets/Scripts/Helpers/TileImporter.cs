using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Mechanics;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

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
                    if (offset != null) AnchorOffset(layer.transform, Int32.Parse(offset));
                }
            }
        }

        public void AnchorOffset(Transform layer, int o) {
            for (int i = 0; i < layer.childCount; ++i) {
                layer.GetChild(i).position += new Vector3(o, o, 0);
            }
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

            layer.gameObject.AddComponent<CompositeShadowCaster2D>();
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
            foreach (XElement xNode in doc.Element("map").Elements()) {
                XAttribute curName = xNode.Attribute("name");
                if (curName != null && curName.Value == layer.name) {
                    return xNode.Element("properties");
                }
            }

            return null;
        }
    }
}