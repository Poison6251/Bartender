using UnityEngine;
using static DalmoreSetting;
public class Guest : MonoBehaviour
{
    [SerializeField] private IGuestMover mover;         // �̵����
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
    // �ֹ�
    public void Order() 
    {
        // �ֹ��� Ȯ��
        var list = DalmoreMenu.GetList;
        if (list == null || list.Count == 0) return;

        // �޴� ����
        var select = list[Random.Range(0, list.Count)];

        // �ֹ�
        OrderWindowList.AddItem(select);

        // �ֹ� �Ϸ�, �̺�Ʈ ����
        mover.seatEvent -= Order;
    }
    // �丮 �ޱ�
    public void Take() { }

}
