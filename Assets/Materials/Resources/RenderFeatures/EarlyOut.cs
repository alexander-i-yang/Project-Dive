using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EarlyOut : ScriptableRendererFeature
{
    [SerializeField] RenderTexture rt;
    [SerializeField] RenderPassEvent time;
    EarlyOutPass p;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(p);
    }

    public override void Create()
    {
        p = new EarlyOutPass(rt, time);
    }

    public class EarlyOutPass : ScriptableRenderPass
    {
        RenderTexture rt;
        string m_ProfilerTag = "Early Out";

        public EarlyOutPass(RenderTexture rt, RenderPassEvent time)
        {
            this.rt = rt;
            renderPassEvent = time;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var renderer = renderingData.cameraData.renderer;

            var cmd = CommandBufferPool.Get(m_ProfilerTag);
            using (new ProfilingScope())
            {
                cmd.Blit(renderer.cameraColorTarget, rt);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
