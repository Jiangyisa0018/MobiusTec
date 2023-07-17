using UnityEngine;

public class Example : MonoBehaviour
{
    private void Start()
    {
        //圆的中心点位置
        Vector3 center = Vector3.zero;
        //圆的半径
        float radius = 3f;
        //添加LineRenderer组件
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        //设置坐标点个数为360个
        lineRenderer.positionCount = 360;
        //将LineRenderer绘制线的宽度 即圆的宽度 设为0.04
        lineRenderer.startWidth = .04f;
        lineRenderer.endWidth = .04f;
        //每一度求得一个在圆上的坐标点
        for (int i = 0; i < 360; i++)
        {
            float x = center.x + radius * Mathf.Cos(i * Mathf.PI / 180f);
            float z = center.z + radius * Mathf.Sin(i * Mathf.PI / 180f);
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
        }
    }
}
