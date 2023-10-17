using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySpeed : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private void Update()
    {
        Time.timeScale = speed;
    }
}
