using UnityEditor;
using UnityEngine;

public class MWindow : EditorWindow
{

    private static MWindow window;


    [MenuItem("CustomWindows/MWindow")]
    public static void OpenWindow()
    {
        window = GetWindow<MWindow>("Custom Window");
        window.Show();
    }

    int countPress = 0;

    private GUIContent content = new GUIContent("DropdownButton");

    private GameObject m_objectValue;
    private string m_textValue;
    private float m_floatValue;
    private Vector2 m_vec2;
    private Vector3 m_vec3;
    private Vector4 m_vec4;
    private Bounds m_bounds;
    private BoundsInt m_boundsInt;

    private int m_layer;
    private string m_tag;

    private Color m_color;
    private GUIContent colorTitle = new GUIContent("Color Filed");

    private AnimationCurve m_curve = AnimationCurve.Linear(0, 0, 1, 1);

    private enum TutorialEnum
    {
        One,
        Two,
        Three,
        None
    }

    private TutorialEnum m_enumValue = TutorialEnum.None;

    private enum TutorialEnumM
    {
        None = 0,
        OneAndTwo = One | Two,
        One = 1 << 0,
        Two = 1 << 1,
        Three = 1 << 2
    }

    private TutorialEnumM m_enum;

    private int m_singleInt;
    private string[] intSelections = new string[] { "整数10", "整数20", "整数30" };
    private int[] intValues = new int[] { 10, 20, 30 };

    private int m_multiInt;
    private string[] intMultiSelections = new string[] { "1号", "2号", "3号" };

    private bool foldOut;
    private bool foldOut1;

    private bool toggle;

    private bool toggle1;
    private string m_inputText;

    private float m_sliderValue;
    private int m_sliderIntValue;

    private float m_leftValue;
    private float m_rightValue;

    private Vector2 scrollRoot;
    private void OnGUI()
    {

        scrollRoot = EditorGUILayout.BeginScrollView(scrollRoot);

        // 按下抬起触发
        if (GUILayout.Button("Buttom"))
        {
            countPress++;
            Debug.Log("Press Buttom" + countPress);
        }

        // 按下触发
        if (EditorGUILayout.DropdownButton(content, FocusType.Passive))
        {
            countPress++;
            Debug.Log(countPress);
        }

        m_objectValue = EditorGUILayout.ObjectField(m_objectValue, typeof(GameObject), true) as GameObject;

        foldOut = EditorGUILayout.Foldout(foldOut, "Foldout Example");

        foldOut1 = EditorGUILayout.BeginFoldoutHeaderGroup(foldOut1, "折叠栏组");

        if (foldOut1)
        {
            EditorGUILayout.LabelField("Foldout Content", "This is the content inside the foldout.");
            countPress = EditorGUILayout.IntField("整型输入框", countPress);
            m_floatValue = EditorGUILayout.FloatField("Float 输入：", m_floatValue);
            m_textValue = EditorGUILayout.TextField("Text输入：", m_textValue);
            m_vec2 = EditorGUILayout.Vector2Field("Vec2输入： ", m_vec2);
            m_vec3 = EditorGUILayout.Vector3Field("Vec3输入： ", m_vec3);
            m_vec4 = EditorGUILayout.Vector4Field("Vec4输入： ", m_vec4);
            m_bounds = EditorGUILayout.BoundsField("Bounds输入： ", m_bounds);
            m_boundsInt = EditorGUILayout.BoundsIntField("Bounds输入： ", m_boundsInt);
            EditorGUILayout.LabelField("文本标题", "文本内容");

            m_layer = EditorGUILayout.LayerField("层级选择", m_layer);
            m_tag = EditorGUILayout.TagField("标签选择", m_tag);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        toggle = EditorGUILayout.Toggle("Normal Toggle", toggle);
        if (toggle)
        {
            toggle = EditorGUILayout.ToggleLeft("Left Toggle", toggle);
        }

        m_color = EditorGUILayout.ColorField(colorTitle, m_color, false, false, false);

        m_curve = EditorGUILayout.CurveField("动画曲线", m_curve);

        m_enumValue = (TutorialEnum)EditorGUILayout.EnumPopup("枚举选择", m_enumValue);
        m_enum = (TutorialEnumM)EditorGUILayout.EnumFlagsField("枚举选择", m_enum);

        m_singleInt = EditorGUILayout.IntPopup("单选框", m_singleInt, intSelections, intValues);
        EditorGUILayout.LabelField($"m_singleInt is {m_singleInt}");
        m_multiInt = EditorGUILayout.MaskField("整数多选框", m_multiInt, intMultiSelections);
        EditorGUILayout.LabelField($"m_multiInt is {m_multiInt}");


        toggle1 = EditorGUILayout.BeginToggleGroup("Toggle Group", toggle1);
        EditorGUILayout.LabelField("-------Input Field-------");
        m_inputText = EditorGUILayout.TextField("输入文本：", m_inputText);
        EditorGUILayout.EndToggleGroup();

        m_sliderValue = EditorGUILayout.Slider("滑动条Sample：", m_sliderValue, 0.123f, 7.77f);
        m_sliderIntValue = EditorGUILayout.IntSlider("整型滑动条Sample：", m_sliderIntValue, 0, 100);

        EditorGUILayout.MinMaxSlider("双块滑动条", ref m_leftValue, ref m_rightValue, 0.25f, 10.25f);
        EditorGUILayout.FloatField("滑动左值：", m_leftValue);
        EditorGUILayout.FloatField("滑动右值：", m_rightValue);

        EditorGUILayout.HelpBox("一般提示，你应该这样做...", MessageType.Info);
        EditorGUILayout.HelpBox("警告提示，你可能需要这样做...", MessageType.Warning);
        EditorGUILayout.HelpBox("错误提示，你不能这样做...", MessageType.Error);

        EditorGUILayout.LabelField("----上面的Label----");
        EditorGUILayout.Space(10); //进行10个单位的间隔
        EditorGUILayout.LabelField("----下面的Label----");

        EditorGUILayout.BeginHorizontal(); //开始水平布局
        GUILayout.Button("1号button", GUILayout.Width(100)); //固定button长度： 200
        GUILayout.FlexibleSpace(); // 自动填充间隔，如果窗口宽600px，那这种写法就是：【左button：200px】【自动间隔：200px】【右button ：200px】
        GUILayout.Button("2号button", GUILayout.Width(200));//固定button长度： 200
        EditorGUILayout.EndHorizontal(); //结束水平布局

        EditorGUILayout.EndScrollView();
    }
}