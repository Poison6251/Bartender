using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DalmoreSetting;

public class VisitDirector : MonoBehaviour
{
    private WaitForSeconds delay = new WaitForSeconds(5f);      // ½ºÆù µô·¹ÀÌ
    private List<Guest> list;                                   // ¼Õ´Ôµé

    [SerializeField] private Guest[] guest;                     // ¼Õ´Ô ÇÁ¸®Æé
    [SerializeField] private int maxGuestCount;                 // ÃÖ´ë ¼Õ´Ô ¼ö
    

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
