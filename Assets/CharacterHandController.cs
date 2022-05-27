using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHandController : MonoBehaviour
{
    private CharacterMovementInput input;
    private Transform handTarget;
    private CollisionThrower collisionThrower;

    // Start is called before the first frame update
    void Start()
    {
		input = GetComponent<CharacterMovementInput>();
		handTarget = transform.Find("HandTarget");
		foreach (Transform child in transform)
		{
			collisionThrower = child.GetComponent<CollisionThrower>();
            if (collisionThrower) break;
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
