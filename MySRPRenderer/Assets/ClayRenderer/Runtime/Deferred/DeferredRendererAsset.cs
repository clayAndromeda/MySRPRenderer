using UnityEngine.Rendering;

namespace ClayRenderer.Runtime.Deferred
{
    public class DeferredRendererAsset : RenderPipelineAsset
    {
        protected override RenderPipeline CreatePipeline()
        {
            return new DeferredRenderer();
        }
    }
}