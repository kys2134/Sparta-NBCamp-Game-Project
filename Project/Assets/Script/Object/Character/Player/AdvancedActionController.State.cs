using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Backend.Object.Character.Player
{
    public partial class AdvancedActionController
    {
        private Dictionary<State, List<Node>> _states;
        private State _state = State.Falling;

        private void InitializeStates()
        {
            _states = new Dictionary<State, List<Node>>
            {
                [State.Grounded] = new()
                {
                    new Node
                    {
                        Next = State.Rising,
                        Condition = ShouldRise,
                        Transition = TranslateToAirborne
                    },
                    new Node
                    {
                        Next = State.Falling,
                        Condition = ShouldFall,
                        Transition = TranslateToAirborne
                    },
                    new Node
                    {
                        Next = State.Sliding,
                        Condition = ShouldSlide,
                        Transition = TranslateToAirborne
                    }
                },
                [State.Sliding] = new()
                {
                    new Node
                    {
                        Next = State.Rising,
                        Condition = ShouldRise,
                        Transition = TranslateToAirborne
                    },
                    new Node
                    {
                        Next = State.Falling,
                        Condition = ShouldFall,
                        Transition = TranslateToAirborne
                    },
                    new Node
                    {
                        Next = State.Grounded,
                        Condition = () => _movementController.IsGrounded && ShouldSlide() == false,
                        Transition = TranslateToGrounded
                    }
                },
                [State.Falling] = new()
                {
                    new Node
                    {
                        Next = State.Rising,
                        Condition = ShouldRise,
                        Transition = null
                    },
                    new Node
                    {
                        Next = State.Grounded,
                        Condition = () => _movementController.IsGrounded && ShouldSlide() == false,
                        Transition = TranslateToGrounded
                    },
                    new Node { Next = State.Sliding, Condition = ShouldSlide }
                },
                [State.Rising] = new()
                {
                    new Node
                    {
                        Next = State.Grounded,
                        Condition = () => ShouldRise() == false && _movementController.IsGrounded && ShouldSlide() == false,
                        Transition = TranslateToGrounded
                    },
                    new Node
                    {
                        Next = State.Sliding,
                        Condition = () => ShouldRise() == false && ShouldSlide(),
                        Transition = null
                    },
                    new Node
                    {
                        Next = State.Falling,
                        Condition = () => ShouldRise() == false && ShouldFall(),
                        Transition = null
                    },
                    new Node
                    {
                        Next = State.Falling,
                        Condition = () => _detector != null && _detector.WasDetected,
                        Transition = ContractCeiling
                    }
                },
                [State.Jumping] = new()
                {
                    new Node
                    {
                        Next = State.Rising,
                        Condition = () => Time.time - _lastJumpButtonUsed > jumpDuration,
                        Transition = null
                    },
                    new Node
                    {
                        Next = State.Rising,
                        Condition = () => _actions.Movement.Jump.WasReleasedThisFrame(),
                        Transition = null
                    },
                    new Node
                    {
                        Next = State.Falling,
                        Condition = () => _detector != null && _detector.WasDetected,
                        Transition = ContractCeiling
                    }
                }
            };
        }

        /// <summary>
        /// Determine current controller state based on current momentum and whether the controller is grounded (or not).
        /// </summary>
        private State DetermineState()
        {
            if (_states.ContainsKey(_state) == false)
            {
                return _state;
            }

            foreach (Node transition in _states[_state].Where(transition => transition.Condition()))
            {
                transition.Transition?.Invoke();

                return transition.Next;
            }

            return _state;
        }

        /// <returns>
        /// True if vertical momentum is above a small threshold. otherwise false.
        /// </returns>
        private bool IsAirborne()
        {
            // Set up threshold to check against.
            // For most applications, a value of '0.001f' is recommended.
            const float limit = 0.001f;

            // Calculate current vertical momentum.
            var momentum = ConvertMomentumToWorldSpace().Project(transform.up);

            // Return true if vertical momentum is above limit.
            return momentum.magnitude > limit;
        }

        /// <returns>
        /// True if angle between controller and ground normal is too big (greater than slope limit), i.e. ground is too steep. otherwise false.
        /// </returns>
        private bool IsGroundTooSteep()
        {
            if (_movementController.IsGrounded == false)
            {
                return true;
            }

            return Vector3.Angle(_movementController.GetGroundNormal(), transform.up) > slopeLimit;
        }

        private bool ShouldRise()
        {
            return IsAirborne() && Vector3.Dot(ConvertMomentumToWorldSpace(), transform.up.normalized) > 0f;
        }

        private bool ShouldSlide()
        {
            return _movementController.IsGrounded && IsGroundTooSteep();
        }

        private bool ShouldFall()
        {
            return _movementController.IsGrounded == false;
        }

        /// <returns>
        /// True if controller is grounded (or sliding down a slope).
        /// </returns>
        public bool IsGrounded => _state is State.Grounded or State.Sliding;

        /// <returns>
        /// True if controller is sliding.
        /// </returns>
        public bool IsSliding() => _state == State.Sliding;

        public bool IsRolling => _state is State.Rolling;

        #region NESTED ENUMERATION API

        private enum State
        {
            Grounded,
            Sliding,
            Falling,
            Rising,
            Jumping,
            Rolling,
        }

        #endregion

        #region NESTED STRUCTURE API

        private class Node
        {
            public State Next { get; set; }

            public Func<bool> Condition { get; set; }

            public Action Transition { get; set; }
        }

        #endregion
    }
}
