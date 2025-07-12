using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{

    RenderTexture gDepth;
    RenderTexture[] gBuffers = new RenderTexture[4];
    RenderTargetIdentifier[] gBufferIDs = new RenderTargetIdentifier[4];

    Lighting lighting = new Lighting();

    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        // set the main camera
        Camera cam = cameras[0];
        context.SetupCameraProperties(cam);

        // 动态创建 gBuffer 和 Depth
        CreateGBuffer(cam.pixelWidth, cam.pixelHeight);
        // 创建光源

        lighting.Setup(context);

        var cmd = new CommandBuffer();
        cmd.name = "GBuffer";
        cmd.SetRenderTarget(gBufferIDs, gDepth);


        // clear the render target
        cmd.ClearRenderTarget(true, true, Color.black);
        context.ExecuteCommandBuffer(cmd);

        // culling
        cam.TryGetCullingParameters(out var cullingParams);
        var cuulingResults = context.Cull(ref cullingParams);

        // config settings
        ShaderTagId shaderTagId = new ShaderTagId("GBuffer");
        SortingSettings sortingSettings = new SortingSettings(cam);
        DrawingSettings drawingSettings = new DrawingSettings(shaderTagId, sortingSettings);
        FilteringSettings filteringSettings = FilteringSettings.defaultValue;

        context.DrawRenderers(cuulingResults, ref drawingSettings, ref filteringSettings);

        LightPass(context, cam);

        context.DrawSkybox(cam);
        if (Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(cam, GizmoSubset.PreImageEffects);
            context.DrawGizmos(cam, GizmoSubset.PostImageEffects);
        }

        context.Submit();

        cmd.Release();
    }

    void LightPass(ScriptableRenderContext context, Camera camera)
    {
        var cmd = new CommandBuffer();
        cmd.name = "LightPass";

        cmd.SetGlobalTexture("_gDepth", gDepth);
        for (int i = 0; i < gBuffers.Length; i++)
        {
            cmd.SetGlobalTexture($"_GT{i}", gBuffers[i]);
        }

        Shader lightShader = Shader.Find("Custom/Light");
        if (lightShader == null)
        {
            Debug.LogError("Custom/Light shader not found!");
            return;
        }
        Material mat = new Material(lightShader);
        cmd.Blit(gBufferIDs[0], BuiltinRenderTextureType.CameraTarget, mat);
        context.ExecuteCommandBuffer(cmd);
    }

    void CreateGBuffer(int width, int height)
    {
        ReleaseBuffers(); // 防止内存泄露

        gDepth = new RenderTexture(width, height, 24, RenderTextureFormat.Depth);
        gDepth.Create();

        for (int i = 0; i < 4; i++)
        {
            gBuffers[i] = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            gBuffers[i].Create();
            gBufferIDs[i] = new RenderTargetIdentifier(gBuffers[i]);
        }
    }
    void ReleaseBuffers()
    {
        gDepth?.Release();
        for (int i = 0; i < gBuffers.Length; i++)
        {
            gBuffers[i]?.Release();
        }
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {

    }
}
