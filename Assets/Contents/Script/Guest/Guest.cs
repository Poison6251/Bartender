using UnityEngine;
using static DalmoreSetting;
public class Guest : MonoBehaviour
{
    [SerializeField] private IGuestMover mover;         // 이동기능
    [SerializeField] private string cheerSoundName;
    [SerializeField] private string normalSoundName;
    [SerializeField] private string raggingSoundName;
    private void Awake()
    {
        if(mover == null) mover = GetComponent<IGuestMover>();
    }

    private void OnEnable()
    {
        mover.seatEvent += Order;
    }
    public IGuestMover GetMover()
    {
        return mover;
    }
    // 주문
    public void Order() 
    {
        // 주문판 확인
        var list = DalmoreMenu.GetList;
        if (list == null || list.Count == 0) return;

        // 메뉴 선택
        var select = list[Random.Range(0, list.Count)];

        // 주문
        OrderWindowList.AddItem(select);

        // 주문 완료, 이벤트 삭제
        mover.seatEvent -= Order;
    }
    // 요리 받기
    public void Take() { }

}
