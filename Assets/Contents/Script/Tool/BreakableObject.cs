using System.Linq;
using Tutorial;
using UnityEngine;


public class BreakableObject : MonoBehaviour
{
    [SerializeField] private GameObject brokenObjectPrefab;
    [SerializeField] private float breakablePower;
    [SerializeField] private Material unvisibleMat;
    [SerializeField] private float disappearTime;
    
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.relativeVelocity.magnitude > breakablePower)
    //    {
    //        Break();
    //    }
    //}
    public void Break()
    {
        if (brokenObjectPrefab == null) return;
        var brokenObject = Instantiate(brokenObjectPrefab, transform.position, Quaternion.identity);
        brokenObject.transform.parent = null;
        var list = transform.GetComponentsInChildren<MeshRenderer>().ToList();
        list.ForEach(x => x.gameObject.SetActive(false));
        SoundManager.Instance?.PlaySound("Sound_À¯¸®±úÁü1");
        Destroy(brokenObject, disappearTime);
        Destroy(gameObject, 0f);
    }
}
