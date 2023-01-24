using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EarlyOut : ScriptableRendererFeature
{
    [SerializeField] RenderTexture rt;
    [SerializeField] RenderPassEvent trigger;
    [SerializeField] string passTag;
    EarlyOutPass p;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(p);
    }

    public override void Create()
    {
        p = new EarlyOutPass(passTag, rt, trigger);
    }

    public class EarlyOutPass : ScriptableRenderPass
    {
        RenderTexture rt;
        ProfilingSampler m_ProfilinSampler;

        public EarlyOutPass(string passTag, RenderTexture rt, RenderPassEvent trigger)
        {
            base.profilingSampler = new ProfilingSampler(nameof(EarlyOutPass)); // again, not really sure what this does
            m_ProfilinSampler = new ProfilingSampler(passTag);
            this.rt = rt;
            renderPassEvent = trigger;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var renderer = renderingData.cameraData.renderer;

            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilinSampler))
            {
                cmd.Blit(renderer.cameraColorTarget, rt);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
