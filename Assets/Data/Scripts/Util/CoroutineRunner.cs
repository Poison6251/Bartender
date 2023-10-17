using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CoroutineRunner : MonoBehaviour
{
    static CoroutineRunner instance;
    public static CoroutineRunner Instance 
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject();
                obj.name = "CoroutineRunner";
                instance = obj.AddComponent<CoroutineRunner>();
            }
            return instance;
        }
    }
    public void DelayAction(float delay, UnityAction action)
    {
        StartCoroutine(DelayActionCoroutine(delay, action));
    }
    IEnumerator DelayActionCoroutine(float delay, UnityAction action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
    private void OnDestroy()
    {
        instance = null;
    }
}
