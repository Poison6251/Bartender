using UnityEngine;

public class TransformEventHandler : MonoBehaviour
{
    public Vector3 pos , rot;
    public bool isPos, isRot;
    public GameObject Target;
    public void SetTransrom()
    {
        if(isPos)
        Target.transform.localPosition = pos;
        if(isRot)
        Target.transform.eulerAngles = rot;
    }
}
