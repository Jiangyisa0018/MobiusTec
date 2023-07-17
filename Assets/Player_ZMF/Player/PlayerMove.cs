using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{

    public CharacterController characterController;
    public MyCamera Camera;
    [Header("˲ɱ����")]
    [SerializeField] private float FlashDistance = 5f;//˲ɱ����
    [Header("����ٶ�")]
    [SerializeField] private float Speed = 3f;
    [Header("��ը��Χ���ٶ�")]
    [SerializeField][Range(0, 500)] private float BoomSpeed;
    [SerializeField][Range(0, 100)] private float BoomRange;
    //�ӿڵ���
    private PlayerSeach Seach;
    /*����*/
    private Rigidbody rb;
    /*��ֱ����*/
    private Vector3 speedDown;//������ҵ���
    /*�������λ����Ϣ*/
    Transform childTransform;
    /*���ܷ�Χ*/
    public GameObject Sphere;
    /*˲��*/
    private bool IsFalshMove=false;
    /*�Ƿ��ܻ�*/
    private bool IsUseSkill=true;

    /**/
    private Queue<GameObject> enemyQueue = new Queue<GameObject>(); // Э�̶���
    public bool isDeathAnimationPlaying = false; // �ж϶����Ƿ���������
    private void Start()
    {
       rb=GetComponent<Rigidbody>();
       rb.useGravity = false;
       Seach = GetComponentInChildren<PlayerSeach>();
       childTransform = transform.Find("Body");
     
    }
    private void Update()
    {   
        Fall();
        Walk();
        FlashMove();
        UsedBoom();
    }

    private void Walk()
    {
        /*�����������*/
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");


        Vector3 Playermovement = new Vector3(horizontalInput, 0, verticalInput) * Speed * Time.deltaTime;

        /*��ȡ���λ����������ת����*/
        Quaternion cameraRotation = Camera.transform.rotation;
        Vector3 cameraForward = cameraRotation * Vector3.forward;
        Vector3 cameraRight = cameraRotation * Vector3.right;
        /*����y�ᣬ�����������Ǹ����������������һ������ǰ��һ����������*/
        cameraForward.y = 0f;
        cameraForward.Normalize();
        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 movement = cameraRight * Playermovement.x + cameraForward * Playermovement.z;

        characterController.Move(movement);
    }


    /*˲ɱ*/
   private void FlashMove()
    {
        Debug.Log(Seach.dashSuccess);
        if (Input.GetKeyDown(KeyCode.Space)&&(Seach.dashSuccess&& Seach.IsAttack )||Seach.hitSuccess)
        {
            if (childTransform != null)
            {   
                // ���������ʹ���µ�λ�ý��в���
                Vector3 newPosition = childTransform.forward * FlashDistance;
                characterController.Move(newPosition);
                PlayerAttribute.instance.life = PlayerAttribute.instance.defaultLife;

                //����ֵ��һ
                if (PlayerAttribute.instance.Energy < PlayerAttribute.instance.MaxEnergy)
                {
                    PlayerAttribute.instance.Energy += 1;
                }
                //��������
            
                StartCoroutine(EnemyDead());
 
            }
            else
            {
                Debug.LogWarning("Child object with name 'Body' not found!");
            }
            
            Seach.dashSuccess = false;
            Seach.hitSuccess= false;
            Seach.IsAttack = false;
            IsFalshMove = true;
            IsUseSkill= false;
        }
    }
    void UsedBoom()
    {
        if (Input.GetKeyDown(KeyCode.K)&& IsFalshMove)
        {
            StartCoroutine(Boom());//��ը����
            BoomEnemy();//�ݻٵ���
            IsFalshMove=false;
            PlayerAttribute.instance.Energy= 0;
        }
    }
    private void Fall()
    {
        if (IsGrounded())
        {
            // ��ɫ�ŵأ�ֹͣ����
            speedDown.y = 0;
        }
        else
        {
            // ��ɫδ�ŵأ�Ӧ������Ч��
            speedDown.y +=  Physics.gravity.y * Time.deltaTime;
        }
        characterController.Move(speedDown*Time.deltaTime);
        Debug.DrawRay(transform.position, Vector3.down,Color.red);
    }
    private bool IsGrounded()
    {
        float rayDistance = 0.1f; // ���߼�����
        return Physics.Raycast(transform.position, Vector3.down, rayDistance);
    }
    private IEnumerator Boom()
    {
        if (PlayerAttribute.instance.Energy == 100)
        {
            Vector3 scale = Sphere.transform.localScale;
            float growthRate = 5f; // ��������ÿ֡���ӵĳߴ�
            

            // ��ѭ��������������ĳߴ�
            while (scale.x < BoomRange)
            {
                scale += new Vector3(growthRate, growthRate, growthRate);
                Sphere.transform.localScale = scale;
                yield return new WaitForSeconds(1f);
            }
        }
    }
    IEnumerator EnemyDead()
    {
        // Check if there is a death animation currently playing
        if (isDeathAnimationPlaying)
        {
            // If a death animation is already playing, enqueue the enemy object
            enemyQueue.Enqueue(Seach.Enemy);
            yield break; // Exit the coroutine to avoid starting a new coroutine
        }

        if (Seach.Ai_animator != null)
        {
            // Set the flag to indicate a death animation is playing
            isDeathAnimationPlaying = true;

            // Play the death animation
            Seach.Ai_animator.SetTrigger("Death");

            // Get the animation's length
            float animationLength = Seach.Ai_animator.GetCurrentAnimatorStateInfo(0).length;

            // Wait for the animation to complete
            yield return new WaitForSeconds(animationLength);

            // Destroy the enemy object
            Destroy(Seach.Enemy);

            // Reset the flag to allow the next death animation to play
            isDeathAnimationPlaying = false;

            // Process the next enemy in the queue, if any
            if (enemyQueue.Count > 0)
            {
                GameObject nextEnemy = enemyQueue.Dequeue();
                StartCoroutine(EnemyDead());
            }
        }
    }
    void BoomEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, BoomRange);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                StartCoroutine(PlayDeathAnimationAndDestroy(collider.gameObject));
            }
        }
    }

    IEnumerator PlayDeathAnimationAndDestroy(GameObject enemyObject)
    {
        // ��ȡ���˶����ϵĶ������������ʹ��Animator�����
        Animator animator = enemyObject.GetComponent<Animator>();
        if (animator != null)
        {
            // ������������
            animator.SetTrigger("Death");

            // ��ȡ���������ĳ���
            AnimationClip deathAnimation = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            float animationLength = deathAnimation.length;

            // �ȴ�һ��ʱ�������������������
            yield return new WaitForSeconds(animationLength + 1f);
        }

        // ���ٵ��˶���
        Destroy(enemyObject);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            if (IsUseSkill)
            {
                // �����ﴦ����ײ����ʱ���߼�
                PlayerAttribute.instance.life -= PlayerAttribute.instance.damage;
                PlayerAttribute.instance.Energy -= 1;

            }
        }
    }

}