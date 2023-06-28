using UnityEngine;
using UnityEngine.Rendering;

namespace ClayRenderer.Runtime.General
{
    public class RendererSwitcher : MonoBehaviour
    {
        public RenderPipelineAsset renderPipelineAsset;

        [ContextMenu("Switch Renderer")]
        public void SwitchRenderer()
        {
            GraphicsSettings.renderPipelineAsset = renderPipelineAsset;
        }
    }
}