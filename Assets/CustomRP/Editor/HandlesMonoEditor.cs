using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandlesMono))]
public class HandlesMonoEditor : Editor
{
    private void OnSceneGUI()
    {
        //Debug.Log("在Editor OnSceneGUI中 调用....");
    }
}