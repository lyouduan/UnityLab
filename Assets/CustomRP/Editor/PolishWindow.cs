using UnityEditor;
using UnityEngine;

public class PolishWindow : EditorWindow
{

    public static PolishWindow m_MainWin;

    private GUILayoutOption maxWidth = GUILayout.MaxWidth(120); // 定义一个GUI组件的最大宽度
    private Vector2 scrollRoot;//定义组件ScrollView的滚动Root
    private int page = 1; //当前页数
    private int currentSelectionIndex = -1; //当前选中项的索引
    private int totalPage; // 总页数
    private int[] arrays;
    private bool foldOut; // 折叠状态

    private const int arrayLength = 35;// 定义元素数组长度
    private const int countPerPage = 10; // 定义每页显示的元素数量

    [MenuItem("CustomWindows/PolishWindow")]
    public static void OpenWindow()
    {
        m_MainWin = EditorWindow.GetWindow<PolishWindow>("Polish Window");
        m_MainWin.Show();
    }

    /// <summary>
    /// 当窗口打开时调用
    /// </summary>
    private void OnEnable()
    {
        arrays = new int[arrayLength]; //实例化数组
        totalPage = (int)Mathf.Ceil((float)arrayLength / (float)countPerPage); //计算总页数
        for (int i = 0; i < arrays.Length; i++)
        {
            arrays[i] = Random.Range(0, 100); //进行随机数据填充
        }
    }

    private void OnDisable()
    {
    }

    /// <summary>
    /// Editor面板绘制方法
    /// </summary>
    private void OnGUI()
    {
        scrollRoot = EditorGUILayout.BeginScrollView(scrollRoot);
        foldOut = EditorGUILayout.BeginFoldoutHeaderGroup(foldOut, "折叠栏组");

        if(foldOut)
        {
            EditorGUILayout.BeginVertical("frameBox");
            for (int i = (page - 1) * countPerPage; i < arrays.Length; i++)
            {
                if (i >= page * countPerPage)
                {
                    break; // 如果超过当前页的元素数量，则退出循环
                }

                if (currentSelectionIndex == i)
                {
                    GUI.color = Color.green; // 设置选中项的颜色
                }
                else
                {
                    GUI.color = Color.white;
                }

                EditorGUILayout.BeginHorizontal("box");
                if (GUILayout.Button("Select This One", maxWidth)) //绘制选择按钮，使用maxWidth限制组件宽度
                {
                    currentSelectionIndex = i; //记录选择Index
                }

                EditorGUILayout.LabelField($"Element {i + 1},他的随机值是：{arrays[i]}"); // 显示元素编号
                EditorGUILayout.EndHorizontal(); //结束水平布局

            }

            // 防止currentSelectionIndex == page * countPerPage时，颜色为绿色
            GUI.color = Color.white;

            EditorGUILayout.BeginHorizontal(); //开始水平布局
            EditorGUILayout.LabelField($"当前页数：{page.ToString()} / 总页数：{totalPage}"); //绘制页数信息
          
            if (GUILayout.Button("上一页")) //绘制上一页Button
            {
                page -= 1; //当前页面-1
                page = Mathf.Clamp(page, 1, totalPage); //由于页面数取值是1~totalPage，这里进行一次取值范围约束
            }
            if (GUILayout.Button("下一页")) //绘制下一页Button
            {
                page += 1; //当前页面+1
                page = Mathf.Clamp(page, 1, totalPage); //由于页面数取值是1~totalPage，这里进行一次取值范围约束
            }
            int inputPage = EditorGUILayout.IntField("跳转页码:", page, GUILayout.ExpandWidth(false));
            if (inputPage != page)
            {
                page = Mathf.Clamp(inputPage, 1, totalPage);
            }
            EditorGUILayout.EndHorizontal(); //结束水平布局

            EditorGUILayout.EndVertical(); //结束垂直布局
        }

       
        EditorGUILayout.EndFoldoutHeaderGroup(); //关闭 折叠组
        EditorGUILayout.EndScrollView(); //关闭 滚动视图
    }
}
