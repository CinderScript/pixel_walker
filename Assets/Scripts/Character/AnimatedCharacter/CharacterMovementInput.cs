/**
 *	Project:		Pixel Walker
 *	
 *	Description:	CharacterMovementValues is a class that holds the values
 *					read by the CharacterController to signal that movement
 *					should be performed for both body movement and hand
 *					movement.
 *					
 *	Author:			Pixel Walker -
 *						Maynard, Gregory
 *						Shubhajeet, Baral
 *						Do, Khuong
 *						Nguyen, Thuong						
 *					
 *	Date:			05-23-2022
 */

using UnityEngine;

[DisallowMultipleComponent]
public class CharacterMovementInput : MonoBehaviour
{
	[Header("Base Movement Values")]
	public int bodyForwardMovement;
	public int bodyRotation;

	[Header("Hand Movement Values")]
	public int handForwardMovement;
	public int handSideMovement;
	public int handVerticalMovement;
}