using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IceMaker : MonoBehaviour
{
    [SerializeField] private int iceCount;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Scooper")
        {
            Scooper scooper = other.gameObject.GetComponent<Scooper>();
            if (!scooper.NoneIce()) return;

            for(int i=0;i< iceCount; i++)
            {
                scooper.InitIce(PoolingManager.IcePool.Get());
            }
        }
    }
}
