using System;
using System.Collections;
using Backend.Util.Debug;
using Backend.Util.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Backend.Object.Character.Player
{
    public partial class AdvancedActionController
    {
        private PlayerControls _actions;
        private Vector3 _direction = Vector3.zero;

        private float _lastJumpButtonUsed;

        private float _isRollButtonBuffered;
        private float _lastRollButtonUsed;

        public event Action<Vector3> OnJump;
        public event Action<Vector3> OnLand;
        public event Action<Vector3> OnRoll;

        private void Move(InputAction.CallbackContext context)
        {
            _direction = context.ReadValue<Vector3>().normalized;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (_state != State.Grounded)
            {
                return;
            }

            if (Time.time < _lastJumpButtonUsed + jumpDuration)
            {
                return;
            }

            TranslateToAirborne();

            _momentum = ConvertMomentumToWorldSpace();

            // Add jump force to momentum.
            _momentum += transform.up * jumpSpeed;

            // Set jump start time.
            _lastJumpButtonUsed = Time.time;

            OnJump?.Invoke(_momentum);

            _momentum = ConvertMomentumToLocalSpace();

            _state = State.Jumping;
        }

        private void Roll(InputAction.CallbackContext context)
        {
            Debugger.LogProgress();

            if (_state != State.Grounded)
            {
                return;
            }

            if (Time.time < _lastRollButtonUsed + rollingDuration)
            {
                return;
            }

            _animationController.SetAnimationBoolean("Is Rolling", true);
        }

        public void OnRollingStateEntered()
        {
            Debugger.LogProgress();

            _state = State.Rolling;

            _actions.Movement.Disable();

            _direction = _facing;
        }

        public void OnRollingStateExited()
        {
            Debugger.LogProgress();

            _animationController.SetAnimationBoolean("Is Rolling", false);

            _direction = Vector3.zero;

            _actions.Movement.Enable();

            _state = State.Grounded;
        }

        private void Stop(InputAction.CallbackContext context)
        {
            _direction = Vector3.zero;
        }
    }
}
