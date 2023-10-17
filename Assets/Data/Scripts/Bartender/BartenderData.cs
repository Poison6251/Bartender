using System;
using UnityEngine;
using UnityEngine.Events;

[Flags]
public enum ItemModifier    // 아이템 수식어
{
    None = 0,
    Stir = 1 << 0,
    Shake = 1 << 1,
    Float = 1 << 2,
    Bland = 1 << 3,
    Cold = 1 << 4,
    Dirty = 1 << 5,
    Dart = 1<< 6
}
public class BartenderData : MonoBehaviour
{
    [SerializeField] protected BartenderAction _action;
    [SerializeField] protected BartenderDirector _director;
    protected OnceEvent _successCallback = new OnceEvent();

    protected Vector3 data;
    protected void SendData()
    {
        _director?._receiveData(data);
    }
    protected void Success()
    {
        _action?.Success();
    }
    public virtual void AddCallback(UnityAction action,Vector3 data)
    {
        _successCallback.AddListener(action);
    }
    public void ResetCallback() 
    {
        _successCallback.RemoveAllListener();
    }
}

[System.Serializable]
public class BartenderDirector
{
    public UnityAction<Vector3> _receiveData;
    protected GameObject _go;

    public BartenderDirector(GameObject go)
    {
        _go = go;
    }
}