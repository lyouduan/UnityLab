using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CullingUtils
{
    public static Vector4 GetPlane(Vector3 normal, Vector3 point)
    {
        // 计算平面方程的系数
        float d = -Vector3.Dot(normal, point);
        return new Vector4(normal.x, normal.y, normal.z, d);
    }

    public static Vector4 GetPlane(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
        return GetPlane(normal, a);
    }
    public static Vector3[] GetCameraFarClipPlanePoint(Camera camera)
    {
        Vector3[] points = new Vector3[4];
        Transform transform = camera.transform;
        float farClipPlane = camera.farClipPlane;
        float aspect = camera.aspect;
        float halfFovY = camera.fieldOfView * Mathf.Deg2Rad * 0.5f;
        float hight = Mathf.Tan(halfFovY) * farClipPlane;
        float width = hight * aspect;

        Vector3 farClipCenter = transform.position + transform.forward * farClipPlane;
        Vector3 up = transform.up * hight;
        Vector3 right = transform.right * width;

        // 计算四个角点
        points[0] = farClipCenter - right + up; // 左上角
        points[1] = farClipCenter + right + up; // 右上角
        points[2] = farClipCenter + right - up; // 右下角
        points[3] = farClipCenter - right - up; // 左下角 
        return points;
    }
  
    public static Vector4[] GetFrustumPlane(Camera camera)
    {
        Vector4[] frustumPlane =new Vector4[6];

        Transform transform = camera.transform;
        Vector3 cameraPosition = transform.position;
        Vector3[] points = GetCameraFarClipPlanePoint(camera);

        frustumPlane[0] = GetPlane(cameraPosition, points[3], points[0]);
        frustumPlane[1] = GetPlane(cameraPosition, points[1], points[2]);
        frustumPlane[2] = GetPlane(cameraPosition, points[0], points[1]);
        frustumPlane[3] = GetPlane(cameraPosition, points[2], points[3]);
        frustumPlane[4] = GetPlane(-transform.forward, transform.position + transform.forward * camera.nearClipPlane); // unity camera sapce obey OpenGL right-hand croodinates
        frustumPlane[5] = GetPlane(transform.forward, transform.position + transform.forward * camera.farClipPlane);
        return frustumPlane;
    }
}
