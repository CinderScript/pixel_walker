using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCRotateDirect : MonoBehaviour
{
    public enum ExMode
    {
        animation = 0,
        script = 1,
    }
    public ExMode ExModes = ExMode.animation;
    public bool IsInverse;
    public string Default = "";
    public string Inverse = "";
    public float Speed = 10;

	// Use this for initialization
	void Start ()
    {
        if (ExModes == ExMode.animation)
        {
            if (IsInverse) GetComponent<Animation>().Play(Inverse);
            else GetComponent<Animation>().Play(Default);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ExModes == ExMode.script)
        {
            transform.Rotate(0, Speed * Time.deltaTime, 0);
        }
    }
}
