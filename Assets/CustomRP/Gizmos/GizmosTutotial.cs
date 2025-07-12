using UnityEngine;

public class GizmosTutotial : MonoBehaviour
{
    [HideInInspector]
    public bool toggle;

    [HideInInspector]
    public bool AttackToggle;

    private Vector3 size = new Vector3(10, 10, 10);

    public Texture teuxtre;

    public Mesh mesh;

    public Transform player;
    public Transform enemy;


    private void OnDrawGizmos()
    {
        if (toggle)
        {
            // draw cube
            //Gizmos.DrawCube(Vector3.one, size);

            // draw frustum
            //Gizmos.DrawFrustum(Vector3.zero, 60, 300, 0.3f, 1.7f);
            //Gizmos.DrawSphere(Vector3.up, 1);

            Gizmos.DrawWireCube(Vector3.left, Vector3.one);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Vector3.right, 1);
            Gizmos.color = Color.green;
            if (mesh != null)
            {
                Gizmos.DrawWireMesh(mesh, 0, Vector3.forward);
            }
            if (teuxtre != null)
            {
                // draw texture
                Gizmos.DrawGUITexture(new Rect(0, 0, 100, 100), teuxtre);
            }

            // draw Icon
            Gizmos.DrawIcon(transform.position, "Default_normal.jpg");

            // draw line
            //Gizmos.DrawLine(Vector3.zero, Vector3.one);
            //Gizmos.DrawRay(Vector3.zero, Vector3.up);
            if (mesh != null)
            {
                Gizmos.DrawMesh(mesh, 0);
            }

            Gizmos.color = Color.blue;
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(Vector3.zero, 1);
        }
        if (AttackToggle)
        {
            if (player != null && enemy != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(player.position, 0.5f);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(enemy.position, 0.5f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(player.position, enemy.position);

                DrawCircle(enemy, 3, 0.1f, Color.red);
                DrawCircle(enemy, 5, 0.1f, Color.green);
                DrawArc(enemy, 1, 90, 0.1f, Color.green);
            }
        }
    }

    private void DrawCircle(Transform transform, float radius, float theta, Color color)
    {
        var matrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = color;
        Vector3 beginPoint = new Vector3(radius, 0, 0);
        Vector3 firstPoint = new Vector3(radius, 0, 0);

        for (float t = 0; t < 2 * Mathf.PI; t += theta)
        {
            float x = radius * Mathf.Cos(t);
            float z = radius * Mathf.Sin(t);
            Vector3 endPoint = new Vector3(x, 0, z);

            Gizmos.DrawLine(beginPoint, endPoint);
            beginPoint = endPoint;
        }

        Gizmos.DrawLine(firstPoint, beginPoint);
        Gizmos.matrix = matrix;
    }

    private void DrawArc(Transform transform, float radius, float angle, float theta, Color color)
    {
        var matrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = color;
        Vector3 beginPoint = Vector3.zero; //定义起始点
        Vector3 firstPoint = Vector3.zero;

        var rad = Mathf.Deg2Rad * angle;
        for (float t = 0; t < rad; t += theta)
        {
            float x = radius * Mathf.Cos(t);
            float z = radius * Mathf.Sin(t);
            Vector3 endPoint = new Vector3(x, 0, z);

            Gizmos.DrawLine(beginPoint, endPoint);
            beginPoint = endPoint;
        }

        Gizmos.DrawLine(firstPoint, beginPoint); // 最后一个点和第一个点连接（原点）
        Gizmos.matrix = matrix;
    }
}
