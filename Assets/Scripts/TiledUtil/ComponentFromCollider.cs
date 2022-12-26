using System.Reflection;
using SuperTiled2Unity.Editor;
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
        // For some reason Tiled always stores the last point twice. Remove it to account for that
        Vector3[] positions = new Vector3[polygon.points.Length];
        for (int i = 0; i < positions.Length; i++) {
            positions[i] = new Vector3(
                polygon.points[i].x,
                polygon.points[i].y, 
                0);
        }
        return positions;
    }

    public static void AddShadowCaster2D(GameObject g, bool selfShadows = false) {
        var addedCaster = g.AddComponent<ShadowCaster2D>();
        addedCaster = g.GetComponent<ShadowCaster2D>();
        addedCaster.selfShadows = selfShadows;
        
        Vector3[] positions = GetColliderPoints(g.GetComponent<EdgeCollider2D>());
        shapePathField.SetValue(addedCaster, positions);
        meshField.SetValue(addedCaster, null);
        onEnableMethod.Invoke(addedCaster, new object[0]);
    }
}