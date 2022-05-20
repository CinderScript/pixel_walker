using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public Activatable Activatable;

	private void Awake()
	{
		Activatable = GetComponent<Activatable>();
	}

	private void OnTriggerEnter(Collider other)
	{
        Activatable.Activate();
	}
}