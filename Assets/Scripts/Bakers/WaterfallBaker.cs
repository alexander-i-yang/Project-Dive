using Bakers;
using UnityEditor;
using UnityEngine;

public class WaterfallBaker : MonoBehaviour, IBaker
{
    public void Bake()
    {
        WaterController[] waterControllers = FindObjectsOfType<WaterController>();
        foreach (var w in waterControllers)
        {
            w.Bake();
            #if UNITY_EDITOR
                EditorUtility.SetDirty(w);
            #endif
        }
    }
}