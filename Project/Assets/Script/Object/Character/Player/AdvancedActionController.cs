using System;
using Backend.Util.Input;
using UnityEngine;

namespace Backend.Object.Character.Player
{
    [RequireComponent(typeof(PlayerMovementController))]
    public partial class AdvancedActionController : MonoBehaviour
    {
        #region SERIALIZABLE FIELD API

        [Header("Target Reference")]
        [Tooltip("Optional camera transform used for calculating movement direction. If assigned, character movement will take camera view into account.\n\n" +
                 "움직임 방향 계산에 사용되는 선택적 카메라 트렌스폼 레퍼런스. 할당된 경우 캐릭터 움직임이 카메라 시점을 고려한다.")]
        [SerializeField] private Transform cameraTransform;

        [Header("Physics Settings")]
        [Tooltip("Movement speed.\n\n" +
                 "이동 속도.")]
        [SerializeField] private float movementSpeed = 7f;

        [Tooltip("The speed at which a character descends a steep slope.\n\n" +
                 "캐릭터가 가파른 경사면을 내려갈 때의 속도.")]
        [SerializeField] private float slidingSpeed = 5f;

        [Tooltip("Acceptable slope angle limit.\n\n" +
                 "허용 가능한 경사각 한계")]
        [SerializeField] private float slopeLimit = 80f;

        [Tooltip("How fast the controller can change direction while in the air. Higher values result in more air control.\n\n" +
                 "공중에 있을 때 컨트롤러가 방향을 전환할 수 있는 속도. 더 높은 값은 더 많은 제어력를 가진다.")]
        [SerializeField] private float airControlRate = 2f;

        [Tooltip("Amount of downward gravity.\n\n" +
                 "하향 중력의 양.")]
        [SerializeField] private float gravity = 30f;

        [Tooltip("Jump speed.\n\n" +
                 "점프 속도.")]
        [SerializeField] private float jumpSpeed = 10f;

        [Tooltip("Jump duration variables.\n\n" +
                 "점프 지속 시간")]
        [SerializeField] private float jumpDuration = 0.2f;

        [Tooltip("Rolling speed.\n\n" +
                 "구르기 속도")]
        [SerializeField] public float rollingSpeed = 1f;

        [Tooltip("rolling duration variables.\n\n" +
                 "구르기 지속 시간")]
        [SerializeField] private float rollingDuration = 0.2f;

        [Tooltip("Air friction determines how fast the controller loses its momentum while in the air.\n\n" +
                 "공기 마찰은 컨트롤러가 공중에 있을 때 운동량을 얼마나 빨리 잃는지를 결정한다.")]
        [SerializeField] private float airFriction = 0.5f;

        [Tooltip("Ground friction is used instead, if the controller is grounded.\n\n" +
                 "컨트롤러가 접지된 경우 접지 마찰이 대신 사용된다.")]
        [SerializeField] private float groundFriction = 100f;

        [Tooltip("If true calculate and apply momentum relative to the controller's transform.\n\n" +
                 "컨트롤러의 변환에 대한 상대적 운동량을 계산하고 적용할지 여부.")]
        [SerializeField] private bool useLocalSpace;

        #endregion

        private PlayerAnimationController _animationController;
        private PlayerMovementController _movementController;
        private CeilingDetector _detector;

        private Vector3 _momentum = Vector3.zero;
        private Vector3 _facing = Vector3.zero;

        private void Awake()
        {
            _animationController = GetComponent<PlayerAnimationController>();
            _movementController = GetComponent<PlayerMovementController>();
            _detector = GetComponent<CeilingDetector>();

            _actions = new PlayerControls();

            InitializeStates();
        }

        private void OnEnable()
        {
            _actions.Enable();
            _actions.Movement.Move.performed += Move;
            _actions.Movement.Move.canceled += Stop;
            _actions.Movement.Jump.performed += Jump;
            _actions.Movement.Roll.performed += Roll;
        }

        private void FixedUpdate()
        {
            // Check if mover is grounded.
            _movementController.Check();

            // Determine controller state.
            _state = DetermineState();

            // Apply friction and gravity to momentum.
            ApplyPhysicalForces();

            // Calculate movement velocity.
            var velocity = Vector3.zero;
            if (_state is State.Grounded or State.Rolling)
            {
                velocity = CalculateMovementVelocity();
            }

            // If local momentum is used, transform momentum into world space first.
            var momentum = _momentum;
            if (useLocalSpace)
            {
                momentum = transform.localToWorldMatrix * _momentum;
            }

            // Add current momentum to velocity.
            velocity += momentum;

            // If player is grounded or sliding on a slope, extend mover's sensor range.
            // This enables the player to walk up/downstairs and slopes without losing ground contact.
            _movementController.UseExtendedRange = IsGrounded;

            // Set mover velocity.
            _movementController.Velocity = velocity;

            // Store velocity for next frame.
            Velocity = velocity;

            // Save controller movement velocity.
            MovementVelocity = CalculateMovementVelocity();

            // Reset ceiling detector, if one is attached to this instance.
            _detector?.Refresh();

#if UNITY_EDITOR

            if (_movementController.Velocity.magnitude > 1f)
            {
                _velocity = _movementController.Velocity;
            }

#endif
        }

