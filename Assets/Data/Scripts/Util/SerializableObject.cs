using System;
using UnityEngine;

[Serializable]
public class SerializableObject<T> where T : class
{
    [SerializeField]
    private MonoBehaviour m_RefObj;
    public T Ref
    {
        get
        {
            return m_RefObj as T;
        }
        set
        {
            m_RefObj = value as MonoBehaviour;
        }
    }

}