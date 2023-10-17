using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DalmoreSetting;

public class VisitDirector : MonoBehaviour
{
    private WaitForSeconds delay = new WaitForSeconds(5f);      // ���� ������
    private List<Guest> list;                                   // �մԵ�

    [SerializeField] private Guest[] guest;                     // �մ� ������
    [SerializeField] private int maxGuestCount;                 // �ִ� �մ� ��
    

    private void Awake()
    {
        list = new List<Guest>();
        for(int i=0; i<maxGuestCount; i++)
        {
            guest[i].gameObject.SetActive(false);
            list.Add(guest[i]);
        }
    }
    public void StartGuestEnter()
    {
        StartCoroutine(Spawn());
    }
    public void StopGuestEnter()
    {
        StopAllCoroutines();
    }
    public void GameDataReset()
    {
        StopGuestEnter();
        AllGuestExit();
    }

    private void AllGuestExit()
    {
        foreach(var i in guest)
        {
            if(i.gameObject.activeSelf)
                i.GetMover().MoveExit();
        }
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            if (list.Count(x=>x.gameObject.activeSelf) < maxGuestCount)
            {
                var index = list.FindIndex(x => !x.gameObject.activeSelf);
                if (index>=0)
                {
                    list[index].gameObject.SetActive(true);
                    
                }
            }
                yield return delay;
        }
    }
}
