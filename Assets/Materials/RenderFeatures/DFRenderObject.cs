using System.Collections;
using System.Collections.Generic;
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

    // never actually used, hide for now
    [SerializeField, HideInInspector] int overrideMaterialPassIndex = 0;

    [SerializeField] string passTag;
    DFRenderObjectPass pass;

    class DFRenderObjectPass : ScriptableRenderPass
    {
        Material _m;
        FilteringSettings fSettings;
        List<ShaderTagId> m_ShaderTagIds;
        RenderStateBlock m_RenderStateBlock;
        ProfilingSampler m_ProfilingSampler;
        RenderTexture rt;
        int overrideMaterialPassIndex;
        bool clear;
        bool useURPLit;

        public DFRenderObjectPass(
            string passTag,
            LayerMask layerMask,
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
            m_ProfilingSampler = new ProfilingSampler(passTag);

            _m = overrideMaterial;
            renderPassEvent = trigger;
            fSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);

            m_ShaderTagIds = new List<ShaderTagId>() {
                new ShaderTagId("SRPDefaultUnlit"),
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("Universal2D") // added on top of tutorial, comes from URP Hidden/Sprite-Lit-Default source
                };

            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

            this.useURPLit = useURPLit;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var criteria = SortingCriteria.CommonOpaque;

            var drawingSettings = CreateDrawingSettings(m_ShaderTagIds, ref renderingData, criteria);
            drawingSettings.overrideMaterial = _m;
            drawingSettings.overrideMaterialPassIndex = overrideMaterialPassIndex;
            if (useURPLit)
            {
                drawingSettings.overrideMaterial.EnableKeyword("USE_SHAPE_LIGHT_TYPE_0");
            }

            CommandBuffer cmd = CommandBufferPool.Get(nameof(DFRenderObjectPass));
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

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref fSettings, ref m_RenderStateBlock);

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
            passTag,
            layerMask,
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
