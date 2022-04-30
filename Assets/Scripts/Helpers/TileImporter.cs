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
                    if (component != null) AddToChunk(layer.transform, component);

                    string castShadows = GetProp(props, "unity:CastShadows");
                    if (castShadows == "true") GenerateShadows(layer.transform);
                }
            }
        }

        public void GenerateShadows(Transform layer) {
            for (int i = 0; i < layer.childCount; ++i) {
                layer.GetChild(i).gameObject.AddComponent<CompositeShadowCaster2D>();
                var obj = layer.GetChild(i).GetChild(0).gameObject;
                ShadowCaster2DFromCollider.Execute(obj);
            }

            layer.gameObject.AddComponent<CompositeShadowCaster2D>();
        }

        public void AddToChunk(Transform layer, string component) {
            for (int i = 0; i < layer.childCount; ++i) {
                layer.GetChild(i).GetChild(0).gameObject.AddComponent(typings[component]);
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