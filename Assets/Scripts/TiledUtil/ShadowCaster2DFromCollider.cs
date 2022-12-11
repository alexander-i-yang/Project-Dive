using System.Reflection;
using UnityEngine;


/// <summary>
/// unitytips: ShadowCaster2DFromCollider Component
/// http://diegogiacomelli.com.br/unitytips-shadowcaster2-from-collider-component
/// <remarks>
/// Based on https://forum.unity.com/threads/can-2d-shadow-caster-use-current-sprite-silhouette.861256/
/// </remarks>
/// </summary>
public class ShadowCaster2DFromCollider {
    private static BindingFlags accessFlagsPrivate =
        BindingFlags.NonPublic | BindingFlags.Instance;

    private static FieldInfo meshField =
        typeof(UnityEngine.Rendering.Universal.ShadowCaster2D).GetField("m_Mesh", accessFlagsPrivate);

    private static FieldInfo shapePathField =
        typeof(UnityEngine.Rendering.Universal.ShadowCaster2D).GetField("m_ShapePath", accessFlagsPrivate);

    private static MethodInfo onEnableMethod =
        typeof(UnityEngine.Rendering.Universal.ShadowCaster2D).GetMethod("OnEnable", accessFlagsPrivate);

    public static void Execute(GameObject g, bool selfShadows = false) {
        var addedCaster = g.AddComponent<UnityEngine.Rendering.Universal.ShadowCaster2D>();
        var polygon = g.GetComponent<EdgeCollider2D>();
        addedCaster = g.GetComponent<UnityEngine.Rendering.Universal.ShadowCaster2D>();
        addedCaster.selfShadows = selfShadows;
        
        Vector3[] positions = new Vector3[polygon.points.Length];

        for (int i = 0; i < polygon.points.Length; i++) {
            positions[i] = new Vector3(
                polygon.points[i].x,
                polygon.points[i].y, 
            0);
        }

        shapePathField.SetValue(addedCaster, positions);

        meshField.SetValue(addedCaster, null);

        onEnableMethod.Invoke(addedCaster, new object[0]);
    }
}