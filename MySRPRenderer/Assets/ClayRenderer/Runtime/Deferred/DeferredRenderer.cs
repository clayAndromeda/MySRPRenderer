using System.Collections.Generic;
using ClayRenderer.Runtime.General;
using UnityEngine;
using UnityEngine.Rendering;

namespace ClayRenderer.Runtime.Deferred
{
    public class DeferredRenderer : RenderPipeline
    {
        // 後方互換性のために実装しておく
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            Render(context, new List<Camera>(cameras));
        }

        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            BeginContextRendering(context, cameras);

            foreach (var camera in cameras)
            {
                BeginCameraRendering(context, camera);
                
                // カリング
                if (!camera.TryGetCullingParameters(out var cullingParameters))
                {
                    continue;
                }
                var cullingResults = context.Cull(ref cullingParameters);
                
                // 組み込み変数をカメラごとにセットする
                // e.g. camera projection matrix etc...
                context.SetupCameraProperties(camera);
                
                // カメラコンポーネントから、描画に必要な設定を取得する
                var clearFlags = camera.clearFlags;
                bool drawSkybox = (clearFlags == CameraClearFlags.Skybox);
                bool clearDepth = (clearFlags != CameraClearFlags.Nothing);
                bool clearColor = (clearFlags == CameraClearFlags.Color);
                
                // カメラの描画先をクリアする
                CommandBuffer cmd = new CommandBuffer();
                cmd.name = "Clear Color";
                cmd.ClearRenderTarget(clearDepth, clearColor, camera.backgroundColor);
                context.ExecuteCommandBuffer(cmd);
                cmd.Release();
                
                // Skybox
                if (drawSkybox)
                {
                    RenderUtils.RenderSkybox(context, camera);
                }
                
                // オブジェクトのソート設定
                var sortingSettings = new SortingSettings(camera);
                // DrawingSettings オブジェクトのソート方法とシェーダパス名を記述する構造体
                DrawingSettings drawingSettings = new DrawingSettings(RendererConstants.PassName, sortingSettings);
                // ScriptableRenderContext.DrawRenderersに渡す描画フィルタリング設定
                // RenderQueueRangeが示す範囲内にあるオブジェクトを描画する
                FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);
                
                // アルベドと法線を出力するためのG-Bufferを確保する
                CommandBuffer gBufferCmd = new CommandBuffer();
                gBufferCmd.name = "Transparent";
                
                context.Submit();
                
                EndCameraRendering(context, camera);
            }
            
            EndContextRendering(context, cameras);
        }
    }
}