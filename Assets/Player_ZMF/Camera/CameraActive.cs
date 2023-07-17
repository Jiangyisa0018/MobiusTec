using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections;
public class CameraActive : MonoBehaviour
{
    private new Camera camera;
    private GameObject player;
    private Vector3 defaultPosition;
    private Vector3 initialCameraPosition;
    [SerializeField]private bool isMoving = false; // 玩家移动状态
    [SerializeField] float time = 0.05f;
    Vector3 _targetMove;
    [SerializeField]bool isShaking;
    public float shakeRange = 1f; // 相机晃动范围

    void Start()
    {
        camera = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
  
        initialCameraPosition = camera.transform.localPosition;
    }

    void Update()
    {
        RangeMove();
        if(isShaking)
        camera.transform.localPosition = Vector3.Lerp(transform.localPosition, _targetMove, 0.2f);
    }
    private void FixedUpdate()
    {
        defaultPosition = player.transform.position;
    }

    void RangeMove()
    {
        if (!isShaking)
        {
            if (player.transform.position.x != defaultPosition.x && player.transform.position.z != defaultPosition.z && !isMoving)
            {
                isMoving = true;

                // 生成一个随机移动向量并限制在指定范围内
                Vector3 move = new Vector3(
                0,Random.Range(-shakeRange, shakeRange),0
                );
                //camera.transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + move, 0.2f);
                _targetMove = transform.localPosition + move;
                StartCoroutine(InvokeCam());
            }
            else
            {
                isMoving = false;

                // 如果玩家停下，相机回到初始位置
                camera.transform.localPosition = initialCameraPosition;
            }
        }
        Debug.Log(isMoving);
    }

    IEnumerator InvokeCam()
    {
        isShaking = true;
        yield return new WaitForSeconds(time);
        isShaking = false;
    }
}