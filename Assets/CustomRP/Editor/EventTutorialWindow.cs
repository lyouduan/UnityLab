using UnityEditor;
using UnityEngine;

public class EventTutorialWindow : EditorWindow
{

    public static EventTutorialWindow m_mainWin;

    [MenuItem("CustomWindows/EventTutorialWindow")]
    public static void OpenWindow()
    {
        m_mainWin = GetWindow<EventTutorialWindow>("Event Tutorial Window");
        m_mainWin.Show();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneView;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneView; //移除场景视图刷新回调
    }

    private LayerMask layerMask = -1;

    private void OnSceneView(SceneView sceneView)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            /* if (Event.current.alt) //检测alt键是否被按下
             {
                 Debug.Log("在场景视图中按下了Alt + 鼠标左键");
             }
             else if (Event.current.shift) //检测shift键是否被按下
             {
                 Debug.Log("在场景视图中按下了Shift + 鼠标左键");
             }

             // 在这里处理鼠标点击事件
             // 例如，获取鼠标位置、创建物体等
             Vector2 mousePosition = Event.current.mousePosition;
             Debug.Log($"鼠标位置: {mousePosition}");*/

            var mousePos = Event.current.mousePosition; //获取鼠标在SceneView中的屏幕位置
            Debug.Log($"鼠标位置: {mousePos}");

            var ray = HandleUtility.GUIPointToWorldRay(mousePos); //生成屏幕坐标-->世界坐标的射线
            RaycastHit hit; //定义射线检测结果结构体
            if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask)) //进行射线检测，layerMask是检测层级
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere); //如果检测成功，创建一个球体。
                obj.transform.position = hit.point; //把球体坐标设置为hit交点处。
            }
        }


    }

    private GUIStyle defaultStyle = new GUIStyle(); //定义 GUIStyle
    private void OnGUI()
    {
        defaultStyle.fontSize = 10; //字体大小： 10
        defaultStyle.fontStyle = FontStyle.Normal; //字体样式：normal

        defaultStyle.alignment = TextAnchor.MiddleCenter; //字体对齐方式： 居中对齐
        defaultStyle.normal.textColor = Color.red; //字体颜色： 红色
        EditorGUILayout.LabelField("这是红色字体", defaultStyle); //绘制GUI组件，使用 defaultStyle

        defaultStyle.alignment = TextAnchor.MiddleLeft; //字体对齐方式： 水平靠左，垂直居中
        defaultStyle.normal.textColor = Color.yellow; //字体颜色：黄色
        defaultStyle.fontSize = 20; //字体大小： 20
        EditorGUILayout.LabelField("这是黄色字体", defaultStyle); //绘制GUI组件

        defaultStyle.normal.textColor = Color.green; //字体颜色： 绿色
        defaultStyle.fontSize = 12; //字体大小 ： 12
        defaultStyle.fontStyle = FontStyle.Bold;  //字体样式： Bold（加粗）
        EditorGUILayout.SelectableLabel("这是绿色字体", defaultStyle); //绘制GUI


        GUILayout.Label("Event Tutorial Window", defaultStyle);
        GUILayout.Label("在场景视图中点击鼠标左键会触发事件。", defaultStyle);
        GUILayout.Label("请查看控制台输出。", defaultStyle);

        GUI.color = Color.red;
        GUILayout.Button("红色Button");
        GUI.color = Color.green;
        EditorGUILayout.LabelField("绿色Label");
        GUI.color = Color.yellow;
        EditorGUILayout.IntField("黄色Int Field", 20);

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.A)
        {
            Debug.Log("键盘A键在Window面板中按下....");
        }

        EditorGUILayout.BeginVertical("frameBox");
        EditorGUILayout.LabelField("这是【frameBox】样式，用于显示面板的分区");
        EditorGUILayout.BoundsField("比如这是一个BoundField", new Bounds());
        EditorGUILayout.Slider("比如这是一个Slider", 5, 0, 10);
        EditorGUILayout.TextArea("这是一个【SearchTextField】样式的文本框", "SearchTextField");
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("这是【box】样式，用于显示面板的分区");
        EditorGUILayout.Vector3Field("比如这是一个Vec3 Field", Vector3.one);
        EditorGUILayout.BeginToggleGroup("比如这是一个Toggle Group", false);
        EditorGUILayout.HelpBox("比如这是一个HelpBox", MessageType.Warning);
        EditorGUILayout.TextArea("比如这是一个TextArea");
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
       
        if (GUILayout.Button("Close Window"))
        {
            Close();
        }
    }

}