using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

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