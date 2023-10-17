using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Animator/RAC_Getter")]
public class RACGetter : ScriptableObject
{
    [SerializeField]
    List<key_value> list = new List<key_value>();

    public RuntimeAnimatorController GetController(string name)
    {
        if(string.IsNullOrEmpty(name)) return null;
        var animator = list.Find(x => x.key == name)?.value;
        return animator;
    }
    
    [System.Serializable]
    class key_value
    {
        public string key;
        public RuntimeAnimatorController value;
    }
    
}


