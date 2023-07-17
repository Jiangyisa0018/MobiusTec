using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmin : MonoBehaviour
{
    [Header("MiniCamera")]public Camera minicamera;
    public Transform player;
    public Transform miniplayerIcon;

    void Update()
    {
        minicamera.transform.position = new Vector3(player.position.x, minicamera.transform.position.y, player.position.z);
        miniplayerIcon.eulerAngles = new Vector3(0, 0, -player.eulerAngles.y);

    }
}
