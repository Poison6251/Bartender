using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[System.Serializable]
public class BartenderAction
{
    [SerializeField] protected ParticleSystem _successEffect;
    protected GameObject _go;
    public BartenderAction(GameObject go,BartenderAction origin = null)
    {
        if(origin != null)
        {
            _successEffect = origin._successEffect;
        }
        _go = go;
    }
    public virtual void Success() 
    {
    }
}

public class ModifierAction : BartenderAction
{
    [SerializeField] public RecipeData _target;         // 글라스, 셰이커
    [SerializeField] ItemModifier _modifier;            // 아이템 수식어

    public ModifierAction(GameObject go, BartenderAction origin = null) : base(go,origin) 
    {
        _target = go.GetComponent<ItemDataComponent>()?.GetItemData as RecipeData;
    }


    public override void Success()
    {
        if (_target == null || _target.data.Count == 0) return;
        for (int i = 0; i < _target.data.Count; i++)
        {
            // 수식어 추가            
            _target.data[i].modifier |= _modifier;

        }
    }
}
public class SuccessAction : BartenderAction
{
    public OnceEvent _successEvent;          // 한 번만 실행되는 성공 이벤트
    public SuccessAction(GameObject go, OnceEvent SuccessEvent,BartenderAction origin = null) : base(go , origin) 
    {
        _successEvent = SuccessEvent;
        _successEvent.AddListener(SuccessEffect);
    }
    public override void Success()
    {
        _successEvent?.Invoke();
    }
    void SuccessEffect()
    {
        if (_successEffect != null)
        {
            _successEffect.Play();
            SoundManager.Instance.PlaySound("Sound_성공2");
        }
    }
}
public class SuccessModifierAction : BartenderAction
{
    ModifierAction _modifierAction;
    SuccessAction _successAction;
    public SuccessModifierAction(GameObject go, OnceEvent SuccessEvent, BartenderAction origin = null) : base(go, origin)
    {
        _modifierAction = new ModifierAction(go);
        _successAction = new SuccessAction(go, SuccessEvent);
    }
    public override void Success()
    {
        _modifierAction.Success();
        _successAction.Success();
    }
}