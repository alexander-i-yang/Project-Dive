using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Helpers;
using UnityEngine;
using UnityEngine.Rendering.Universal;


/// <summary>
/// unitytips: ComponentFromCollider
/// http://diegogiacomelli.com.br/unitytips-shadowcaster2-from-collider-component
/// <remarks>
/// Based on https://forum.unity.com/threads/can-2d-shadow-caster-use-current-sprite-silhouette.861256/
/// </remarks>
/// </summary>
public class ComponentFromCollider {
    private static BindingFlags accessFlagsPrivate =
        BindingFlags.NonPublic | BindingFlags.Instance;

    private static FieldInfo meshField =
        typeof(ShadowCaster2D).GetField("m_Mesh", accessFlagsPrivate);

    private static FieldInfo shapePathField =
        typeof(ShadowCaster2D).GetField("m_ShapePath", accessFlagsPrivate);

    private static MethodInfo onEnableMethod =
        typeof(ShadowCaster2D).GetMethod("OnEnable", accessFlagsPrivate);

    public static Vector3[] GetColliderPoints(EdgeCollider2D polygon) {
        return Helper.ToVector3(GetColliderPointsVec2(polygon));
    }
    
    public static Vector2[] GetColliderPointsVec2(EdgeCollider2D polygon) {
        // For some reason Tiled always stores the last point twice. Remove it to account for that
        List<Vector2> positions = new();
        for (int i = 0; i < polygon.pointCount-1; i++) {
            positions.Add(
                new Vector2(
                    polygon.points[i].x,
                    polygon.points[i].y)
            );
        }
        return positions.ToArray();
    }

    public static void AddShadowCaster2D(GameObject g, bool selfShadows = true) {
        var addedCaster = g.AddComponent<ShadowCaster2D>();
        addedCaster = g.GetComponent<ShadowCaster2D>();
        addedCaster.selfShadows = selfShadows;
        
        Vector3[] positions = GetColliderPoints(g.GetComponent<EdgeCollider2D>());
        SetCastPoints(addedCaster, positions);
    }

    public static void SetCastPoints(ShadowCaster2D caster, Vector3[] points) {
        shapePathField.SetValue(caster, points);
        meshField.SetValue(caster, null);
        onEnableMethod.Invoke(caster, new object[0]);
    }

    public static void SetPrefabPoints(GameObject prefab, Vector2[] points, bool setSprite = true) {
        Vector2 avg = Helper.ComputeAverage(points);
        points = Helper.NormalizePoints(points, avg);
        prefab.transform.localPosition = avg;

        ShadowCaster2D s = prefab.GetComponent<ShadowCaster2D>();
        if (s != null) SetCastPoints(s, Helper.ToVector3(points));

        SetEdgeCollider2DPoints(prefab, points);
        SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            if (points.Length != 4) {
                throw new ConstraintException($"All objects in layer {prefab.transform.parent.parent} must be rectangular");
            }
            SetSpriteSize(spriteRenderer, points);
        }
    }

    public static void SetEdgeCollider2DPoints(GameObject instance, Vector2[] points) {
        EdgeCollider2D pc2d = instance.GetComponent<EdgeCollider2D>();
        if (pc2d == null) throw new ConstraintException($"Prefab {instance} must have EdgeCollider2D attached");
        
        pc2d.points = points;
    }

    public static void SetSpriteSize(SpriteRenderer sr, Vector2[] points) {
        if (sr == null) return;
        
        Vector2 dimensions = points[0] - points[2];
        dimensions.x = Math.Abs(dimensions.x);
        dimensions.y = Math.Abs(dimensions.y);
        sr.size = dimensions;
    }
}