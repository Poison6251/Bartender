using UnityEngine.Events;

public class OnceEvent
{
    UnityEvent _event;
    UnityEvent _onceEvent;

    public OnceEvent()
    {
        _event = new UnityEvent();
        _onceEvent = new UnityEvent();
    }
    public void AddListener(UnityAction action, bool isOnce = true)
    {
        if (action == null) return;
        if(isOnce && !IsUnityEventInSameAction(_onceEvent, action))
            _onceEvent.AddListener(action);
        if(!isOnce && !IsUnityEventInSameAction(_event,action))
            _event.AddListener(action);
    }
    public void Invoke()
    {
        _event.Invoke();
        _onceEvent.Invoke();
        _onceEvent.RemoveAllListeners();
    }
    public void RemoveListener(UnityAction action)
    {
        _event.RemoveListener(action);
        _onceEvent.RemoveListener(action);
    }
    public void RemoveAllListener()
    {
        _event.RemoveAllListeners();
        _onceEvent.RemoveAllListeners();
    }
    bool IsUnityEventInSameAction(UnityEvent myEvent, UnityAction action)
    {
        for (int i = 0; i < myEvent.GetPersistentEventCount(); i++)
        {
            if (action.Method.Name == myEvent.GetPersistentMethodName(i))
            {
                return true;
            }
        }
        return false;
    }
}
