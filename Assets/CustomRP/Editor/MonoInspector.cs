using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TutorialMono), true)]
public class MMonoInspector : Editor
{
    //private TutorialMono m_target;
    public TutorialMono m_target;

    private void OnEnable()
    {
        m_target = target as TutorialMono;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Toggle Gizmos"))
        {
        }
    }
}

