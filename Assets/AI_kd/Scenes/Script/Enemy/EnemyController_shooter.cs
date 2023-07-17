using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum ShooterEnemyState { IDLE, ATTACK, MOVE, DEAD, AIM}

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyController_shooter : MonoBehaviour
{
    public ShooterEnemyState enemyState; //当前敌人状态
    
    public NavMeshAgent agent;
    public Animator anim;
    [Header("Basic Settings")]
    public float waitShootingTime; //读秒时间
    public float attackRange; //攻击范围
    public int dumbtime; //发呆时间
    public float hitFixDistance; // attack range - hfd >= dtp时，才进行攻击
    public float actualrange;
    public float speed;        //移动速度
    public float attackSpeed;      //攻击速度
    public int damage;         //造成伤害
    public float distanceToPlayer; //敌人到玩家的距离
    public Transform playerTrans;
    public bool isDead = false;   //是否死亡 直接给你
    
    public float timer;
   

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.speed = speed;
        

    }

    
   
    private void Start()
    {
        StartCoroutine(Idle());
        timer = waitShootingTime;
     
    }

    IEnumerator Idle() 
    {
        enemyState = ShooterEnemyState.IDLE;

        yield return new WaitForSeconds(dumbtime);
        enemyState = ShooterEnemyState.MOVE;
    }

    private void Update()
    {
        Timer();
        StatusCalculate();
        SwitchStates();

        if(timer == 0)
        {
            //攻击动画
        }




        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            anim.speed = attackSpeed;
        }
        actualrange = attackRange - hitFixDistance;
    }

    void GetPlayer()
    {

        Collider[] collider = Physics.OverlapSphere(transform.position, attackRange, 6);
        if (collider == null) return;
       
           
        //playerTrans = collider[0].GetComponent<Transform>();
    }

    void StatusCalculate()
    {
        GetPlayer();//在攻击范围内实时获取当前玩家的位置

        distanceToPlayer = Vector3.Distance(transform.position, playerTrans.position); //计算两者之间距离
        


        if (distanceToPlayer > (attackRange - hitFixDistance) && enemyState != ShooterEnemyState.IDLE) //开始时追击
        {
            enemyState = ShooterEnemyState.MOVE;
        }

        else if (distanceToPlayer <= (attackRange - hitFixDistance)&& timer > 0) //进圈就开始瞄准,进圈且在到计时的时候
        {
            enemyState = ShooterEnemyState.AIM;
        }

        else if (distanceToPlayer <= (attackRange - hitFixDistance) && timer < 0) //进圈攻击的时候
        {
            enemyState = ShooterEnemyState.ATTACK;
        }
        else if (distanceToPlayer <= (attackRange - hitFixDistance) && timer > -3.5f && timer < 0) //进圈攻击的时候
        {
            enemyState = ShooterEnemyState.ATTACK;
        }
        else if (distanceToPlayer <= (attackRange - hitFixDistance) && timer < -3.5f) //进圈攻击的时候
        {
            timer = waitShootingTime;
        }

        else if (distanceToPlayer > (attackRange - hitFixDistance) && enemyState == ShooterEnemyState.AIM)//在瞄准的时候追击
        { 
            enemyState = ShooterEnemyState.MOVE;
        }
        else if (isDead == true)
        {
            enemyState = ShooterEnemyState.DEAD;
        }
    }
    public void Recover()
    {
        //Invoke("wow", 1f);
        timer = waitShootingTime;
        anim.SetTrigger("AimT");

    }
    void wow()
    {
        
    }

    void Timer() //当进入瞄准动画时，开始计时
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Aim_S"))
        {
            timer = waitShootingTime;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }


    void SwitchStates()
    {
        switch (enemyState)
        {
            case ShooterEnemyState.IDLE:
                {
                    break;
                }
            case ShooterEnemyState.ATTACK:
                {
                    Attack();
                    break;
                }
            case ShooterEnemyState.AIM:
                {
                    Aim(); 
                    break;
                }

            case ShooterEnemyState.MOVE:
                {
                    anim.speed = 1;
                    Move();
                    break;
                }
            case ShooterEnemyState.DEAD:
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
        anim.SetBool("IsAim", false);
        anim.SetBool("IsAttack", false);
        
        agent.destination = playerTrans.position;
    }


    void Aim()
    {
        anim.SetBool("IsWalk", false);
        anim.SetBool("IsAttack", false);
        anim.SetBool("IsAim", true);
        agent.isStopped = true;
        
       
    }

    private void Attack()
    {
        anim.SetBool("IsWalk", false);
        anim.SetBool("IsAim", false);
        anim.SetBool("IsAttack", true);
        agent.isStopped = true;
    }

   

    void Decision()
    {
        if(distanceToPlayer > (attackRange - hitFixDistance))
        {
            enemyState = ShooterEnemyState.MOVE;
        }
        else
        {
            enemyState = ShooterEnemyState.AIM;
        }
    }


    void Die()
    {
        if (isDead == true) //若是检测到被弄死了 则
        {
            anim.SetTrigger("Death");
            agent.isStopped = true;
        }
    }


}
