using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { IDLE, ATTACK, MOVE, DEAD }

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyController : MonoBehaviour
{
    public EnemyStates enemyState; //当前敌人状态
    public NavMeshAgent agent;
    public Animator anim;
    [Header("Basic Settings")]
    public float attackRange; //攻击范围
    public float hitFixDistance; // attack range - hfd >= dtp时，才进行攻击
    public float actualrange;
    public int dumbtime; //发呆时间
    public float speed;        //移动速度
    public float attackSpeed;      //攻击速度
    public int damage;         //造成伤害
    public float distanceToPlayer; //敌人到玩家的距离
    
    public Transform playerTrans;
   


    public bool isDead = false;   //是否死亡 直接给你
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.speed = speed;
        
    }
    

    private void DrawCircles(float r)
    {
        //圆的中心点位置
        Vector3 center = transform.position;
        //圆的半径
        float radius = r;
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
    private void Start()
    {
        StartCoroutine(Idle());
        //DrawCircles(attackRange);
        //DrawCircles(attackRange - hitFixDistance);

    }
    IEnumerator Idle()
    {
        enemyState = EnemyStates.IDLE;
        
        yield return new WaitForSeconds(dumbtime);
       
        enemyState = EnemyStates.MOVE;
    }
    private void Update()
    {
        
        StatusCalculate();
        SwitchStates();

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            anim.speed = attackSpeed;
        }
        actualrange = attackRange - hitFixDistance;
    }

    void GetPlayer()
    {
        
        Collider []collider = Physics.OverlapSphere(transform.position, attackRange,6);
        if (collider == null) return;
        if (collider != null)
            Debug.Log(collider.Length);
            //playerTrans = collider[0].GetComponent<Transform>();
    }

    void StatusCalculate()
    {
        GetPlayer();//在攻击范围内实时获取当前玩家的位置

        distanceToPlayer = Vector3.Distance(transform.position, playerTrans.position); //计算两者之间距离

        if(distanceToPlayer > (attackRange - hitFixDistance) && enemyState != EnemyStates.IDLE)
        {
            enemyState = EnemyStates.MOVE;
        }
        else if (distanceToPlayer <= (attackRange - hitFixDistance) && enemyState!= EnemyStates.IDLE)
        {
            enemyState = EnemyStates.ATTACK;
        }
        else if(isDead == true)
        {
            enemyState = EnemyStates.DEAD;
        }
    }

    void SwitchStates()
    {
        switch(enemyState)
        {
            case EnemyStates.IDLE:
                {
                   
                    break;
                }
            case EnemyStates.ATTACK:
                {
                    Attack();
                    break;
                }
                
            case EnemyStates.MOVE:
                {
                    anim.speed = 1;
                    Move();
                    break;
                }
            case EnemyStates.DEAD:
                {
                    anim.speed = 1;
                    Die();
                    break;
                }

        }
    }

    private void Move()
    {
        agent.isStopped = false;
        anim.SetBool("IsWalk", true);
        anim.SetBool("IsAttack", false);
        agent.destination = playerTrans.position;
    }

    private void Attack()
    {
        anim.SetBool("IsAttack",true);
        anim.SetBool("IsWalk", false);
        agent.isStopped = true;
        
    }

    void Die()
    {
        if(isDead == true) //若是检测到被弄死了 则
        {
            anim.SetTrigger("Death");
            agent.isStopped = true;
        }
    }

    
}
