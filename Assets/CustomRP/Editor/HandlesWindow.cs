using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

public class HandlesWindow : EditorWindow
{

    public static HandlesWindow m_MainWindow;

    [MenuItem("CustomWindows/HandlesWindow")]
    public static void OpenWindow()
    {
        m_MainWindow = EditorWindow.GetWindow<HandlesWindow>();
        m_MainWindow.Show();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        Selection.selectionChanged += OnSelectionChanged;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        Selection.selectionChanged -= OnSelectionChanged;
    }


    private Vector3 labelPos = Vector3.zero;
    private string labelText = "Handles text info";
    private Vector3 root = Vector3.zero;

    private void OnSceneGUI(SceneView sceneView) //自定义刷新事件的委托方法
    {
        //Debug.Log("在 Window OnSceneGUI中 调用...."); //具体逻辑

        Handles.Label(labelPos, labelText);

        //1.from 2.to 3.thickness（厚度）决定线段厚度
        Handles.DrawLine(Vector3.zero, new Vector3(-1, 1, 1), 2f);
        //1.from 2.to 3.ScreenSpaceSize（屏幕空间大小）决定虚线长度
        Handles.DrawDottedLine(Vector3.zero, Vector3.one, 2f);

       /* Handles.color = new Color(1, 0, 0, 0.3f); //提前使用Color进行颜色设置，便于观察
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.right, 90, 2); //绘制线框弧线
        Handles.DrawSolidArc(Vector3.zero, Vector3.up, Vector3.back, 90, 2); // 绘制填充弧线

        Handles.color = new Color(0, 1, 0, 0.3f);
        Handles.DrawSolidDisc(new Vector3(0, 0, 5), Vector3.up, 5); //绘制填充圆环
        Handles.DrawWireDisc(new Vector3(0, 0, -5), Vector3.up, 5); //绘制线框圆环

        Handles.DrawSolidRectangleWithOutline(new Rect(0, 0, 1, 1), Color.blue, Color.red);

        Handles.BeginGUI();
        GUILayout.Label("我是SceneView中的Label");
        if (GUILayout.Button("我是SceneView中的Button"))
        {
            Debug.Log("GUI");
        }
        Handles.EndGUI();*/

        Handles.DrawSolidDisc(root, Vector3.up, 1); //画个圆，假设这是目标对象
        root = Handles.PositionHandle(root, Quaternion.identity); //控制柄
    }

    private string selectedName = "无";
    private Vector3 selectedPosition = Vector3.zero;
    private Transform selectedTransform;

    void OnSelectionChanged()
    {
        if (Selection.activeGameObject != null)
        {
            selectedName = Selection.activeGameObject.name;
            selectedPosition = Selection.activeGameObject.transform.position;
            selectedTransform = Selection.activeTransform;
        }
        else
        {
            selectedName = "无";
            selectedPosition = Vector3.zero;
            selectedTransform = null;
        }   

        // 刷新窗口 UI
        Repaint();
    }

    private void OnGUI()
    {
        labelPos = EditorGUILayout.Vector3Field("文本坐标：", labelPos);
        labelText = EditorGUILayout.TextField("文本标题:", labelText);

        GUILayout.Label("当前选中物体信息", EditorStyles.boldLabel);
        GUILayout.Label("名称：" + selectedName);
        GUILayout.Label("位置：" + selectedPosition);
        GUILayout.Label("变换：" + selectedTransform);

        if (GUILayout.Button("选择一个文件夹，获取下面所有的Texture资源文件"))
        {
            var texs = Selection.GetFiltered<Texture>(SelectionMode.DeepAssets); //进行深度遍历所有文件夹
            Debug.Log($"Tex总计数：{texs.Length}");
            foreach (var tex in texs)
            {
                //使用AssetDatabase获取一次资产路径
                Debug.Log($"Tex 文件名：{tex.name}，文件路径 ：{AssetDatabase.GetAssetPath(tex)}");
            }
        }

        if (GUILayout.Button("创建一个 Cube，同时选中"))
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube); //创建Cube
            obj.name = "New Cube";
            Selection.activeGameObject = obj; //选中Cube
        }

    }
}