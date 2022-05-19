using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class AreaProps : MonoBehaviour
{
	public PropInfo[] Props;
    public GameObject SelectedProp;

	private void Awake()
	{
        Props = FindObjectsOfType(typeof(PropInfo)) as PropInfo[];
	}

	public GameObject SelectRandomProp()
	{
		if (Props.Length == 0)
		{
			Debug.LogError("No props found");
			return null;
		}

		var prop = Props[Random.Range(0, Props.Length)];
		return SelectedProp = prop.gameObject;
	}
}