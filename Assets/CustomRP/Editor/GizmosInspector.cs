using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GizmosTutotial), true)]
public class GizmosInspector : Editor
{
    //private TutorialMono m_target;
    public GizmosTutotial m_target;

    private void OnEnable()
    {
        m_target = target as GizmosTutotial;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        m_target.toggle = EditorGUILayout.Toggle("开关", m_target.toggle);
        m_target.AttackToggle = EditorGUILayout.Toggle("辅助攻击", m_target.AttackToggle);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(m_target); // 记录更改
        }
    }
}

