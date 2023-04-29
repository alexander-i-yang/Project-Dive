#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EditorHelper : EditorWindow {
    public static int sliceWidth = 32;
    public static int sliceHeight = 32;

    public static string importPath = "Assets/Sprites/Conveyors";

    public static string exportPath = "Assets/Animation/Conveyors/";

    [MenuItem("Window/Sprite Animator")]
    static void Init() {
        // Window Set-Up
        EditorHelper window =
            EditorWindow.GetWindow(typeof(EditorHelper), false, "AnimationGenerator", true) as
                EditorHelper;
        window.minSize = new Vector2(200, 200);
        window.maxSize = new Vector2(400, 400);
        window.Show();
    }

    //Show UI
    void OnGUI() {
        sliceWidth = EditorGUILayout.IntField("Slice Width", sliceWidth);
        sliceHeight = EditorGUILayout.IntField("Slice Width", sliceHeight);
        importPath = EditorGUILayout.TextField("Import Path", importPath);
        exportPath = EditorGUILayout.TextField("Export Path", exportPath);

        if (GUILayout.Button("Generate Animation")) {
            SliceSprites();
        }
        else {
            Debug.LogWarning("Forgot to set the textures?");
        }

        Repaint();
    }

    static void SliceSprites() {
        // Find all Texture2Ds that have 'co' in their filename, that are labelled with 'architecture' and are placed in 'MyAwesomeProps' folder
        string[] guids = AssetDatabase.FindAssets("t:texture2D", new[] {importPath});
        Debug.Log("spriteSheets.Length: " + guids.Length);

        for (int z = 0; z < guids.Length; z++) {
            string assetPath = AssetDatabase.GUIDToAssetPath (guids[z]);
            Texture2D spriteSheet = AssetDatabase.LoadAssetAtPath (assetPath, typeof(Texture2D)) as Texture2D;
            string path = AssetDatabase.GetAssetPath(spriteSheet);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            ti.spriteImportMode = SpriteImportMode.Multiple;

            List<SpriteMetaData> newData = new List<SpriteMetaData>();

            for (int i = 0; i < spriteSheet.width; i += sliceWidth) {
                for (int j = spriteSheet.height; j > 0; j -= sliceHeight) {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f, 0.5f);
                    smd.alignment = 9;
                    smd.name = (spriteSheet.height - j) / sliceHeight + ", " + i / sliceWidth;
                    smd.rect = new Rect(i, j - sliceHeight, sliceWidth, sliceHeight);

                    newData.Add(smd);
                }
            }

            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Sprite[] sprites = CutSprites(spriteSheet);
            makeAnimation(spriteSheet.name, sprites);
        }

        Debug.Log("Done Slicing!");
    }

    private static Sprite[] CutSprites(Texture2D spriteSheet) {
        if (!IsAtlas(spriteSheet)) {
            Debug.LogWarning("Unable to proceed, the source texture is not a sprite atlas.");
            return null;
        }

        Sprite[] sprites = null;

        //Proceed to read all sprites from CopyFrom texture and reassign to a TextureImporter for the end result
        UnityEngine.Object[] _objects =
            AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(spriteSheet));

        if (_objects != null && _objects.Length > 0)
            sprites = new Sprite[_objects.Length];

        for (int i = 0; i < _objects.Length; i++) {
            sprites[i] = _objects[i] as Sprite;
        }

        return sprites;
    }

    private static void makeAnimation(String name, Sprite[] sprites) {
        //http://forum.unity3d.com/threads/lack-of-scripting-functionality-for-creating-2d-animation-clips-by-code.212615/
        AnimationClip animClip = new AnimationClip();
        // First you need to create e Editor Curve Binding
        EditorCurveBinding curveBinding = new EditorCurveBinding();

        // I want to change the sprites of the sprite renderer, so I put the typeof(SpriteRenderer) as the binding type.
        curveBinding.type = typeof(SpriteRenderer);
        // Regular path to the gameobject that will be changed (empty string means root)
        curveBinding.path = "";
        // This is the property name to change the sprite of a sprite renderer
        curveBinding.propertyName = "m_Sprite";

        // An array to hold the object keyframes
        List<ObjectReferenceKeyframe> keyFrames = new List<ObjectReferenceKeyframe>();
        for (int i = 0; i < sprites.Length; i++) {
            ObjectReferenceKeyframe add = new ObjectReferenceKeyframe();
            add.value = sprites[i];
            keyFrames.Add(add);
        }

        ObjectReferenceKeyframe first = keyFrames.First();
        keyFrames.Add(first);
        keyFrames.RemoveAt(0);
        
        for (int i = 0; i < keyFrames.Count; i++) {
            var objectReferenceKeyframe = keyFrames[i];
            objectReferenceKeyframe.time = i / 60.0f;
            keyFrames[i] = objectReferenceKeyframe;
        }
        
        AnimationUtility.SetObjectReferenceCurve(animClip, curveBinding, keyFrames.ToArray());
        AssetDatabase.CreateAsset(animClip, exportPath + name + ".anim");
    }

    //Check that the texture is an actual atlas and not a normal texture
    private static bool IsAtlas(Texture2D tex) {
        string _path = AssetDatabase.GetAssetPath(tex);
        TextureImporter _importer = AssetImporter.GetAtPath(_path) as TextureImporter;

        return _importer.textureType == TextureImporterType.Sprite &&
               _importer.spriteImportMode == SpriteImportMode.Multiple;
    }
}
#endif