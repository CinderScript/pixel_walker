/**
 * Unity Standard Assest - copyright Unity3D
 * Part of "Third Person Character Controller" package
 * 
 * Modified by Gregory Maynard:
 * Removed camera controlls to be added into a separate CameraController script.
 * Removed Jumping, Sprinting, Analog input magnitude.
 * 
 * Rotation of camera no longer affects the forward vector of movement. Character 
 * forward vector is based on the rotation of the character object, not the camera.
 * 
 * Added character rotation.
 * 
 * Forward walking values and rotation values are also changed from values between 
 * -1 and 1 to values between 0 and 1 for walking, and 0, 1, 2 for rotation. This
 * is so that this character controller can take in values directly from ml-agents
 * discrete action buffers.
 */

using UnityEngine;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterController_Animated : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("How fast the character turns")]
        public float RotationVelocity = 120;
        public float RotationSpeedAcceleration = 15f;

        [Tooltip("Acceleration and deceleration")]
        public float MoveAcceleration = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        // player
        private float _currentSpeed;
        private float _animationBlend;
        private float _currentRotationVelocity;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;

        private int _animIDMotionSpeed;

        private Animator _animator;
        private CharacterController _controller;
        private CharacterMovementValues _input;
        private GameObject _mainCamera;

        private bool _hasAnimator;


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<CharacterMovementValues>();

            AssignAnimationIDs();
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            GroundedCheck();
            Move();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void Move()
        {
            /** FORWARD MOVEMENT **/
            float targetSpeed = MoveSpeed;
            if (_input.forward == 0) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _currentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                    Time.deltaTime * MoveAcceleration);

                // round speed to 3 decimal places
                _currentSpeed = Mathf.Round(_currentSpeed * 1000f) / 1000f;
            }
            else
            {
                _currentSpeed = targetSpeed; // done accelerating
            }

            // move the player
            _controller.Move(transform.forward * (_currentSpeed * Time.deltaTime));

            /** ROTATION MOVEMENT **/
            float targetRotationVelocity = RotationVelocity;
            if (_input.rotate == 0) targetRotationVelocity = 0;

            if (_currentRotationVelocity < targetRotationVelocity - speedOffset ||
                _currentRotationVelocity > targetRotationVelocity + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _currentRotationVelocity = Mathf.Lerp(_currentRotationVelocity, targetRotationVelocity,
                    Time.deltaTime * RotationSpeedAcceleration);

                // round speed to 3 decimal places
                _currentRotationVelocity = Mathf.Round(_currentRotationVelocity * 1000f) / 1000f;
            }
            else
            {
                _currentRotationVelocity = targetRotationVelocity; // done accelerating
            }
            transform.Rotate(0, _currentRotationVelocity * Time.deltaTime, 0);

            /** ANIMATIONS **/
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * MoveAcceleration);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, 1);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}