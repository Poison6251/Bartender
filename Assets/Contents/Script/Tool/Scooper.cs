using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class Scooper : MonoBehaviour
{
    [SerializeField] private List<GameObject> iceTransforms;
    [SerializeField] private List<GameObject> ices;
    [Tooltip("스쿠퍼의 각도가 일정치 이상이면 얼음이 드롭되게 하는 각도 (천장 : 1 , 바닥 : -1) ")]
    [SerializeField] private float iceDropAngle;
    private WaitForSeconds delay = new WaitForSeconds(0.1f);

    private void OnEnable()
    {
        IceClear();
        ices = new List<GameObject>();
        StartCoroutine(IceDropAngle());
    }
    public void InitIce(GameObject ice)
    {
        if (NoneIce()) IceClear();
        foreach(var transform in iceTransforms)
        {
            if (!transform.activeSelf)
            {
                transform.SetActive(true);
                ice.transform.parent = transform.transform;
                ice.transform.position = transform.transform.position;
                ice.transform.rotation = transform.transform.rotation;
                ice.GetComponent<Rigidbody>().isKinematic = true;
                ice.GetComponent<Rigidbody>().useGravity = false;
                ice.GetComponent<BoxCollider>().isTrigger = true;
                ices.Add(ice);
                return;
            }
        }
    }
    public bool NoneIce()
    {
        foreach(var i in ices)
        {
            if (i.activeSelf)
            {
                return false;
            }
        }
        return true;
    }
    [ContextMenu("얼음 놓기")]
    public void IceClear()
    {
        if (NoneIce()) return;
        foreach(var i in ices.ToList())
        {
            if (!i.activeSelf)
                PoolingManager.IcePool.Release(i);
            else
            {
                i.GetComponent<Rigidbody>().isKinematic = false;
                i.GetComponent<Rigidbody>().useGravity = true;
                i.GetComponent<BoxCollider>().isTrigger = false;
                i.transform.parent = null;
                StartCoroutine(IceDestroy(i, 2f));
            }
            
            ices.Remove(i);
        }
        foreach(var i in iceTransforms)
        {
            i.SetActive(false);
        }
    }
    private IEnumerator IceDestroy(GameObject ice,float time)
    {
        yield return new WaitForSeconds(time);
        PoolingManager.IcePool?.Release(ice);
    }
    private IEnumerator IceDropAngle()
    {
        while (true)
        {
            if(transform.up.y < iceDropAngle)
            {
                IceClear();
            }

            yield return delay;
        }
    }
}
