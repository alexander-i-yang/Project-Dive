using System;
using System.Collections.Generic;
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
            SuperMap map = data.ImportedSuperMap;
            var args = data.AssetImporter;
            var layers = map.GetComponentsInChildren<SuperLayer>();
            var doc = XDocument.Load(args.assetPath);
            foreach (SuperLayer layer in layers) {
                var props = GetLayerProps(doc, layer);
                if (props != null) {
                    string component = GetProp(props, "unity:Component");
                    AddToChunk(layer.transform, component);
                }
            }
        }

        public void AddToChunk(Transform layer, string component) {
            if (component != null) {
                for (int i = 0; i < layer.childCount; ++i) {
                    layer.GetChild(i).GetChild(0).gameObject.AddComponent(typings[component]);
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