        private void OnDisable()
        {
            _actions.Movement.Move.performed -= Move;
            _actions.Movement.Move.canceled -= Stop;
            _actions.Movement.Jump.performed -= Jump;
            _actions.Movement.Roll.performed -= Roll;
            _actions.Disable();
        }

        /// <returns>
        /// Movement direction based on player input.
        /// </returns>
        private Vector3 CalculateMovementDirection()
        {
            if (_actions == null)
            {
                return Vector3.zero;
            }

            var direction = Vector3.zero;

            // If no camera transform has been assigned, use the character's transform axes to calculate the movement direction.
            if (cameraTransform == null)
            {
                direction += transform.right * _direction.x;
                direction += transform.forward * _direction.z;
            }
            else
            {
                // If a camera transform has been assigned, use the assigned transform's axes for movement direction.
                // Project movement direction so movement stays parallel to the ground.
                var v = Vector3.ProjectOnPlane(cameraTransform.forward, transform.up).normalized;
                var h = Vector3.ProjectOnPlane(cameraTransform.right, transform.up).normalized;
                direction += h * _direction.x;
                direction += v * _direction.z;
            }

            // If necessary, clamp movement vector to magnitude of '1f'.
            if (direction.magnitude > 1f)
            {
                direction.Normalize();
            }

            if (direction != Vector3.zero)
            {
                _facing = direction;
            }

            return direction;
        }

        /// <returns>
        /// Movement velocity based on player input, controller state, ground normal, etc.
        /// </returns>
        private Vector3 CalculateMovementVelocity()
        {
            // Calculate normalized movement direction and multiply normalized velocity with movement speed.
            var direction = CalculateMovementDirection();
            direction *= movementSpeed * rollingSpeed;

            return direction;
        }

