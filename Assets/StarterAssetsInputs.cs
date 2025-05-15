using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool crouch;
        [Tooltip("Camera for crouch movement")]
        public CinemachineCamera crouchCamera;
        public bool interaction;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        private bool _canMove = true;
        private bool _isTransitioning = false;
        private Coroutine _crouchTransitionRoutine;
        private Vector2 _bufferedMoveInput;

        public void OnMove(InputValue value)
        {
            Vector2 inputValue = value.Get<Vector2>();
            
            if (_canMove)
            {
                // Apply movement immediately if allowed
                MoveInput(inputValue);
                _bufferedMoveInput = Vector2.zero; // Clear buffer if we're moving
            }
            else
            {
                // Buffer the input if we're in transition
                _bufferedMoveInput = inputValue;
                MoveInput(Vector2.zero); // Stop current movement
            }
        }

        public void OnLook(InputValue value)
        {
            if(cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnInteract(InputValue value)
        {
            InteractionInput(value.isPressed);
        }

        public void OnCrouch(InputValue value)
        {
            if (value.isPressed && !_isTransitioning)
            {
                CrouchInput();
            }
        }

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        } 

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void InteractionInput(bool newInteractionState)
        {
            interaction = newInteractionState;
        }

        public void CrouchInput()
        {
            if (!FirstPersonController.PlayerEvents.CheckMove() || !FirstPersonController.PlayerEvents.CheckCrouch()) 
                return;
            
            // Only allow crouch/stand when not moving (but buffer will handle held inputs)
            if (move.magnitude > 0.1f && !_isTransitioning)
                return;

            // If a transition is already running, stop it
            if (_crouchTransitionRoutine != null)
            {
                StopCoroutine(_crouchTransitionRoutine);
            }

            _crouchTransitionRoutine = StartCoroutine(CrouchTransition());
        }

        private IEnumerator CrouchTransition()
        {
            _isTransitioning = true;
            _canMove = false;

            // Store current move input to prevent sudden stops
            Vector2 preTransitionMove = move;
            MoveInput(Vector2.zero);

            if (crouch)
            {
                // Stand up
                crouch = false;
                CameraSwitch.CameraEvents.SwitchCameraToDefaultWithTime(0.75f);
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                // Crouch down
                crouch = true;
                CameraSwitch.CameraEvents.SwitchCamera(crouchCamera);
                yield return new WaitForSeconds(0.75f);
            }

            _canMove = true;
            _isTransitioning = false;
            _crouchTransitionRoutine = null;

            // Apply buffered input if any exists
            if (_bufferedMoveInput.magnitude > 0.1f)
            {
                MoveInput(_bufferedMoveInput);
                _bufferedMoveInput = Vector2.zero;
            }
            else if (preTransitionMove.magnitude > 0.1f)
            {
                // If no buffered input but was moving before transition
                MoveInput(preTransitionMove);
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}