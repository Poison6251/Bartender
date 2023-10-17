using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DartManager : MonoBehaviour
{
    public string DartTag = "Dart";
    public GameObject DartPre;
    public Transform[] transforms;
    void Update()
    {
        var list = GameObject.FindGameObjectsWithTag(DartTag);
        if(list == null || list.Length == 0)
        {
            ResetDart();
        }
    }
    private void OnEnable()
    {
        var list = GameObject.FindGameObjectsWithTag(DartTag);
        if (list != null || list.Length != 0)
        {
            foreach(var i in list)
            {
                Destroy(i.gameObject);
            }
        }
        ResetDart();
    }
    private void OnDisable()
    {
        var list = GameObject.FindGameObjectsWithTag(DartTag);
        if (list != null || list.Length != 0)
        {
            foreach (var i in list)
            {
                Destroy(i.gameObject);
            }
        }
    }
    void ResetDart()
    {
        if(DartPre != null && transforms!=null) 
        {
            foreach(var pos in transforms)
            {
                var obj = Instantiate(DartPre);
                obj.transform.SetPositionAndRotation(pos.transform.position,pos.transform.rotation);
            }
        }
    }
}
