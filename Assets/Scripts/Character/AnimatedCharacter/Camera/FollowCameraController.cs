/**
 * Taken from the Character"Unity Standard Assest - copyright Unity3D" Unity package.
 * Copyright Unity3D
 * 
 * Pixel Walker as created this code file by copying parts of the ThirdPersonController.cs 
 * file in order to separate the camera controls from the character controls for the  
 * application.
 */

using UnityEngine;

[DisallowMultipleComponent]
public class FollowCameraController : MonoBehaviour
{
    [Header("User Input")]
    public UserInputValues Input;
    public float LookSpeedMultiplier = 2.0f;

    [Header("Area of CameraTarget")]
    [Tooltip("This camera controller will use this PlayerArea to find the player to follow.")]
    public GameObject sceneArea;

    [Header("Camera Settings")]
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    [Header("Set at RTime")]
    //The target the Cinemachine Virtual Camera will follow
    public GameObject cinemachineTarget;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // input
    private const float _threshold = 0.01f;

    private Quaternion _startingRotation;

    void Awake()
	{
		// find the camera target in children (child of character armature)
		cinemachineTarget = sceneArea.GetComponentInChildren<CameraTarget>().gameObject;
		_startingRotation = cinemachineTarget.transform.rotation;		
	}

	void Start()
    {
		cinemachineTarget.transform.rotation = _startingRotation;

		_cinemachineTargetYaw = cinemachineTarget.transform.rotation.eulerAngles.y;
        _cinemachineTargetPitch = cinemachineTarget.transform.rotation.eulerAngles.x;
    }

    private void LateUpdate()
    {
        CameraRotation();
    }
	
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (Input.Look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            _cinemachineTargetYaw += Input.Look.x * LookSpeedMultiplier;
            _cinemachineTargetPitch += Input.Look.y * LookSpeedMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        cinemachineTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
