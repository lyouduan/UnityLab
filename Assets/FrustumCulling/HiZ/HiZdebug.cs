using UnityEngine;

public class HiZPreviewController : MonoBehaviour
{
    public GenerateHiZ hizGenerator;
    public Material hizDebugMat;
    [Range(0, 10)]
    public int mipLevel = 0;

   
    void Update()
    {
        if (hizGenerator != null && hizDebugMat != null)
        {
            hizDebugMat.SetTexture("_MainTex", hizGenerator.depthTexture);
            hizDebugMat.SetFloat("_MipLevel", mipLevel);
        }
    }
}