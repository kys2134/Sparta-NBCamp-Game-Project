using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class ThirdPersonCameraController : CameraController
    {
        #region SERIALIZABLE FIELD API

        [Header("Controller Reference")]
        [SerializeField] private AdvancedActionController controller;

        [Header("3th Person Settings")]
        [Tooltip("The general rate at which the camera turns toward the movement direction.\n\n" +
                 "카메라가 움직임 방향으로 회전하는 일반적인 속도.")]
        [SerializeField] private float turningTowardSpeed = 120f;

        [Tooltip("The maximum expected movement speed of this game object. " +
                 "This value should be set to the maximum movement speed achievable by this instance. " +
                 "The closer the current movement speed is to given maximum movement speed, the faster the camera will turn. " +
                 "As a result, if the instance moves slower (i.e. 'walking' instead of 'running', in case of a character), the camera will turn slower as well\n\n" +
                 "이 게임 오브젝트의 최대 예상 이동 속도. " +
                 "이 값은 이 인스턴스가 도달할 수 있는 최대 이동 속도로 설정되어야 한다. " +
                 "현재 이동 속도가 주어진 최대 이동 속도에 가까울수록 카메라 회전 속도가 빨라진다. " +
                 "결과적으로, 인스턴스의 움직임이 느려지면(예: 캐릭터의 경우 '달리기' 대신 '걷기' 상태), 카메라 회전 속도도 함께 느려진다.")]
        [SerializeField] private float maximumMovementSpeed = 7f;

        [Tooltip("Whether the camera turns towards the controller's movement direction.\n\n" +
                 "카메라가 컨트롤러의 이동 방향을 향하는지 여부.")]
        [SerializeField] private bool isTurningToward = true;

        [SerializeField] public Transform target;
        [SerializeField] private LayerMask layerMask;

        #endregion

        protected override void Update()
        {
            base.Update();

            if (target != null)
            {
                RotateTowardPosition(target.position, turningTowardSpeed);
            }
        }

        protected override void SetUp()
        {
            if (controller == null)
            {
                Debug.LogWarning("No controller reference has been assigned to this script.", gameObject);
            }
        }

        protected override void Rotate()
        {
            // Execute normal camera rotation code.
            base.Rotate();

            if (controller == null)
            {
                return;
            }

            if (isTurningToward == false || controller == null)
            {
                return;
            }

            // Get controller velocity.
            var velocity = controller.Velocity;

            RotateInDirectionOfVelocity(velocity, turningTowardSpeed);
        }

        /// <summary>
        /// Rotate camera toward direction, at the rate of _speed, around the upwards vector of this instance.
        /// </summary>
        private void RotateInDirectionOfVelocity(Vector3 velocity, float speed)
        {
            //Remove any unwanted components of direction.
            velocity = velocity.Reject(UpDirection);

            // Calculate angle difference of current direction and new direction.
            var angle = FacingDirection.SignedAngle(velocity, UpDirection);

            // Calculate sign of angle.
            var sign = Mathf.Sign(angle);

            // Calculate final angle difference.
            var difference = sign * Mathf.Abs(angle / 90f) * speed * Time.deltaTime;

            // If angle is greater than 90 degrees, recalculate final angle difference.
            if (Mathf.Abs(angle) > 90f)
            {
                difference = Time.deltaTime * speed * sign * (Mathf.Abs(180f - Mathf.Abs(angle)) / 90f);
            }

            // Check if calculated angle overshoots.
            if (Mathf.Abs(difference) > Mathf.Abs(angle))
            {
                difference = angle;
            }

            // Take movement speed into account by comparing it to maximum movement speed.
            difference *= Mathf.InverseLerp(0f, maximumMovementSpeed, velocity.magnitude);

            Angles = new Vector2(Angles.x, Angles.y + difference);
        }

        public PerspectiveMode Mode => target != null ? PerspectiveMode.LockOn : PerspectiveMode.ThirdPerson;
    }
}
