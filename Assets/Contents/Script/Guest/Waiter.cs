using System;
using System.Collections.Generic;
using UnityEngine;


public class Waiter : MonoBehaviour, IWaiter
{
    private const int EmptySeat = 0;
    private List<int> seatGuest;                        // 의자에 앉은 손님
    private Queue<Action<Transform>> waitingQueue;      // 대기열
    [SerializeField] private Transform[] Seats;         // 좌석

    private void Awake()
    {
        // 데이터 초기화
        if (Seats == null)
        {
            Seats = new Transform[0];
            Debug.LogError("좌석 없음!");
        }
        waitingQueue = new Queue<Action<Transform>>();
        seatGuest = new List<int>(Seats.Length);
        for(int i = 0; i < Seats.Length; i++)
        {
            seatGuest.Add(EmptySeat);
        }
    }
    // 손님 입장
    public void GuestEnterEvent(Action<Transform> GuestEnterAction, int guestHashcode)
     {
        if(seatGuest == null) Awake();
         
        // 좌석 확인
        int seatNum = CheckSeat();
        
        //  ↓ 대기열 있음               ↓ 만석                                          
        if (waitingQueue.Count != 0 || seatNum == -1)
        {
            // 대기
            waitingQueue.Enqueue(GuestEnterAction);
            return;
        }

        // 좌석 배정
        GuestEnterAction?.Invoke(Seats[seatNum]);
        seatGuest[seatNum] = guestHashcode;
    }
    // 손님 퇴장
    public void ExitGuest(int guestHashcode)
    {
        // 좌석 확인
        int index = seatGuest.IndexOf(guestHashcode);
        if (index == -1) return;

        // 손님 퇴장
        seatGuest[index] = EmptySeat;

        int seatNum = CheckSeat();
        //  ↓ 대기열 없음               ↓ 만석                                          
        if (waitingQueue.Count == 0 || seatNum == -1) return;

        // 다음 손님 입장
        var Guest = waitingQueue.Dequeue();
        Guest.Invoke(Seats[seatNum]);
        seatGuest[seatNum] = Guest.GetHashCode();

    }
    // 좌석 비우기
    public void ClearSeat()
    {
        waitingQueue = new Queue<Action<Transform>>();
    }
    // 좌석 확인
    private int CheckSeat()
    {
        // 빈자리 확인
        for (int seatNum = 0; seatNum < seatGuest.Count; seatNum++)
        {
            if (seatGuest[seatNum]== EmptySeat) return seatNum;
        }
        // 만석
        return -1;
    }
}


public interface IWaiter
{
    // 손님 입장
    public void GuestEnterEvent(Action<Transform> GuestEnterEvent, int guestHashcode);
    public void ExitGuest(int guestHashcode);
}
