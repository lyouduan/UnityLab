using UnityEngine;
using UnityEngine.Rendering;

public class ComputeShaderTest: MonoBehaviour
{
    public ComputeShader computeShader;

    public Material mat;

    RenderTexture renderTexture;


    private void Start()
    {
        renderTexture = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGBFloat) { enableRandomWrite = true };
        renderTexture.Create();
        mat.mainTexture = renderTexture;

        int kernelIndex = computeShader.FindKernel("CSMain");
        computeShader.SetTexture(kernelIndex, "Result", renderTexture);
        computeShader.Dispatch(kernelIndex, renderTexture.width / 8, renderTexture.height / 8, 1);
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}