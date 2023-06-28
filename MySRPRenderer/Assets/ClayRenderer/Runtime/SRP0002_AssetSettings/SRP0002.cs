using System.Collections.Generic;
using ClayRenderer.Runtime.General;
using UnityEngine;
using UnityEngine.Rendering;

namespace ClayRenderer.Runtime.SRP0002_AssetSettings
{
    [CreateAssetMenu(menuName = "ClayRenderer/SRP0002_AssetSettings")]
    public class SRP0002RenderPipelineAsset : RenderPipelineAsset
    {
        public bool drawOpaqueObjects = true;
        public bool drawTransparentObjects = true;
        
        protected override RenderPipeline CreatePipeline()
        {
            return new SRP0002(this);
        }
    }
    public class SRP0002 : RenderPipeline
    {
        private static SRP0002RenderPipelineAsset pipelineAsset;
        // NOTE: SRP0001のシェーダを使い回すので、PassNameは0001のまま
        private static readonly ShaderTagId PassName = new("SRP0001_Pass");

        public SRP0002(SRP0002RenderPipelineAsset asset)
        {
            pipelineAsset = asset;
        }

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
                if (pipelineAsset.drawOpaqueObjects)
                {
                    sortingSettings.criteria = SortingCriteria.CommonOpaque;
                    drawingSettings.sortingSettings = sortingSettings;
                    filteringSettings.renderQueueRange = RenderQueueRange.opaque;
                    RenderUtils.RenderObjects(
                        "Render Opaque Objects",
                        context, cullingResults, filteringSettings, drawingSettings);
                }

                // 半透明オブジェクトを描画する
                if (pipelineAsset.drawTransparentObjects)
                {
                    sortingSettings.criteria = SortingCriteria.CommonTransparent;
                    drawingSettings.sortingSettings = sortingSettings;
                    filteringSettings.renderQueueRange = RenderQueueRange.transparent;
                    RenderUtils.RenderObjects(
                        "Render Transparent Objects",
                        context, cullingResults, filteringSettings, drawingSettings);
                }

                context.Submit();

                EndCameraRendering(context, camera);
            }

            EndContextRendering(context, cameras);
        }
    }
}