using System.Collections;
using UnityEngine;

public class FirstPosIniter : MonoBehaviour
{
    Vector3 firstPos, firstRotation;
    public bool isAwakeDisable = true;
    private void Awake()
    {
        if (isAwakeDisable)
        {
            gameObject.SetActive(false);
        }
        firstPos = transform.position;
        firstRotation = transform.eulerAngles;
    }

    private void OnEnable()
    {
        var attach = transform.GetComponent<AttachObject>();
        if (attach != null && attach.isAttached)
            attach.Detach(null);
        transform.position = firstPos;
        transform.eulerAngles = firstRotation;
        var rigid = transform.GetComponent<Rigidbody>();
        if(rigid != null )
        {
            rigid.velocity = Vector3.zero;
            transform.parent = null;
        }
        
    }
    IEnumerator Delay()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        yield return new WaitForSeconds(3f);
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
