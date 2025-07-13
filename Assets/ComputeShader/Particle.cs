using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public struct ParticleData
    {
        public Vector3 pos; 
        public Color color;
    }

    public ComputeShader computeShader;

    ComputeBuffer buffer;
    public Material material;

    const int mParticleCount = 20000;
    int kernelIndex;

    // Start is called before the first frame update
    void Start()
    {
        buffer = new ComputeBuffer(mParticleCount, 28); // 4 * 7 = 28
        ParticleData[] particleDatas = new ParticleData[mParticleCount];
        
        buffer.SetData(particleDatas);

        if (computeShader == null)
        {
            Debug.LogError("computeShader 未在 Inspector 中赋值！");
            return;
        }
        kernelIndex = computeShader.FindKernel("UpdateParticle");

    }

    // Update is called once per frame
    void Update()
    {
        computeShader.SetBuffer(kernelIndex, "ParticleBuffer", buffer);
        computeShader.SetFloat("time", Time.time);
        computeShader.Dispatch(kernelIndex, mParticleCount / 1000, 1, 1);
        material.SetBuffer("_particleDataBuffer", buffer);
    }
    void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, mParticleCount);
    }

    void OnDestroy()
    {
        buffer.Release();
        buffer = null;
    }
}
