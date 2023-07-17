using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange; //攻击范围
    public float speed;        //移动速度
    public float coolDown;      //冷却时间
    public int damage;         //造成伤害

}
