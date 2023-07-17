using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{

    public CharacterController characterController;
    public MyCamera Camera;
    [Header("瞬杀距离")]
    [SerializeField] private float FlashDistance = 5f;//瞬杀距离
    [Header("玩家速度")]
    [SerializeField] private float Speed = 3f;
    [Header("爆炸范围与速度")]
    [SerializeField][Range(0, 500)] private float BoomSpeed;
    [SerializeField][Range(0, 100)] private float BoomRange;
    //接口调入
    private PlayerSeach Seach;
    /*刚体*/
    private Rigidbody rb;
    /*垂直下落*/
    private Vector3 speedDown;//控制玩家掉落
    /*子物体的位置信息*/
    Transform childTransform;
    /*技能范围*/
    public GameObject Sphere;
    /*瞬闪*/
    private bool IsFalshMove=false;
    /*是否受击*/
    private bool IsUseSkill=true;

    /**/
    private Queue<GameObject> enemyQueue = new Queue<GameObject>(); // 协程队列
    public bool isDeathAnimationPlaying = false; // 判断动画是否正常播放
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
        /*控制玩家输入*/
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");


        Vector3 Playermovement = new Vector3(horizontalInput, 0, verticalInput) * Speed * Time.deltaTime;

        /*获取相机位置属性与旋转属性*/
        Quaternion cameraRotation = Camera.transform.rotation;
        Vector3 cameraForward = cameraRotation * Vector3.forward;
        Vector3 cameraRight = cameraRotation * Vector3.right;
        /*禁用y轴，本质意义上是给予玩家两个向量，一个控制前后一个控制左右*/
        cameraForward.y = 0f;
        cameraForward.Normalize();
        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 movement = cameraRight * Playermovement.x + cameraForward * Playermovement.z;

        characterController.Move(movement);
    }


    /*瞬杀*/
   private void FlashMove()
    {
        Debug.Log(Seach.dashSuccess);
        if (Input.GetKeyDown(KeyCode.Space)&&(Seach.dashSuccess&& Seach.IsAttack )||Seach.hitSuccess)
        {
            if (childTransform != null)
            {   
                // 在这里可以使用新的位置进行操作
                Vector3 newPosition = childTransform.forward * FlashDistance;
                characterController.Move(newPosition);
                PlayerAttribute.instance.life = PlayerAttribute.instance.defaultLife;

                //能量值加一
                if (PlayerAttribute.instance.Energy < PlayerAttribute.instance.MaxEnergy)
                {
                    PlayerAttribute.instance.Energy += 1;
                }
                //敌人死亡
            
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
            StartCoroutine(Boom());//爆炸动画
            BoomEnemy();//摧毁敌人
            IsFalshMove=false;
            PlayerAttribute.instance.Energy= 0;
        }
    }
    private void Fall()
    {
        if (IsGrounded())
        {
            // 角色着地，停止掉落
            speedDown.y = 0;
        }
        else
        {
            // 角色未着地，应用重力效果
            speedDown.y +=  Physics.gravity.y * Time.deltaTime;
        }
        characterController.Move(speedDown*Time.deltaTime);
        Debug.DrawRay(transform.position, Vector3.down,Color.red);
    }
    private bool IsGrounded()
    {
        float rayDistance = 0.1f; // 射线检测距离
        return Physics.Raycast(transform.position, Vector3.down, rayDistance);
    }
    private IEnumerator Boom()
    {
        if (PlayerAttribute.instance.Energy == 100)
        {
            Vector3 scale = Sphere.transform.localScale;
            float growthRate = 5f; // 设置球体每帧增加的尺寸
            

            // 在循环中逐渐增加球体的尺寸
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
        // 获取敌人对象上的动画组件（假设使用Animator组件）
        Animator animator = enemyObject.GetComponent<Animator>();
        if (animator != null)
        {
            // 播放死亡动画
            animator.SetTrigger("Death");

            // 获取死亡动画的长度
            AnimationClip deathAnimation = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            float animationLength = deathAnimation.length;

            // 等待一段时间以完成死亡动画播放
            yield return new WaitForSeconds(animationLength + 1f);
        }

        // 销毁敌人对象
        Destroy(enemyObject);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            if (IsUseSkill)
            {
                // 在这里处理碰撞发生时的逻辑
                PlayerAttribute.instance.life -= PlayerAttribute.instance.damage;
                PlayerAttribute.instance.Energy -= 1;

            }
        }
    }

}