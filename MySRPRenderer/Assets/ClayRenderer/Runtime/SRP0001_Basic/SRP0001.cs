using System.Collections.Generic;
using ClayRenderer.Runtime.General;
using UnityEngine;
using UnityEngine.Rendering;

namespace ClayRenderer.Runtime.SRP0001_Basic
{
    // 参考サイト: https://github.com/cinight/CustomSRP
    [CreateAssetMenu(menuName = "ClayRenderer/SRP0001_Basic")]
    public class CustomRenderPipelineAsset : RenderPipelineAsset
    {
        protected override RenderPipeline CreatePipeline()
        {
            return new CustomRenderPipeline();
        }
    }

    public class CustomRenderPipeline : RenderPipeline
    {
        private static readonly ShaderTagId PassName = new ShaderTagId("SRP0001_Pass");

        // コンストラクタ
        public CustomRenderPipeline()
        {
        }

        // 後方互換性のためにoverrideしておく
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            Render(context, new List<Camera>(cameras));
        }

        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            // Call the RenderPipelineManger.beginContextRendering and RenderPipelineManger.beginCameraRendering delegates.
            // NOTE: 昔はBeginFrameRendering()を呼び出していたが、パフォーマンス上の問題からこのメソッドを使う方がよい
            BeginContextRendering(context, cameras);

            foreach (var camera in cameras)
            {
                // call RenderPipelineManager.beginCameraRendering delegate.
                BeginCameraRendering(context, camera);

                // Culling
                ScriptableCullingParameters cullingParameters;
                if (!camera.TryGetCullingParameters(out cullingParameters))
                {
                    continue;
                }

                CullingResults cullingResults = context.Cull(ref cullingParameters);

                // builtin variablesをカメラごとにセットアップする
                // e.g. camera projection matrix etc...
                context.SetupCameraProperties(camera);

                // カメラコンポーネントから必要な設定を取得する
                var clearFlags = camera.clearFlags;
                bool drawSkybox = (clearFlags == CameraClearFlags.Skybox);
                bool clearDepth = (clearFlags != CameraClearFlags.Nothing);
                bool clearColor = (clearFlags == CameraClearFlags.Color);

                // カメラを初期化する
                CommandBuffer cmd = new CommandBuffer();
                cmd.name = "Clear Color";
                cmd.ClearRenderTarget(clearDepth, clearColor, camera.backgroundColor);
                context.ExecuteCommandBuffer(cmd);
                cmd.Release();

                // オブジェクトのソート設定
                var sortingSettings = new SortingSettings(camera);
                // DrawingSettings オブジェクトのソート方法とシェーダパス名を記述する構造体
                DrawingSettings drawingSettings = new DrawingSettings(PassName, sortingSettings);
                // ScriptableRenderContext.DrawRenderersに渡す描画フィルタリング設定
                // RenderQueueRangeが示す範囲内にあるオブジェクトを描画する
                FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);

                // Skybox
                if (drawSkybox)
                {
                    RenderUtils.RenderSkybox(context, camera);
                }

                // 不透明オブジェクトを描画する
                sortingSettings.criteria = SortingCriteria.CommonOpaque;
                drawingSettings.sortingSettings = sortingSettings;
                filteringSettings.renderQueueRange = RenderQueueRange.opaque;
                RenderUtils.RenderObjects(
                    "Render Opaque Objects",
                    context, cullingResults, filteringSettings, drawingSettings);

                // 半透明オブジェクトを描画する
                sortingSettings.criteria = SortingCriteria.CommonTransparent;
                drawingSettings.sortingSettings = sortingSettings;
                filteringSettings.renderQueueRange = RenderQueueRange.transparent;
                RenderUtils.RenderObjects(
                    "Render Transparent Objects",
                    context, cullingResults, filteringSettings, drawingSettings);

                context.Submit();

                EndCameraRendering(context, camera);
            }

            EndContextRendering(context, cameras);
        }
    }
}