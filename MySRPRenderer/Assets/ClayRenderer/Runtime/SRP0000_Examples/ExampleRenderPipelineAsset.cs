using UnityEngine;
using UnityEngine.Rendering;

namespace ClayRenderer.Runtime.SRP0000_Examples
{
    [CreateAssetMenu(menuName = "Rendering/ClayRenderer/Example Render Pipeline Asset")]
    public class ExampleRenderPipelineAsset : RenderPipelineAsset
    {
        public Color exampleColor;
        public string exampleString;
        
        // 最初のフレームを描画する前に、Unityが呼び出すメソッド
        // レンダーパイプラインアセットの設定が変更された時には、Unityは今使っているRenderPipelineInstanceを破棄して、
        // 次のフレームを描画するときに再度このメソッドを呼び出す
        protected override RenderPipeline CreatePipeline()
        {
            return new ExampleRenderPipeline(this);
        }
    }
}