using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EarlyOut : ScriptableRendererFeature
{
    [SerializeField] RenderTexture rt;
    [SerializeField] RenderPassEvent trigger;
    EarlyOutPass pass;

    public class EarlyOutPass : ScriptableRenderPass
    {
        RenderTexture rt;
        ProfilingSampler m_ProfilingSampler = new ProfilingSampler(nameof(EarlyOutPass));

        public EarlyOutPass(RenderTexture rt, RenderPassEvent trigger)
        {
            this.rt = rt;
            renderPassEvent = trigger;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera != Camera.main) return;

            var renderer = renderingData.cameraData.renderer;

            var cmd = CommandBufferPool.Get(nameof(EarlyOutPass));
            using(new ProfilingScope(cmd, m_ProfilingSampler))
            { 
                // flush buffer
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                cmd.Blit(renderer.cameraColorTarget, rt); // from the Frame Debugger, this operation actually sets the target of the entire buffer
                cmd.SetRenderTarget(renderer.cameraColorTarget); // this cleans up the target change... BUT it assumes the original target is camera color
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }

    public override void Create()
    {
        pass = new EarlyOutPass(rt, trigger);
    }
}
