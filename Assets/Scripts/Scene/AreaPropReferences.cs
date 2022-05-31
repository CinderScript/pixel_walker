using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class AreaPropReferences : MonoBehaviour
{
	public PropInfo[] Props;
    public GameObject SelectedProp;

	private void Awake()
	{
		// area containing props is the parent of this object
		Props = GetComponentsInChildren<PropInfo>();
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

	public GameObject GetProp(string name)
	{
		name = name.Replace("_", " ");
		return Props.FirstOrDefault(p => p.Name.ToLower() == name.ToLower())?.gameObject;
	}

	public string GetAllPropNames()
	{
		string[] names = Props.Select(prop => prop.Name).ToArray();
		for (int i = 0; i < names.Length; i++)
		{
			names[i] = names[i].Replace(' ', '_');
		}
		return string.Join(", ", names);
	}
}