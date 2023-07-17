using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSeach : MonoBehaviour
{
    [Header("检测半径")]
    [SerializeField] private float detectionRadius = 5f;
    [Header("规定距离")]
    [SerializeField] private float distance0 = 5f;
    [SerializeField] private float distance1 = 10f;
    [SerializeField] private float distance2 = 20f;
    public bool dashSuccess=false;//瞬闪判定
    public bool hitSuccess=false;//受击判定
    public bool IsAttack=false;//是否可以攻击
    public GameObject Enemy;
    /*距离判定*/
    private float ToPlayerDistance;//敌人距离玩家的距离
    private float SwordToPlayerDistance;//🗡距离玩家的距离 

    public Animator Ai_animator;

    //接口调入
    private PlayerMove Move;
    private void Start()
    {
        Move=GetComponentInParent<PlayerMove>();
    }
    void Update()
    {   
        RayCheck();
        SeachEnemy();      
        OnDrawGizmos();
    }


    private void SeachEnemy() 
    {
        
        /*OverlapSphere获取检测区域中所有碰撞体信息并存储到一个数组里面*/
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        bool foundEnemy = false; // 布尔标志用于跟踪是否找到了敌人

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Vector3 enemyPosition = collider.transform.position;
                ToPlayerDistance = Vector3.Distance(transform.position, enemyPosition);
                foundEnemy = true; // 找到了敌人
                if (ToPlayerDistance > distance1 && ToPlayerDistance < distance2&&Ai_animator.GetCurrentAnimatorStateInfo(0).IsName("Aim_S"))
                {
                    //判定前方是否为敌人
                    RayCheck();  
                }
                else if (ToPlayerDistance < distance1 && (Ai_animator != null && (Ai_animator.GetCurrentAnimatorStateInfo(0).IsName("Aim_S") || Ai_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")))) // 击杀敌人
                {
                    RayCheck();
                }
            }

            else
            {
                Debug.LogWarning("没有敌人");
                break;
            }
            // 没有找到敌人时中断函数
            if (!foundEnemy)
            {
                return;
            }
        }
        foreach (Collider collider in colliders)//判定武器受击点
        {
            if (collider.CompareTag("Weapon")&& Ai_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                Vector3 swordPosition = collider.transform.position;
                SwordToPlayerDistance = Vector3.Distance(transform.position, swordPosition);
                if (SwordToPlayerDistance < distance0)
                {
                    hitSuccess=true;
                    IsAttack = true;
                }
            }
            else
            {
                Debug.LogWarning("没有检测到武器信息");
                break;
            }
        }
    }
    public void RayCheck()
    {
        if(Move.isDeathAnimationPlaying) { return; }
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, transform.localScale / 2, transform.forward, out hit, transform.rotation, 100))
        {
            // 检测到碰撞，hit.collider可以获取被击中的物体
            if (hit.collider.CompareTag("Enemy"))
            {
                dashSuccess = true;
                Enemy = hit.collider.gameObject;
                Ai_animator = Enemy.GetComponent<Animator>();
                if (Ai_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")|| Ai_animator.GetCurrentAnimatorStateInfo(0).IsName("Aim_S"))
                {
                    IsAttack= true;
                }
            }
        }
    }

    /*可视化射线*/
    private void OnDrawGizmos()
    {
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, transform.localScale/2 , transform.forward, out hit, transform.rotation, 100))
        {
            // 绘制射线
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);

            // 绘制盒形
            Vector3 boxCenter = transform.position + transform.forward * hit.distance / 2;
            Vector3 boxSize = transform.localScale;
            Vector3 halfBoxSize = boxSize / 2f;
            Vector3 boxCorner = boxCenter - halfBoxSize;

            Debug.DrawLine(boxCorner, boxCorner + new Vector3(boxSize.x, 0f, 0f), Color.green);
            Debug.DrawLine(boxCorner, boxCorner + new Vector3(0f, boxSize.y, 0f), Color.green);
            Debug.DrawLine(boxCorner, boxCorner + new Vector3(0f, 0f, boxSize.z), Color.green);

            Vector3 otherCorner = boxCorner + boxSize;

            Debug.DrawLine(otherCorner, otherCorner - new Vector3(boxSize.x, 0f, 0f), Color.green);
            Debug.DrawLine(otherCorner, otherCorner - new Vector3(0f, boxSize.y, 0f), Color.green);
            Debug.DrawLine(otherCorner, otherCorner - new Vector3(0f, 0f, boxSize.z), Color.green);

            Debug.DrawLine(boxCorner + new Vector3(boxSize.x, 0f, 0f), boxCorner + new Vector3(boxSize.x, boxSize.y, 0f), Color.green);
            Debug.DrawLine(boxCorner + new Vector3(boxSize.x, 0f, 0f), boxCorner + new Vector3(boxSize.x, 0f, boxSize.z), Color.green);
            Debug.DrawLine(boxCorner + new Vector3(0f, boxSize.y, 0f), boxCorner + new Vector3(0f, boxSize.y, boxSize.z), Color.green);

            Debug.DrawLine(otherCorner - new Vector3(boxSize.x, 0f, 0f), otherCorner - new Vector3(boxSize.x, boxSize.y, 0f), Color.green);
            Debug.DrawLine(otherCorner - new Vector3(boxSize.x, 0f, 0f), otherCorner - new Vector3(boxSize.x, 0f, boxSize.z), Color.green);
            Debug.DrawLine(otherCorner - new Vector3(0f, boxSize.y, 0f), otherCorner - new Vector3(0f, boxSize.y, boxSize.z), Color.green);
        }
    }
}
 