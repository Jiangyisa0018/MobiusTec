using UnityEngine;
using System.Collections;
public class MonsterInstance : MonoBehaviour
{
    [SerializeField] private GameObject monsterPrefab; //怪物预制体
    [Header("怪物生成间隔时间")][SerializeField][Range(0,100)] private int MonsterTime;
    [Header("怪物总数量")][SerializeField] private int MonsterIndex;
    [Header("怪物单次生成数量")][SerializeField] private int MonsterOnce;
    private GameObject PlayerPrefab; //玩家预制体
    private int enemyCounter; //生成怪物的计数器

    void Start()
    {
        PlayerPrefab = GameObject.FindGameObjectWithTag("Player");
        //初始时，怪物计数为0；
        enemyCounter = 0;
        //重复生成怪物
        InvokeRepeating("CreateEnemy", 0.5F, MonsterTime);
    }

    private void CreateEnemy()
    {
        //如果玩家存活
        if (PlayerAttribute.instance.life > 0)
        {
            //生成怪物
            for (int i = 0; i < MonsterOnce; i++)
            {
                var item = Instantiate(monsterPrefab, new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10)), Quaternion.identity);
                StartCoroutine(Creater(item));
                enemyCounter++;
                //如果计数达到最大值
                if (enemyCounter == MonsterIndex)
                {
                    //停止刷新
                    CancelInvoke();
                    break;
                }
            }
        }
        //玩家死亡
        else
        {
            //停止刷新
            CancelInvoke();
        }
    }

    IEnumerator Creater(GameObject i)
    {
        Debug.Log("IsTrue");
        i.SetActive(true);
        yield return null;
    }
}