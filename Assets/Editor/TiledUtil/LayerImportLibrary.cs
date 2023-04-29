using System;
using System.Data;
using System.Reflection;
using System.Xml.Linq;
using Cinemachine.Utility;
using ASK.Helpers;
using MyBox;
using SuperTiled2Unity.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

namespace TiledUtil {
    public static class LayerImportLibrary {
        private static BindingFlags accessFlagsPrivate =
            BindingFlags.NonPublic | BindingFlags.Instance;

        private static FieldInfo meshField =
            typeof(ShadowCaster2D).GetField("m_Mesh", accessFlagsPrivate);

        private static FieldInfo shapePathField =
            typeof(ShadowCaster2D).GetField("m_ShapePath", accessFlagsPrivate);

        private static MethodInfo onEnableMethod =
            typeof(ShadowCaster2D).GetMethod("OnEnable", accessFlagsPrivate);

        public static Vector2[] EdgeToPoints(GameObject g) {
            return g.GetRequiredComponent<EdgeCollider2D>().points;
        }

        public static void SetEdgeCollider2DPoints(GameObject g, Vector2[] points) {
            g.GetRequiredComponent<EdgeCollider2D>().points = points;
        }

        public static GameObject CreatePrefab(GameObject prefab, int index, Transform parent) {
            Transform instance = ((GameObject)PrefabUtility.InstantiatePrefab(prefab)).transform;
            instance.SetParent(parent);
            instance.localPosition = Vector3.zero;
            instance.name = $"{instance.name} ({index})";
            return instance.gameObject;
        }

        public static GameObject ConvertToPrefab(GameObject original, GameObject prefab, int index) {
            GameObject instance = CreatePrefab(prefab, index, original.transform.parent);
            UnityEngine.Object.DestroyImmediate(original);
            return instance;
        }

        public static GameObject AddPrefabAsChild(GameObject parent, GameObject prefab) {
            return CreatePrefab(prefab, 0, parent.transform);
        }
        
        public static ShadowCaster2D AddShadowCast(GameObject g, Vector3[] points)
        {
            ShadowCaster2D caster = g.GetRequiredComponent<ShadowCaster2D>();
            shapePathField.SetValue(caster, points);
            meshField.SetValue(caster, null);
            onEnableMethod.Invoke(caster, new object[0]);
            return caster;
        }

        public static void SetNineSliceSprite(GameObject g, Vector2[] points) {
            SpriteRenderer spriteRenderer = g.GetRequiredComponent<SpriteRenderer>();
            if (spriteRenderer.drawMode != SpriteDrawMode.Tiled) {
                throw new ConstraintException($"Gameobject {g} must be 9 sliced in order to dynamically set sprite");
            }
            
            //Compute width and height of collider from points
            Vector2 dimensions = points[0] - points[2];
            dimensions.x = Math.Abs(dimensions.x);
            dimensions.y = Math.Abs(dimensions.y);
            spriteRenderer.size = dimensions;
        }

        public static Vector2[] ColliderPointsToRectanglePoints(GameObject g, Vector2[] points) {
            points = points.Slice(0, points.Length - 1); // EdgeColliders have one extra point
            if (points.Length != 4) {
                throw new ConstraintException($"GameObject {g} must be rectangular");
            }
            return points;
        }

        public static void SetBoxColliderPoints(GameObject g, Vector2[] rectanglePoints)
        {
            //Move the position up by 8 units, I think bc the layers all get set 8 units down.
            g.transform.localPosition += Vector3.up * 8;
            var boxCollider = g.GetRequiredComponent<BoxCollider2D>();
            Vector2 size = (rectanglePoints[0] - rectanglePoints[2]).Abs();
            Vector2 topRightBox = (Vector2)g.transform.position + size/2;
            Vector2 topRightWorld = rectanglePoints.MaxManhattan();
            Vector2 offset = topRightWorld - topRightBox;
            boxCollider.offset = offset;
            boxCollider.size = size;
            //Reset position here.
            g.transform.localPosition -= Vector3.up * 8;
        }

        public static void AddPolygonCollider(GameObject g, Vector2[] colliderPoints)
        {
            var polygonCollider = g.AddComponent<PolygonCollider2D>();
            SetPolygonColliderPoints(g, colliderPoints);
            //polygonCollider.isTrigger = true;
        }

        public static void SetPolygonColliderPoints(GameObject g, Vector2[] colliderPoints)
        {
            var polygonCollider = g.GetRequiredComponent<PolygonCollider2D>();
            polygonCollider.points = colliderPoints;
        }

        #region ObjectLayers

        private static Vector2 WidthAndHeight(XElement xNode) {
            return new Vector2(xNode.GetAttributeAs<Int16>("width"), xNode.GetAttributeAs<Int16>("height"));
        }
            
        private static Transform FindObjByID(Transform parent, string id) {
            for (var i=0; i < parent.childCount; i++){
                if(parent.GetChild(i).name.Contains("Object_" + id)) {
                    return parent.GetChild(i);
                }
            }
            return null;
        }
        #endregion

        /*TODO: write a class for configuration.
         * Could take in a config object. Config object could specify behavior for reading overwritten types in Tiled.
         */
        public static ShadowCaster2D ConfigureShadowCaster(ShadowCaster2D caster, bool selfShadows = true) {
            caster.selfShadows = selfShadows;
            return caster;
        }
        
        public static void AddFreeformLightPrefab(GameObject g, GameObject lightPrefab, Vector3[] points) {
            GameObject instance = AddPrefabAsChild(g, lightPrefab);
            Light2D l = instance.GetRequiredComponent<Light2D>();
            l.SetShapePath(points);
        }

        public static void SetMaterial(GameObject g, string materialName)
        {
            Material mat = Resources.Load(materialName) as Material;

            if (mat == null)
            {
                Debug.LogError($"Could not find material {materialName}");
                return;
            }
            g.GetComponent<TilemapRenderer>().material = mat;
        }

        public static void SetLayer(GameObject g, String layerName) {
            g.layer = LayerMask.NameToLayer(layerName);
        }
        
        public static T GetRequiredComponent<T>(this GameObject g) where T : Component {
            T t = g.GetComponent<T>();
            if (t == null) {
                throw new ConstraintException($"Gameobject {g} has no component {typeof(T)}");
            }
            return t;
        }

        public static void SetSortingLayer(this Renderer tr, string layer) => tr.sortingLayerName = layer;
    }
}