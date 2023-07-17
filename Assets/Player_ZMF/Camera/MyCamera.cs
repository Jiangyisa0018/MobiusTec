using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public Transform player; // ��Ҷ���
    public float Xspeed = 3f; // ˮƽ�ٶ�
    public float Yspeed = 3f; // ��ֱ�ٶ�
    public float UpVer = -85f; // ���ϽǶ�
    public float DownVer = 85f; // ���½Ƕ�

    private float rotVer; // ��ֱ�Ƕ�

    void Start()
    {
        rotVer = transform.localEulerAngles.x;
    }

    void Update()
    {
        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");

        // ��������
        rotVer -= MouseY * Yspeed;
        rotVer = Mathf.Clamp(rotVer, UpVer, DownVer);

        transform.localEulerAngles = new Vector3(rotVer, 0, 0); // ������������ƶ���Ҳ���

        // ����ˮƽ
        player.Rotate(Vector3.up * MouseX * Xspeed);
    }
}