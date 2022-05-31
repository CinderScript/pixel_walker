using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCDanceLightDelay : MonoBehaviour
{
    void Awake()
    {
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks * 1000);
    }

    // Use this for initialization
    void Start ()
    {
        float _time = Random.Range(0, 1.5f);
        StartCoroutine(TimeDelay(_time));

    }

    IEnumerator TimeDelay(float _time)
    {
        yield return new WaitForSeconds(_time);
        GetComponent<Animation>().Play();
    }

}
