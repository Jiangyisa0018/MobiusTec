using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public Transform player; // 玩家对象
    public float Xspeed = 3f; // 水平速度
    public float Yspeed = 3f; // 垂直速度
    public float UpVer = -85f; // 向上角度
    public float DownVer = 85f; // 向下角度

    private float rotVer; // 垂直角度

    void Start()
    {
        rotVer = transform.localEulerAngles.x;
    }

    void Update()
    {
        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");

        // 控制上下
        rotVer -= MouseY * Yspeed;
        rotVer = Mathf.Clamp(rotVer, UpVer, DownVer);

        transform.localEulerAngles = new Vector3(rotVer, 0, 0); // 控制相机上下移动玩家不动

        // 控制水平
        player.Rotate(Vector3.up * MouseX * Xspeed);
    }
}