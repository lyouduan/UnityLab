using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class ExampleClass : MonoBehaviour
{
    public int instanceCount = 100000;
    public Mesh instanceMesh;
    public Material instanceMaterial;
    public int subMeshIndex = 0;

    private int cachedInstanceCount = -1;
    private int cachedSubMeshIndex = -1;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    public ComputeShader computeShader;
    private ComputeBuffer cullResult;
    ComputeBuffer localToWorldMatrixBuffer;
    List<Matrix4x4> localToWorldMatrixs = new List<Matrix4x4>();
    int kernelId;
    Camera mainCamera;

    // Hi-Z
    public GenerateHiZ generateHiZ;
    int  vpMatrixId, hizTextureId;
    void Start()
    { 

        kernelId = computeShader.FindKernel("FrustumCulling");
        vpMatrixId = Shader.PropertyToID("vpMatrix");
        hizTextureId = Shader.PropertyToID("hizTexture");
        computeShader.SetInt("depthTextureSize", generateHiZ.depthTextureSize);

        cullResult = new ComputeBuffer(instanceCount, sizeof(float)* 16, ComputeBufferType.Append);
        mainCamera = Camera.main;
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();

    }

    void Update()
    {
        // Update starting position buffer
        if (cachedInstanceCount != instanceCount || cachedSubMeshIndex != subMeshIndex)
            UpdateBuffers();

        // Pad input
        if (Input.GetAxisRaw("Horizontal") != 0.0f)
            instanceCount = (int)Mathf.Clamp(instanceCount + Input.GetAxis("Horizontal") * 40000, 1.0f, 5000000.0f);

        Vector4[] planes = CullingUtils.GetFrustumPlane(mainCamera);

        computeShader.SetBuffer(kernelId, "positions", localToWorldMatrixBuffer);
        cullResult.SetCounterValue(0);
        computeShader.SetBuffer(kernelId, "cullingResults", cullResult);
        computeShader.SetVectorArray("planes", planes);
        computeShader.SetInt("instanceCount", instanceCount);
        Matrix4x4 matrix = GL.GetGPUProjectionMatrix(mainCamera.projectionMatrix, false) * mainCamera.worldToCameraMatrix;
        computeShader.SetMatrix("vpMatrix", matrix);
        
        computeShader.SetTexture(kernelId, hizTextureId, generateHiZ.depthTexture);

        computeShader.Dispatch(kernelId, 1 + (instanceCount / 512), 1, 1);
        instanceMaterial.SetBuffer("positionBuffer", cullResult);

        ComputeBuffer.CopyCount(cullResult, argsBuffer, sizeof(uint));

        // Render
        Graphics.DrawMeshInstancedIndirect(instanceMesh, subMeshIndex, instanceMaterial, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(265, 25, 200, 30), "Instance Count: " + instanceCount.ToString());
        instanceCount = (int)GUI.HorizontalSlider(new Rect(25, 20, 200, 30), (float)instanceCount, 1.0f, 5000000.0f);
    }

    void UpdateBuffers()
    {
        // Ensure submesh index is in range
        if (instanceMesh != null)
            subMeshIndex = Mathf.Clamp(subMeshIndex, 0, instanceMesh.subMeshCount - 1);

        // Positions
        if (positionBuffer != null)
            positionBuffer.Release();

        if (localToWorldMatrixBuffer != null)
            localToWorldMatrixBuffer.Release();

        positionBuffer = new ComputeBuffer(instanceCount, 16);
        localToWorldMatrixBuffer = new ComputeBuffer(instanceCount, 16 * sizeof(float));
        Vector4[] positions = new Vector4[instanceCount];
        for (int i = 0; i < instanceCount; i++)
        {
            float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
            float distance = Random.Range(20.0f, 100.0f);
            float height = Random.Range(-2.0f, 2.0f);
            float size = Random.Range(0.05f, 0.25f);
            positions[i] = new Vector4(Mathf.Sin(angle) * distance, height, Mathf.Cos(angle) * distance, size);
            localToWorldMatrixs.Add(Matrix4x4.TRS(positions[i], Quaternion.identity, new Vector3(size, size, size)));
        }
        positionBuffer.SetData(positions);
        localToWorldMatrixBuffer.SetData(localToWorldMatrixs);

        // Indirect args
        if (instanceMesh != null)
        {
            args[0] = (uint)instanceMesh.GetIndexCount(subMeshIndex);
            args[1] = (uint)instanceCount;
            args[2] = (uint)instanceMesh.GetIndexStart(subMeshIndex);
            args[3] = (uint)instanceMesh.GetBaseVertex(subMeshIndex);
        }
        else
        {
            args[0] = args[1] = args[2] = args[3] = 0;
        }

        argsBuffer.SetData(args);

        cachedInstanceCount = instanceCount;
        cachedSubMeshIndex = subMeshIndex;
    }

    void OnDisable()
    {
        localToWorldMatrixBuffer?.Release();
        localToWorldMatrixBuffer = null;
        
        positionBuffer?.Release();
        positionBuffer = null;

        localToWorldMatrixs.Clear();

        if (cullResult != null)
            cullResult.Release();
        cullResult = null;

        if (argsBuffer != null)
            argsBuffer.Release();
        argsBuffer = null;
    }
}