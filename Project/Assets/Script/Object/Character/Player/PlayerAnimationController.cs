using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class PlayerAnimationController : AnimationController
    {
        #region CONSTANT FIELD API

        private const float SmoothingFactor = 40f;

        #endregion

        #region SERIALIZABLE FIELD API

        [Tooltip("Whether the character is using the strafing blend tree.\n\n" +
                 "캐릭터가 스트레이핑 블렌드 트리를 사용하고 있는지 여부.")]
        [SerializeField] private bool useStrafeAnimations;

        [Tooltip("Velocity threshold for landing animation. Animation will only be triggered if downward velocity exceeds this threshold.\n\n" +
                 "착륙 애니메이션의 속도 임계값. 하향 속도가 이 임계값을 초과할 때만 애니메이션이 트리거된다.")]
        [SerializeField] private float landVelocityThreshold = 5f;

        #endregion

        private AdvancedActionController _controller;
        private ThirdPersonCameraController _camera;
        private Animator _animator;
        private Transform _transform;

        private Vector3 _cache = Vector3.zero;

        protected override void Awake()
        {
            _controller = GetComponent<AdvancedActionController>();
            _camera = GetComponentInChildren<ThirdPersonCameraController>();

            Animator = GetComponentInChildren<Animator>();
            _transform = Animator.transform;
        }

        private void OnEnable()
        {
            // Connect events to controller events.
            _controller.OnLand += Land;
            _controller.OnJump += Jump;
        }

        private void Update()
        {
            // Get controller velocity.
            var velocity = _controller.Velocity;

            // Split up velocity.
            var h = velocity.Reject(transform.up);
            var v = velocity - h;

            // Smooth horizontal velocity for fluid animation.
            h = Vector3.Lerp(_cache, h, SmoothingFactor * Time.deltaTime);
            _cache = h;

            SetAnimationFloat("Vertical Speed", v.magnitude * Vector3.Dot(v.normalized, transform.up.normalized));
            SetAnimationFloat("Horizontal Speed", h.magnitude);

            // If animator is strafing, split up horizontal velocity.
            if (useStrafeAnimations)
            {
                Vector3 localVelocity = _transform.InverseTransformVector(h);

                SetAnimationFloat("Forward Speed", localVelocity.z);
                SetAnimationFloat("Strafe Speed", localVelocity.x);
            }

            //Pass values to animator;
            SetAnimationBoolean("Is Grounded", _controller.IsGrounded);
            SetAnimationBoolean("Is Strafing", _camera.Mode == PerspectiveMode.LockOn);
        }

        private void OnDisable()
        {
            // Disconnect events to prevent calls to disabled instance.
            _controller.OnLand -= Land;
            _controller.OnJump -= Jump;
        }

        private void Land(Vector3 velocity)
        {
            // Only trigger animation if downward velocity exceeds threshold.
            if (Vector3.Dot(velocity, transform.up.normalized) > -landVelocityThreshold)
            {
                return;
            }

            SetAnimationTrigger("Land");
        }

        private void Jump(Vector3 velocity)
        {

        }
    }
}
