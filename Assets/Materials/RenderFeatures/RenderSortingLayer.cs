using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEditor.SceneView;

public class RenderSortingLayer : ScriptableRendererFeature
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] RenderPassEvent trigger;

    // default values synced to RenderObjects feature
    [SerializeField] Material overrideMaterial = null;
    [SerializeField] int overrideMaterialPassIndex = 0;

    [SerializeField] string passTag;
    RenderSortingLayerPass pass;

    class RenderSortingLayerPass : ScriptableRenderPass
    {
        Material _m;
        FilteringSettings fSettings;
        List<ShaderTagId> m_ShaderTagIds;
        RenderStateBlock m_RenderStateBlock;
        ProfilingSampler m_ProfilingSampler;
        int overrideMaterialPassIndex;

        public RenderSortingLayerPass(string passTag, LayerMask layerMask, RenderPassEvent trigger, Material overrideMaterial, int overrideMaterialPassIndex)
        {
            base.profilingSampler = new ProfilingSampler(nameof(RenderSortingLayerPass)); // no idea what this does. lol
            m_ProfilingSampler = new ProfilingSampler(passTag);

            _m = overrideMaterial;
            renderPassEvent = trigger;
            fSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);

            m_ShaderTagIds = new List<ShaderTagId>() {
                new ShaderTagId("SRPDefaultUnlit"),
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly")
                };

            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var criteria = SortingCriteria.SortingLayer | SortingCriteria.CommonOpaque; // the URP RenderObjects feature uses CommonOpaque only, here we add sorting layer to support tilemaps

            var drawingSettings = CreateDrawingSettings(m_ShaderTagIds, ref renderingData, criteria);
            drawingSettings.overrideMaterial = _m;
            drawingSettings.overrideMaterialPassIndex = overrideMaterialPassIndex;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                // flush buffer
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref fSettings, ref m_RenderStateBlock);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public override void Create()
    {
        pass = new RenderSortingLayerPass(passTag, layerMask, trigger, overrideMaterial, overrideMaterialPassIndex);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}
