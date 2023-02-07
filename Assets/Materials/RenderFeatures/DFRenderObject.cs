using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DFRenderObject : ScriptableRendererFeature
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] RenderPassEvent trigger;

    // render to RT instead of camear color, when chosen
    [SerializeField] RenderTexture overrideTexture = null;
    [SerializeField] bool clearOverrideTextureBefore = false;
    [SerializeField] bool useURPLit = false;

    // default values synced to RenderObjects feature
    [SerializeField] Material overrideMaterial = null;
    public Material OverrideMaterialPrototype => overrideMaterial;
    public Material OverrideMaterialInstance { get; set; }

    [SerializeField] bool useSortingLayer = false;
    [SerializeField] string sortingLayer;
    [SerializeField] int lightBlendMode = 0;

    // never actually used, hide for now
    [SerializeField, HideInInspector] int overrideMaterialPassIndex = 0;
    DFRenderObjectPass pass;
    string passName;

    class DFRenderObjectPass : ScriptableRenderPass
    {
        Material _m;
        FilteringSettings filteringSettings;
        List<ShaderTagId> m_ShaderTagIds;
        RenderStateBlock m_RenderStateBlock;
        ProfilingSampler m_ProfilingSampler;
        RenderTexture rt;
        int overrideMaterialPassIndex;
        bool clear;
        bool useURPLit;
        string urpLightKeyword;
        string passName;

        // No longer relevant, see Execute notes
        // int shapeLightPropertyID, lightTextureID; // CAREFUL: directly copies URP, must keep this in sync if URP updates!

        public DFRenderObjectPass(
            string passName,
            LayerMask layerMask,
            // negative to not use sorting layers as filtering criteria, only consider layer mask
            short sortingLayer,
            int blendMode,
            RenderPassEvent trigger,
            bool cleareOverrideTextureBefore,
            RenderTexture overrideTexture,
            Material overrideMaterial,
            int overrideMaterialPassIndex,
            bool useURPLit)
        {
            rt = overrideTexture;
            clear = cleareOverrideTextureBefore;

            base.profilingSampler = new ProfilingSampler(nameof(DFRenderObjectPass)); // no idea what this does. lol
            this.passName = passName;
            m_ProfilingSampler = new ProfilingSampler(passName);

            _m = overrideMaterial;
            renderPassEvent = trigger;
            filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);

            if (sortingLayer >= 0)
            {
                filteringSettings.sortingLayerRange = new SortingLayerRange(sortingLayer, sortingLayer);
            }

            m_ShaderTagIds = new List<ShaderTagId>() {
                new ShaderTagId("SRPDefaultUnlit"), // from tutorial, probably harmless
                new ShaderTagId("UniversalForward"), // for unlit pass
                new ShaderTagId("UniversalForwardOnly"), // for unlit pass
                new ShaderTagId("Universal2D") // added on top of tutorial, comes from URP Hidden/Sprite-Lit-Default source. this adds the lighting pass
                };

            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

            // lightTextureID = Shader.PropertyToID($"_LightTexture_{ sortingLayer }_{ blendMode }");
            // shapeLightPropertyID = Shader.PropertyToID($"_ShapeLightTexture{ blendMode }");

            this.useURPLit = useURPLit;
            if (useURPLit)
            {
                urpLightKeyword = $"USE_SHAPE_LIGHT_TYPE_{blendMode}";
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var criteria = SortingCriteria.CommonOpaque;

            // Not sure if this must be placed here instead of the constructor
            // The tutorial does it this way but it would be nice if this could be factored out
            var drawingSettings = CreateDrawingSettings(m_ShaderTagIds, ref renderingData, criteria);

            drawingSettings.overrideMaterial = _m;
            drawingSettings.overrideMaterialPassIndex = overrideMaterialPassIndex;
            if (useURPLit)
            {
                drawingSettings.overrideMaterial.EnableKeyword(urpLightKeyword);
            }

            CommandBuffer cmd = CommandBufferPool.Get(passName);
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                // flush buffer
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                if (rt != null)
                {
                    cmd.SetRenderTarget(rt);
                    if (clear)
                    {
                        cmd.ClearRenderTarget(false, true, Color.black);
                    }
                }

                context.ExecuteCommandBuffer(cmd); // DrawRenderers doesn't respect buffer order so the above commands need to be done immediately
                cmd.Clear();

                // AT: originally...
                // In URP's Render2DLightningPass, light is injected by setting the global shader texture in the command before calling DrawRenderers
                // HOWEVER everything's internal and non-writable so we have to resort to copying some code
                // cmd.SetGlobalTexture(shapeLightPropertyID, new RenderTargetIdentifier(lightTextureID));

                // however non-writable is for a reason. The temporary light texture is discarded by the time the commented line gets called
                // so instead we will just have to deal with DF Render Objects not supporting URP lighting, unless it's the very last sorting layer

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref m_RenderStateBlock);

                // if (rt != null) cmd.SetRenderTarget(renderingData.cameraData.renderer.cameraColorTarget);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public override void Create()
    {
        var m = OverrideMaterialInstance != null ? OverrideMaterialInstance : OverrideMaterialPrototype;
        pass = new DFRenderObjectPass(
            name,
            layerMask,
            // Unity people should really talk to each other
            // The sorting layer value takes in a short cuz of course there won't be that many sorting layers
            // But the .value method returns an int??? Even URP code has to convert that to short explicitly (see Render2DLightingPass)
            (short)(useSortingLayer ? SortingLayer.GetLayerValueFromName(sortingLayer) : -1),
            lightBlendMode,
            trigger,
            clearOverrideTextureBefore,
            overrideTexture,
            m,
            overrideMaterialPassIndex,
            useURPLit);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}
