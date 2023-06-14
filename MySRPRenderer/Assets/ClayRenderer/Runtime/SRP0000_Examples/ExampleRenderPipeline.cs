using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ClayRenderer.Runtime.SRP0000_Examples
{
    public class ExampleRenderPipeline : RenderPipeline
    {
        private ExampleRenderPipelineAsset renderPipelineAsset;
        public ExampleRenderPipeline(ExampleRenderPipelineAsset asset)
        {
            renderPipelineAsset = asset;
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            Render(context, new List<Camera>(cameras));
        }
        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            // 現在のレンダーターゲットを消去するコマンドを作成してスケジューリングします
            var cmd = new CommandBuffer();
            cmd.ClearRenderTarget(true, true, Color.black);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();

            // すべてのカメラに繰り返します
            foreach (Camera camera in cameras)
            {
                // 現在のカメラからカリングパラメーターを取得します
                camera.TryGetCullingParameters(out var cullingParameters);

                // カリングパラメーターを使用してカリング操作を実行し、結果を保存します
                var cullingResults = context.Cull(ref cullingParameters);

                // 現在のカメラに基づいて、ビルトインのシェーダー変数の値を更新します
                context.SetupCameraProperties(camera);

                // LightMode パスタグの値に基づいて、 Unity に描画するジオメトリを指示します
                ShaderTagId shaderTagId = new ShaderTagId("ExampleLightModeTag");

                //現在のカメラに基づいて、Unity にジオメトリを並べ替える方法を指示します
                var sortingSettings = new SortingSettings(camera);

                // どのジオメトリを描画するかとその描画方法を説明する DrawingSettings 構造体を作成します
                DrawingSettings drawingSettings = new DrawingSettings(shaderTagId, sortingSettings);

                // カリング結果をフィルタリングする方法を Unity に指示し、描画するジオメトリをさらに指定します
                // FilteringSettings.defaultValue を使用して、フィルタリングなしを指定します
                FilteringSettings filteringSettings = FilteringSettings.defaultValue;

                // 定義した設定に基づいて、ジオメトリを描画するコマンドをスケジューリングします
                context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

                // 必要に応じて、スカイボックスを描画するコマンドをスケジューリングします
                if (camera.clearFlags == CameraClearFlags.Skybox && RenderSettings.skybox != null)
                {
                    context.DrawSkybox(camera);
                }

                // スケジュールされたすべてのコマンドを実行するようにグラフィックス API に指示します
                context.Submit();
            }
        }
    }
}