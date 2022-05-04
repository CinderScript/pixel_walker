using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalizationDebug : MonoBehaviour
{
    private Vector3 MAX_MIN_AGENT_WORLD_SIZE = new Vector3(30, 30, 30);
    void Update()
    {
        var normalizedPosition = transform.position;

		normalizedPosition = (normalizedPosition + MAX_MIN_AGENT_WORLD_SIZE)
				/ (MAX_MIN_AGENT_WORLD_SIZE.x) - Vector3.one;
		Debug.Log(normalizedPosition);
    }
}
