using System;
using UnityEngine;

public interface IGuestMover
{
    // 가게 입장
    public void MoveSeat(Transform target);
    // 퇴장
    public void MoveExit();
    public event Action seatEvent;
}

public abstract class GuestMover : MonoBehaviour, IGuestMover
{
    protected enum State {Wait,Move,Seat}
    [SerializeField] protected Waiter waiter;                       // 웨이터
    [SerializeField] protected Transform spawnPoint,exitPoint;      // 입장, 퇴장 위치
    [SerializeField] private string seatAnimName = "Sit_Enter";     // 앉는 모션
    [SerializeField] private string idleAnimName = "Idle";          // 아이들 모션
    protected Animator guestAnim;                                   // 손님 애니메이터
    protected State m_state;                                        // 손님 상태
    public event Action seatEvent;                                  // 앉고 나서 실행할 이벤트


    protected virtual void Awake()
    {
        guestAnim = GetComponent<Animator>();
    }
    protected virtual  void OnEnable()
    {
        
        if(m_state== State.Wait)
        {
            // 입장 대기
            transform.position = spawnPoint.position;
            waiter.GuestEnterEvent(MoveSeat,GetHashCode());
        }
    }
    // 가게 입장 후 자리에 앉기 구현
    public abstract void MoveSeat(Transform target);
    // 자리에서 퇴장 구현
    public abstract void MoveExit();
    // 가게 웨이팅
    protected void Wait()
    {
        m_state = State.Wait;
        if(guestAnim==null) guestAnim= GetComponent<Animator>();
        guestAnim.Play(idleAnimName);
    }
    // 앉는다
    protected void Seat()
    {
        m_state = State.Seat;
        guestAnim.Play(seatAnimName);
        Invoke( nameof(SeatEvent),0.5f);

    }
    private void SeatEvent()
    {
        seatEvent?.Invoke();
    }
}
