using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnableEventHandler : MonoBehaviour
{
    [SerializeField]
    UnityEvent enableEvents;
    [SerializeField]
    UnityEvent disableEvents;

    private void OnEnable()
    {
        enableEvents?.Invoke();
    }

    private void OnDisable()
    {
        disableEvents?.Invoke();
    }
}
