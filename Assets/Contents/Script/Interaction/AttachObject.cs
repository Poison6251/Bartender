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
    public List<uint> groupID;              // ������ �з� (��, ���Ͻ�, ���� ��)
    public List<uint> itemID;               // ���� ������ (����Ŀ, ��Ʈ���̳� ��)
    public Vector3 attachPosition;          // ���� ��ġ
    public Vector3 attachRotatoin;          // ���� ����
    private AttachTargetInfo attachTarget;  // ������ ������Ʈ
    public UnityEvent OnAttachEvent;        // ����ġ �Ǹ� �ߵ��� �̺�Ʈ
    public UnityEvent OnDettachEvent;        // ����ġ �Ǹ� �ߵ��� �̺�Ʈ
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

        // �߰�
        // ���� �׼� ���, ���� ����
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
            // ����� ������ ������Ʈ�� ����
            attachTarget = new AttachTargetInfo(other, other.transform.parent, other.transform, interactable, itemData, other.transform.localScale);
            // ����� ������ ����
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

    // ����
    private void Attach(Hand x)
    {
        var target = attachTarget.Transform;
        var id = target.GetComponent<AttachObject>()?.AttachedID ?? target.GetComponent<ItemDataComponent>()?.GetItemData.ID;
        if (id == 0 || !IDUpdate((uint)id, true)) return;

        // ����
        Vector3 scale = new Vector3(attachTarget.Scale.x / transform.lossyScale.x, attachTarget.Scale.y / transform.lossyScale.y, attachTarget.Scale.z / transform.lossyScale.z);
        attachTarget.Interactable.onDetachedFromHand.RemoveListener(Attach);
        attachTarget.Transform.SetParent(transform);
        attachTarget.Transform.localScale = scale;
        attachTarget.Transform.SetLocalPositionAndRotation(attachPosition, Quaternion.Euler(attachRotatoin));
        attachTarget.Transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        attachTarget.Transform.GetComponent<Rigidbody>().isKinematic = true;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        isAttached = true;

        // ���� �̺�Ʈ ����
        OnAttachEvent.Invoke();

        // �߰�
        // 
        if (AttachedID == _wantAttachedID) Success();
        // ==
    }
    // Ż��
    public void Detach(Hand x)
    {
        var target = attachTarget.Transform;
        var id = target?.GetComponent<AttachObject>()?.AttachedID ?? target?.GetComponent<ItemDataComponent>()?.GetItemData.ID;
        if (id == 0 || !IDUpdate((uint)id, false)) return;

        // Ż��
        attachTarget.Transform.SetParent(attachTarget.Parent,false);
        attachTarget.Transform.localScale = attachTarget.Scale;
        attachTarget.Transform.SetPositionAndRotation(attachTarget.Transform.position,attachTarget.Transform.rotation);
        attachTarget.Transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        attachTarget.Transform.GetComponent<Rigidbody>().isKinematic = false;
        attachTarget.Coll = null;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        isAttached = false;

        // Ż�� �̺�Ʈ ����
        OnDettachEvent.Invoke();

        // �߰�
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
                // ��� ������Ʈ�� ���� ������ �׷��̴�.
                if (group / groupRange == id / groupRange) return true;
            }
        }
        if (itemID != null)
        {
            foreach(var item in itemID)
            {
                // ��� ������Ʈ�� ���� ������ �������̴�.
                if (item==id) return true;
            }
        }
        return false;
    }
}
