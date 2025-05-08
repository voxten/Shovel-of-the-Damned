using System;
using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(PlayerInput))]
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Crouch speed of the character in m/s")]
		public float CrouchSpeed = 2.0f;

		private float targetSpeed;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;
		public float decelerationRate = .4f;
		[Tooltip("Model for player")] 
		[SerializeField] private GameObject playerModel;
		
		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		public bool EnableJump;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Space(10)] 
		[Header("Crouch")] 
		[Header("Heights")] 
		public float normalHeight;
		public float crouchHeight;
		[Header("Centers")] 
		public float normalCenter;
		public float crouchCenter;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		[SerializeField] private Animator mainAnimator;
		[SerializeField] private Animator shadowAnimator;
        [SerializeField] private ItemCamera itemCamera;

        private bool _isTurned;
		
		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

	
		private PlayerInput _playerInput;
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private bool _newLocation;
		private bool _locationStop;
		private Transform _destination;
		private bool _canMove = true;
		private bool _canMoveCamera = true;
		
		private bool IsCurrentDeviceMouse
		{
			get
			{
				return _playerInput.currentControlScheme == "KeyboardMouse";
			}
		}
		
		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void OnEnable() 
		{
			PlayerEvents.SetLocation += SetLocation;
			PlayerEvents.StopLocationChange += StopLocationChange;
			PlayerEvents.TogglePlayerModel += TogglePlayerModel;
			PlayerEvents.ToggleController += ToggleController;
			PlayerEvents.ToggleMove += ToggleMovement;
			PlayerEvents.ToggleMoveCamera += ToggleMoveCamera;
			PlayerEvents.CheckMove += CheckMove;
		}

		private void OnDisable() 
		{
			PlayerEvents.SetLocation -= SetLocation;
			PlayerEvents.StopLocationChange -= StopLocationChange;
			PlayerEvents.TogglePlayerModel -= TogglePlayerModel;
			PlayerEvents.ToggleController -= ToggleController;
			PlayerEvents.ToggleMove -= ToggleMovement;
			PlayerEvents.ToggleMoveCamera -= ToggleMoveCamera;
			PlayerEvents.CheckMove -= CheckMove;
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
			_playerInput = GetComponent<PlayerInput>();

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		private void Update()
		{
			if(!_canMove) return;
			if (EnableJump)
			{
				JumpAndGravity();
			}
			GroundedCheck();
			Move();
		}

		private void LateUpdate()
		{
			if (_newLocation) {
				_controller.enabled = false;
				transform.position = _destination.position;
				_newLocation = false;
				_controller.enabled = true;
			}
			if(!_canMove) return;
			if (!_canMoveCamera) return;
			CameraRotation();
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);

				/*TODO: Zrobić ten turn postaci
				switch (_rotationVelocity)
				{
					case > 0.0f:
						mainAnimator.SetTrigger("RightTurn");
						break;
					case < 0.0f:
						mainAnimator.SetTrigger("LeftTurn");
						break;
				}
				*/
			}
		}

        private void Move()
        {
            bool isTryingToCrouch = _input.crouch;

            // Czy gracz może wstać?
            if (!isTryingToCrouch && _controller.height != normalHeight)
            {
                if (CanStandUp())
                {
                    _controller.height = normalHeight;
                    _controller.center = new Vector3(_controller.center.x, normalCenter, _controller.center.z);
                    mainAnimator.SetBool("Crouch", false);
                    shadowAnimator.SetBool("Crouch", false);
                }
                else
                {
                    // Blokuj wstawanie
                    isTryingToCrouch = true;
                    _input.crouch = true; // Wymuś ponowne kucanie
                }
            }

            // Aktualizacja wysokości podczas kucania
            if (isTryingToCrouch)
            {
                _controller.height = crouchHeight;
                _controller.center = new Vector3(_controller.center.x, crouchCenter, _controller.center.z);
                mainAnimator.SetBool("Crouch", true);
                shadowAnimator.SetBool("Crouch", true);
            }

            // --- USTALANIE PRĘDKOŚCI ---
            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }
            else if (isTryingToCrouch)
            {
                targetSpeed = CrouchSpeed;
            }
            else
            {
                targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            }

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // --- KIERUNEK RUCHU ---
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            if (_input.move != Vector2.zero)
            {
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }
            else if (currentHorizontalSpeed > 0)
            {
                _speed -= decelerationRate * Time.deltaTime;
                _speed = Mathf.Max(_speed, 0);
                inputDirection = _controller.velocity.normalized;
            }

            // --- RUCH POSTACI ---
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // --- ANIMACJE ---
            SetAnimations();

            if (itemCamera != null)
            {
                itemCamera.SetPlayerMoving(_input.move != Vector2.zero);
            }
        }


        private void SetAnimations()
		{
			mainAnimator.SetBool("Walk", _input.move != Vector2.zero);
			mainAnimator.SetBool("Sprint", _input.sprint);
			mainAnimator.SetBool("WalkLeft", _input.move.x < 0.0f);
			mainAnimator.SetBool("WalkRight", _input.move.x > 0.0f);
			mainAnimator.SetBool("WalkBack", _input.move.y < 0.0f);
			
			shadowAnimator.SetBool("Walk", _input.move != Vector2.zero);
			shadowAnimator.SetBool("Sprint", _input.sprint);
			shadowAnimator.SetBool("WalkLeft", _input.move.x < 0.0f);
			shadowAnimator.SetBool("WalkRight", _input.move.x > 0.0f);
			shadowAnimator.SetBool("WalkBack", _input.move.y < 0.0f);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void SetLocation(Transform destination) {
			if (_locationStop) {
				_newLocation = false;
				_locationStop = false;
				return;
			}
			_destination = destination;
			_newLocation = true;
		}

		private void StopLocationChange() {
			_locationStop = true;
		}
		
		private void TogglePlayerModel()
		{
			playerModel.SetActive(!playerModel.activeSelf);
		}

		private void ToggleController()
		{
			_controller.enabled = !_controller.enabled;
		}

		private void ToggleMovement() 
		{
			_canMove = !_canMove;
		}

		private bool CheckMove()
		{
			return _canMove && CanStandUp();
		}

		private void ToggleMoveCamera(bool state)
		{
			_canMoveCamera = state;
			_canMove = state;
		}
		
		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
		
		public static class PlayerEvents 
		{
			public static Action<Transform> SetLocation;
			public static Action StopLocationChange;
			public static Action TogglePlayerModel;
			public static Action ToggleController;
			public static Action ToggleMove;
			public static Func<bool> CheckMove;
			public static Action<bool> ToggleMoveCamera;
		}

        private bool CanStandUp()
        {
            float rayDistance = normalHeight; // ile "wolnej" przestrzeni potrzebujesz
            Vector3 rayStart = transform.position + Vector3.up * (crouchHeight + 0.05f); // start minimalnie nad crouchem

            // Strzel promieniem w górę
            return !Physics.Raycast(rayStart, Vector3.up, rayDistance, GroundLayers, QueryTriggerInteraction.Ignore);
        }
    }
}
