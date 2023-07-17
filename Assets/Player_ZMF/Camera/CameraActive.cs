using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections;
public class CameraActive : MonoBehaviour
{
    private new Camera camera;
    private GameObject player;
    private Vector3 defaultPosition;
    private Vector3 initialCameraPosition;
    [SerializeField]private bool isMoving = false; // ����ƶ�״̬
    [SerializeField] float time = 0.05f;
    Vector3 _targetMove;
    [SerializeField]bool isShaking;
    public float shakeRange = 1f; // ����ζ���Χ

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

                // ����һ������ƶ�������������ָ����Χ��
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

                // ������ͣ�£�����ص���ʼλ��
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