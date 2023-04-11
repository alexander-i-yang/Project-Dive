using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using Helpers;
using Player;
using World;
// using UnityEngine.Rendering.Universal;
using UnityEditor;

namespace VFX
{
    public class LiquidSimulation : MonoBehaviour, IFilterLoggerTarget
    {
        [Header("Material Prototypes")]
        [SerializeField] Material SimMat;
        private Material m_SimMat;
        [SerializeField] List<Material> SimListenerMats;
        // private List<Material> m_SimListenerMats;
        [SerializeField, Range(0f, 1f)] private float impulseStrength = 0f;

        // [SerializeField] ScriptableRendererData scriptableRenderer;

        [Header("Debug")]
        [SerializeField] RawImage _testImage;

        private CustomRenderTexture _SimTex;
        public CustomRenderTexture SimTex => _SimTex;

        private Room CurrRoom => PlayerCore.SpawnManager.CurrentRoom;

        private void Awake()
        {
            m_SimMat = InstantiateMaterial(SimMat);

            // AT: see OnDestroy()
            // InstantiateMaterials();
        }

        private void OnEnable()
        {
            Room.RoomTransitionEvent += OnRoomTransition;
        }

        private void OnDisable()
        {
            Room.RoomTransitionEvent -= OnRoomTransition;
        }

        private void Update()
        {
            if (_SimTex != null)
            {
                if (_testImage != null)
                {
                    _testImage.texture = _SimTex;
                }

                float velMag = PlayerCore.Actor.velocity.magnitude;
                if (CurrRoom != null && velMag > 1f)
                {
                    //Note: y is flipped because it is the upper left corner.
                    float impulseU = (PlayerCore.Actor.transform.position.x - CurrRoom.transform.position.x) / _SimTex.width;
                    float impulseV = (CurrRoom.transform.position.y - PlayerCore.Actor.transform.position.y) / _SimTex.height;
                    FilterLogger.Log(this, $"Impulse Strength: {impulseStrength * velMag / 256}");
                    m_SimMat.SetVector("_Impulse", new Vector3(impulseU, impulseV, impulseStrength * velMag / 256));
                }
                else
                {
                    m_SimMat.SetVector("_Impulse", new Vector3(0, 0, 0));
                }

            }
        }

        private void OnDestroy()
        {
            FilterLogger.Log(this, "Clear material edits");
            foreach(var m in SimListenerMats)
            {
                m.SetVector("_Impulse", new Vector3(0, 0, 0));
                m.SetVector("_RoomSize", new Vector2(0, 0));
                m.SetVector("_RoomPos", new Vector3(0, 0, 0));
                m.SetTexture("_SimulationTex", null);

#if UNITY_EDITOR
                EditorUtility.SetDirty(m);
                AssetDatabase.SaveAssetIfDirty(m);
#endif
            }
        }

        private void OnRoomTransition(Room roomEntering)
        {
            Bounds bounds = roomEntering.GetComponent<Collider2D>().bounds;

            _SimTex = CreateLavaSimTexture((int)bounds.extents.x * 2, (int)bounds.extents.y * 2);

            foreach(var m in SimListenerMats)
            {
                m.SetVector("_RoomPos", roomEntering.transform.position);
                m.SetVector("_RoomSize", new Vector2(_SimTex.width, _SimTex.height));
                m.SetTexture("_SimulationTex", _SimTex);
            }
        }

        private CustomRenderTexture CreateLavaSimTexture(int width, int height)
        {
            CustomRenderTexture tex = new CustomRenderTexture(width, height);
            tex.material = m_SimMat;

            tex.initializationMode = CustomRenderTextureUpdateMode.OnLoad;
            tex.initializationSource = CustomRenderTextureInitializationSource.TextureAndColor;
            tex.initializationColor = Color.black;

            tex.updateMode = CustomRenderTextureUpdateMode.Realtime;
            tex.doubleBuffered = true;

            return tex;
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Warning;
        }

        //private void InstantiateMaterials()
        //{
        //    m_SimListenerMats = new List<Material>(SimListenerMats.Count);

        //    if (scriptableRenderer == null) throw new System.Exception("Requires reference to the 2D renderer data");

        //    foreach (var m in SimListenerMats)
        //    {
        //        var mInst = InstantiateMaterial(m);
        //        m_SimListenerMats.Add(mInst);

        //    // AT: Material instancing does not actually work on render features(sim textures updates will NOT reflect on the actual render)
        //    //     This is because the Editor.SetDirty() methods needs to be called on the render feature after editing
        //    //     But that method needs the UNITY_EDITOR flag to be set
        //    // replace render feature at run time
        //    var rfs = scriptableRenderer.rendererFeatures;
        //        foreach (var rf in rfs)
        //        {
        //            if (rf is DFRenderObject)
        //            {
        //                var roFeature = rf as DFRenderObject;
        //                if (roFeature.OverrideMaterialPrototype == m)
        //                {
        //                    Debug.Log($"Replacing {roFeature.name}'s material prototype {roFeature.OverrideMaterialPrototype.name}");
        //                    roFeature.OverrideMaterialInstance = m;

        //                }
        //            }
        //        }

        //        MaterialFinder.ReplaceMaterial(m, mInst);
        //    }
        //}

        private Material InstantiateMaterial(Material m)
        {
            return new Material(m)
            {
                name = m.name + " (Instantiated)"
            };
        }
    }
}