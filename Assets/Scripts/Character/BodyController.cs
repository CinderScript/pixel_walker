using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour
{
	[Header("Limb and Joint Strength")]
	public Vector3 HeadTorqueMax;
	public Vector3 StomachTorqueMax;
	public Vector3 ShoulderTorqueMax;
	public Vector3 ElbowTorqueMax;
	public Vector3 WristTorqueMax;
	public Vector3 HipTorqueMax;
	public Vector3 KneeTorqueMax;
	public Vector3 AnkleTorqueMax;

	public float AverageLimbVelocity { get; private set; }
	
	private JointController[] jointControllers;

	private void Awake()
	{
		var joints = GetComponentsInChildren<Joint>();
	}

	public void ApplyTorque(Vector3 headTorque, 
							Vector3 stomachTorque, 
							Vector3 shoulderTorque, 
							Vector3 elbowTorque, 
							Vector3 wristTorque, 
							Vector3 hipTorque, 
							Vector3 kneeTorque, 
							Vector3 ankleTorque)
	{
		// Apply Torque
		GetComponent<Rigidbody>().AddRelativeTorque(headTorque);
		GetComponent<Rigidbody>().AddRelativeTorque(stomachTorque);
		GetComponent<Rigidbody>().AddRelativeTorque(shoulderTorque);
		GetComponent<Rigidbody>().AddRelativeTorque(elbowTorque);
		GetComponent<Rigidbody>().AddRelativeTorque(wristTorque);
		GetComponent<Rigidbody>().AddRelativeTorque(hipTorque);
		GetComponent<Rigidbody>().AddRelativeTorque(kneeTorque);
		GetComponent<Rigidbody>().AddRelativeTorque(ankleTorque);
	}
}