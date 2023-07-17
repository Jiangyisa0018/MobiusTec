using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttribute : MonoBehaviour
{
    public static PlayerAttribute instance;

    [Header("生命")][Range(0,1000)]public float life;
    [Header("受到伤害值")][Range(0, 1000)] public float damage;
    [Header("能量")][Range(0, 1000)] public float Energy;
    [Header("CD")][Range(0, 1000)] public float energyCDTime ;
    [Header("无敌时间")][Range(0, 1000)] public float dashNoDamageTime;
    [Header("削弱蓝条")][Range(0, 1000)] public float energyDownDamageTime;
    [Header("能量峰值")] public float MaxEnergy = 100f;
    [Header("Dont MODIFY ")]
    public  float defaultLife;
    public float defaultEnergy;
    private float defaultDamage;
    private float defaultEnergyCDTime;
    private float defaultDashNoDamageTime;
    private float defaultEnergyDownDamageTime;
    void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        saveAttribute();
    }
    /* private void Update()
     {
         BeHit();
     }
     void BeHit()
     {
         life-=Time.deltaTime;
     }*/
    public void saveAttribute()
    {
        defaultLife = life;
        defaultDamage = damage;
        defaultEnergy = Energy;
        defaultEnergyCDTime = energyCDTime;
        defaultDashNoDamageTime = dashNoDamageTime;
        defaultEnergyDownDamageTime = energyDownDamageTime;
    }
    public void ResetToDefault()
    {
        life = defaultLife;
        damage = defaultDamage;
        Energy = defaultEnergy;
        energyCDTime = defaultEnergyCDTime;
        dashNoDamageTime = defaultDashNoDamageTime;
        energyDownDamageTime = defaultEnergyDownDamageTime;
    }
}
