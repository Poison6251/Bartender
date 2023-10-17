using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using static DalmoreSetting;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(ItemDataComponent))]
public class AttachObject : BartenderData
{
    [SerializeField] ItemDataComponent _itemData;
    private struct AttachTargetInfo
    {
        public Collider Coll;
        public Transform Parent;
        public Transform Transform;
        public Interactable Interactable;
    
        public ItemDataComponent ItemData;
        public Vector3 Scale;
        public AttachTargetInfo(Collider coll, Transform parent, Transform transform, Interactable interactable, ItemDataComponent itemData, Vector3 scale)
        {
            Coll = coll;
            Parent = parent;
            Transform = transform;
            Interactable = interactable;
            ItemData = itemData;
            Scale = scale;
        }
    }
    private static uint groupRange = 10000;
    private Interactable m_Interaction;
    public bool isAttached { get; private set; }
    public List<uint> groupID;              // 아이템 분류 (술, 가니쉬, 도구 등)
    public List<uint> itemID;               // 개별 아이템 (셰이커, 스트레이너 등)
    public Vector3 attachPosition;          // 부착 위치
    public Vector3 attachRotatoin;          // 부착 각도
    private AttachTargetInfo attachTarget;  // 부착할 오브젝트
    public UnityEvent OnAttachEvent;        // 어태치 되면 발동할 이벤트
    public UnityEvent OnDettachEvent;        // 디태치 되면 발동할 이벤트
    private uint m_Data;
    public uint AttachedID { get; private set; }

    uint _wantAttachedID;
    private void Awake()
    {
        m_Interaction = GetComponent<Interactable>();
        if (_itemData != null)
        {
            m_Data = _itemData.GetItemData.ID;
        }
        else if(TryGetComponent(out ItemDataComponent data))
        { 
            m_Data = data.GetItemData?.ID ?? 0;
        }
        AttachedID = m_Data;

        // 추가
        // 성공 액션 등록, 디렉터 제거
        _action = new SuccessAction(gameObject, _successCallback, _action);
        _director = null;
        // ==
    }
    public override void AddCallback(UnityAction action, Vector3 data)
    {
        base.AddCallback(action, data);
        _wantAttachedID = ((uint)data.x);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isAttached) return;
        var interactable = other.gameObject.GetComponent<Interactable>();
        if (attachTarget.Coll == other) return;
        ItemDataComponent itemData = other.GetComponents<ItemDataComponent>()?.ToList().Find(x=> x.GetItemData.GetType()!=typeof(RecipeData));
        if (!itemData || !itemData.GetItemData || !IsAttachTarget(itemData.GetItemData.ID)) return;
        if(interactable)
        {
            // 대상을 부착할 오브젝트로 선택
            attachTarget = new AttachTargetInfo(other, other.transform.parent, other.transform, interactable, itemData, other.transform.localScale);
            // 대상을 놓으면 부착
            interactable.onDetachedFromHand.RemoveListener(Detach);
            interactable.onDetachedFromHand.AddListener(Attach);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (attachTarget.Coll != other) return;
        other.gameObject.GetComponent<Interactable>()?.onDetachedFromHand.RemoveListener(Attach);
        if(isAttached)
        other.gameObject.GetComponent<Interactable>()?.onDetachedFromHand.AddListener(Detach);
    }

    public bool IDUpdate(uint child,bool isAttach)
    {
        bool isUpdate = false;
        if (isAttach)
        {
            var i = DB_Crafting.GetList.Find(x => (x.A == m_Data && x.B == child));
            if(i.Result != 0 && AttachedID != i.Result)
            {
                AttachedID = i.Result;
                isUpdate = true;
            }
        }
        else if(AttachedID != m_Data)
        {
            AttachedID = m_Data;
            isUpdate = true;
        } 
        
        if(isUpdate) transform.parent?.GetComponent<AttachObject>()?.IDUpdate(AttachedID,true);
        return isUpdate;
    }

    // 부착
    private void Attach(Hand x)
    {
        var target = attachTarget.Transform;
        var id = target.GetComponent<AttachObject>()?.AttachedID ?? target.GetComponent<ItemDataComponent>()?.GetItemData.ID;
        if (id == 0 || !IDUpdate((uint)id, true)) return;

        // 부착
        Vector3 scale = new Vector3(attachTarget.Scale.x / transform.lossyScale.x, attachTarget.Scale.y / transform.lossyScale.y, attachTarget.Scale.z / transform.lossyScale.z);
        attachTarget.Interactable.onDetachedFromHand.RemoveListener(Attach);
        attachTarget.Transform.SetParent(transform);
        attachTarget.Transform.localScale = scale;
        attachTarget.Transform.SetLocalPositionAndRotation(attachPosition, Quaternion.Euler(attachRotatoin));
        attachTarget.Transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        attachTarget.Transform.GetComponent<Rigidbody>().isKinematic = true;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        isAttached = true;

        // 부착 이벤트 실행
        OnAttachEvent.Invoke();

        // 추가
        // 
        if (AttachedID == _wantAttachedID) Success();
        // ==
    }
    // 탈착
    public void Detach(Hand x)
    {
        var target = attachTarget.Transform;
        var id = target?.GetComponent<AttachObject>()?.AttachedID ?? target?.GetComponent<ItemDataComponent>()?.GetItemData.ID;
        if (id == 0 || !IDUpdate((uint)id, false)) return;

        // 탈착
        attachTarget.Transform.SetParent(attachTarget.Parent,false);
        attachTarget.Transform.localScale = attachTarget.Scale;
        attachTarget.Transform.SetPositionAndRotation(attachTarget.Transform.position,attachTarget.Transform.rotation);
        attachTarget.Transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        attachTarget.Transform.GetComponent<Rigidbody>().isKinematic = false;
        attachTarget.Coll = null;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        isAttached = false;

        // 탈착 이벤트 실행
        OnDettachEvent.Invoke();

        // 추가
        // 
        if (AttachedID == _wantAttachedID) Success();
        // ==
    }

    private bool IsAttachTarget(uint id)
    {
        if (groupID != null)
        {
            foreach(var group in  groupID)
            {
                // 대상 오브젝트가 부착 가능한 그룹이다.
                if (group / groupRange == id / groupRange) return true;
            }
        }
        if (itemID != null)
        {
            foreach(var item in itemID)
            {
                // 대상 오브젝트가 부착 가능한 아이템이다.
                if (item==id) return true;
            }
        }
        return false;
    }
}
