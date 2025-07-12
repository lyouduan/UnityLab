using System.Security;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SimpleScriptableObject), true)]
public class ScriptableObjectInspector : Editor
{
    public SimpleScriptableObject simpleScriptableObject;

    private void OnEnable()
    {
        simpleScriptableObject = target as SimpleScriptableObject;
    }

    private float value;

    private Texture tex;

    private string path;

    private string texPath = "Assets/Source/Default_normal.jpg";
    private Texture m_Tex;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Add Process"))
        {
            value += 0.1f;
            value = Mathf.Clamp01(value); // 限制在0到1之间
            EditorUtility.DisplayProgressBar("进度条", "显示信息", value);
            if (value == 1)
            {
                EditorUtility.ClearProgressBar(); // 清除进度条
            }
        }
        
        if (GUILayout.Button("Save"))
        {

            EditorUtility.SetDirty(simpleScriptableObject); //标记脏数据
            AssetDatabase.SaveAssets();

            var flag = EditorUtility.DisplayDialog("保存资源", "成功保存资源", "OK");

        }
        if (GUILayout.Button("查找法线贴图"))
        {
             EditorGUIUtility.ShowObjectPicker<Texture>(tex, false, "normal", 0);
        }

        tex = EditorGUILayout.ObjectField("法线贴图", tex, typeof(Texture), false) as Texture;
        if(GUILayout.Button("获取路径"))
        {
            path = AssetDatabase.GetAssetPath(tex);
        }
        EditorGUILayout.LabelField("资产路径是", path);

        EditorGUILayout.LabelField("法线贴图路径", texPath);
        if(GUILayout.Button("加载法线贴图"))
        {
            m_Tex = AssetDatabase.LoadAssetAtPath<Texture>(texPath);
        }
        EditorGUILayout.ObjectField(m_Tex, typeof(Texture), false);
    }

}

