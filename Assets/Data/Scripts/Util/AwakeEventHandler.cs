using UnityEngine;
using UnityEngine.Events;

public class AwakeEventHandler : MonoBehaviour
{
    public UnityEvent OnAwakeEvent;
    private void Awake()
    {
        OnAwakeEvent.Invoke();
    }
}