        /// <summary>
        /// Apply friction to both vertical and horizontal momentum based on friction and gravity.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void ApplyPhysicalForces()
        {
            _momentum = ConvertMomentumToWorldSpace();

            // Set vertical and horizontal momentum.
            var v = Vector3.zero;
            var h = Vector3.zero;

            // Split momentum into vertical and horizontal components.
            if (_momentum != Vector3.zero)
            {
                v = _momentum.Project(transform.up);
                h = _momentum - v;
            }

            // Add gravity to vertical momentum.
            v -= transform.up * (gravity * Time.deltaTime);

            // Remove any downward force if the controller is grounded.
            if (_state == State.Grounded && Vector3.Dot(v, transform.up.normalized) < 0f)
            {
                v = Vector3.zero;
            }

            // Manipulate momentum to steer controller in the air (if controller is not grounded or sliding).
            if (IsGrounded == false)
            {
                var velocity = CalculateMovementVelocity();

                // If controller has received additional momentum from somewhere else.
                if (h.magnitude > movementSpeed)
                {
                    // Prevent unwanted accumulation of speed in the direction of the current momentum.
                    if (Vector3.Dot(velocity, h.normalized) > 0f)
                    {
                        velocity = velocity.Reject(h.normalized);
                    }

                    // Lower air control slightly with a multiplier to add some 'weight' to any momentum applied to the controller.
                    const float multiplier = 0.25f;
                    h += velocity * (airControlRate * multiplier * Time.deltaTime);
                }
                // If controller has not received additional momentum.
                else
                {
                    // Clamp _horizontal velocity to prevent accumulation of speed.
                    h += velocity * (airControlRate * Time.deltaTime);
                    h = Vector3.ClampMagnitude(h, movementSpeed);
                }
            }

            // Steer controller on slopes.
            if (_state == State.Sliding)
            {
                // Calculate vector pointing away from slope.
                var direction = Vector3.ProjectOnPlane(_movementController.GetGroundNormal(), transform.up).normalized;

                // Calculate movement velocity and remove all velocity that is pointing up the slope.
                var velocity = CalculateMovementVelocity();
                velocity = velocity.Reject(direction);

                // Add movement velocity to momentum.
                h += velocity * Time.fixedDeltaTime;
            }

            // Apply friction to horizontal momentum based on whether the controller is grounded;
            var friction = _state == State.Grounded ? groundFriction : airFriction;
            h = Vector3.MoveTowards(h, Vector3.zero, friction * Time.deltaTime);

            // Add horizontal and vertical momentum back together.
            _momentum = h + v;

            switch (_state)
            {
                // Additional momentum calculations for sliding.
                case State.Sliding:
                {
                    // Project the current momentum onto the current ground normal if the controller is sliding down a slope.
                    _momentum = Vector3.ProjectOnPlane(_momentum, _movementController.GetGroundNormal());

                    // Remove any upwards momentum when sliding.
                    if (Vector3.Dot(_momentum, transform.up.normalized) > 0f)
                    {
                        _momentum = _momentum.Reject(transform.up);
                    }

                    // Apply additional slide gravity.
                    var direction = Vector3.ProjectOnPlane(-transform.up, _movementController.GetGroundNormal()).normalized;
                    _momentum += direction * (slidingSpeed * Time.deltaTime);

                    break;
                }
                // If controller is jumping, override vertical velocity with jump speed.
                case State.Jumping:
                    _momentum = _momentum.Reject(transform.up);
                    _momentum += transform.up * jumpSpeed;
                    break;
                case State.Grounded:
                case State.Falling:
                case State.Rising:
                case State.Rolling:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _momentum = ConvertMomentumToLocalSpace();
        }

        /// <summary>
        /// This function is called when the controller has lost ground contact. (i.e. is either falling or rising, or generally in the air)
        /// </summary>
        private void TranslateToAirborne()
        {
            _momentum = ConvertMomentumToWorldSpace();

            // Get current movement velocity.
            var velocity = MovementVelocity;

            // Check if the controller has both momentum and a current movement velocity.
            if (velocity.sqrMagnitude >= 0f && _momentum.sqrMagnitude > 0f)
            {
                // Project momentum onto movement direction.
                var momentum = Vector3.Project(_momentum, velocity.normalized);

                // Calculate dot product to determine whether momentum and movement are aligned.
                var dot = Vector3.Dot(momentum.normalized, velocity.normalized);

                // If current momentum is already pointing in the same direction as movement velocity,
                // don't add further momentum (or limit movement velocity) to prevent unwanted speed accumulation.
                if (momentum.sqrMagnitude >= velocity.sqrMagnitude && dot > 0f)
                {
                    velocity = Vector3.zero;
                }
                else if (dot > 0f)
                {
                    velocity -= momentum;
                }
            }

            // Add movement velocity to momentum.
            _momentum += velocity;

            _momentum = ConvertMomentumToLocalSpace();
        }

        /// <summary>
        /// This function is called when the controller has landed on a surface after being in the air.
        /// </summary>
        private void TranslateToGrounded()
        {
            if (OnLand == null)
            {
                return;
            }

            var momentum = _momentum;

            // If local momentum is used, transform momentum into world coordinates first.
            if (useLocalSpace)
            {
                momentum = transform.localToWorldMatrix * momentum;
            }

            OnLand.Invoke(momentum);
        }

        /// <summary>
        /// This function is called when the controller has collided with a ceiling while jumping or moving upwards.
        /// </summary>
        private void ContractCeiling()
        {
            _momentum = ConvertMomentumToWorldSpace();

            // Remove all vertical parts of momentum.
            _momentum = _momentum.Reject(transform.up);

            _momentum = ConvertMomentumToLocalSpace();
        }

        private Vector3 ConvertMomentumToWorldSpace()
        {
            var momentum = _momentum;

            // If local momentum is used, transform momentum into world coordinates system first.
            if (useLocalSpace)
            {
                momentum = transform.localToWorldMatrix * _momentum;
            }

            return momentum;
        }

        private Vector3 ConvertMomentumToLocalSpace()
        {
            var momentum = _momentum;

            if (useLocalSpace)
            {
                momentum = transform.worldToLocalMatrix * _momentum;
            }

            return momentum;
        }

        public Vector3 Velocity { get; private set; } = Vector3.zero;

        public Vector3 MovementVelocity { get; private set; } = Vector3.zero;

        /// <summary>
        /// Add momentum to controller.
        /// </summary>
        public void AddMomentum(Vector3 momentum)
        {
            _momentum = ConvertMomentumToWorldSpace();

            _momentum += momentum;

            _momentum = ConvertMomentumToLocalSpace();
        }

        /// <param name="momentum">Controller momentum directly.</param>
        public void SetMomentum(Vector3 momentum)
        {
            if (useLocalSpace)
            {
                _momentum = transform.worldToLocalMatrix * momentum;
            }
            else
            {
                _momentum = momentum;
            }
        }
    }
}
