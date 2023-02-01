using UnityEngine;
using UnityEditor;

using VFX;

[CustomEditor(typeof(LiquidSimulation))]
public class LavaSimulationEditor : Editor
{
    LiquidSimulation _lavaSimulation;

    Texture2D _texture;

    private void OnEnable()
    {
        //target is by default available for you
        //because we inherite Editor
        _lavaSimulation = target as LiquidSimulation;
    }

    private void OnSceneGUI()
    {
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);

        _texture = AssetPreview.GetAssetPreview(_lavaSimulation.SimTex);
        if (_texture != null)
        {
            //We crate empty space 80x80 (you may need to tweak it to scale better your sprite
            //This allows us to place the image JUST UNDER our default inspector
            //GUILayout.Label("", GUILayout.Height(256), GUILayout.Width(256));
            //Draws the texture where we have defined our Label (empty space)
            GUI.DrawTexture(r, _texture, ScaleMode.StretchToFill, false);
        }
    }

    public override void OnInspectorGUI()
    {
        //Draw whatever we already have in SO definition
        base.OnInspectorGUI();
        //Guard clause

        //Convert the weaponSprite (see SO script) to Texture
    }
}
