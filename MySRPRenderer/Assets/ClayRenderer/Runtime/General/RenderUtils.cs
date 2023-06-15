using UnityEngine;
using UnityEngine.Rendering;

namespace ClayRenderer.Runtime.General
{
    public static class RenderUtils
    {
        // SRPでSkyboxを描画する
        public static void RenderSkybox(ScriptableRenderContext context, Camera camera)
        {
            RendererList rendererList = context.CreateSkyboxRendererList(camera);
            CommandBuffer cmd = new CommandBuffer();
            cmd.DrawRendererList(rendererList);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();
        }

        public static void RenderObjects(
            string name, ScriptableRenderContext context,
            CullingResults cull, FilteringSettings filteringSettings, DrawingSettings drawingSettings)
        {
            RendererListParams renderListParams = new RendererListParams(cull, drawingSettings, filteringSettings);
            RendererList rendererList = context.CreateRendererList(ref renderListParams);
            CommandBuffer cmd = new CommandBuffer();
            cmd.name = name;
            cmd.DrawRendererList(rendererList);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();
        }
    }
}