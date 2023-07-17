using UnityEngine;
using System.Collections;
public class MonsterInstance : MonoBehaviour
{
    [SerializeField] private GameObject monsterPrefab; //����Ԥ����
    [Header("�������ɼ��ʱ��")][SerializeField][Range(0,100)] private int MonsterTime;
    [Header("����������")][SerializeField] private int MonsterIndex;
    [Header("���ﵥ����������")][SerializeField] private int MonsterOnce;
    private GameObject PlayerPrefab; //���Ԥ����
    private int enemyCounter; //���ɹ���ļ�����

    void Start()
    {
        PlayerPrefab = GameObject.FindGameObjectWithTag("Player");
        //��ʼʱ���������Ϊ0��
        enemyCounter = 0;
        //�ظ����ɹ���
        InvokeRepeating("CreateEnemy", 0.5F, MonsterTime);
    }

    private void CreateEnemy()
    {
        //�����Ҵ��
        if (PlayerAttribute.instance.life > 0)
        {
            //���ɹ���
            for (int i = 0; i < MonsterOnce; i++)
            {
                var item = Instantiate(monsterPrefab, new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10)), Quaternion.identity);
                StartCoroutine(Creater(item));
                enemyCounter++;
                //��������ﵽ���ֵ
                if (enemyCounter == MonsterIndex)
                {
                    //ֹͣˢ��
                    CancelInvoke();
                    break;
                }
            }
        }
        //�������
        else
        {
            //ֹͣˢ��
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