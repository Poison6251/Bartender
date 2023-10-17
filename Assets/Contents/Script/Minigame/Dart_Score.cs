using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dart_Score : MonoBehaviour
{
    [SerializeField]
    public int score;
    public Dart_Plane plane;
    public UnityEvent<int,Vector3> AddScore;

    private void OnTriggerEnter(Collider other)
    {
        if (plane.isDelay) return;
        if (!(other.tag == "Dart")) return;
        StartCoroutine(delay());
        other.gameObject.SetActive(false);
        AddScore.Invoke(score, other.transform.position);
        Destroy(other.gameObject);
    }
    IEnumerator delay()
    {
        plane.isDelay = true;
        yield return null;
        yield return null;
        yield return null;
        plane.isDelay = false;
    }

}


