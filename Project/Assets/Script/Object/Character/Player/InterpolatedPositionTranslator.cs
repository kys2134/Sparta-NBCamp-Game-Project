using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class InterpolatedPositionTranslator : MonoBehaviour
    {
        #region SERIALIZABLE FIELD API

        [Header("Target Reference")]
        [Tooltip("The target transform, whose position values will be copied and smoothed.\n\n" +
                 "이동 값이 복사되고 부드럽게 조정될 트렌스폼 레퍼런스.")]
        [SerializeField] private Transform target;

        [Header("Method Settings")]
        [SerializeField] private UpdateMode updateType;

        [Tooltip("If the delay occurring during the process of estimating the rotation value and smoothing it is corrected, then true. otherwise, false.\n\n" +
                 "회전 값이 추정되어 부드럽게 처리하는 과정에서 발생하는 지연을 보정할 것인지 여부.")]
        [SerializeField] private bool isExtrapolated;

        [Header("Translation Settings")]
        [SerializeField] public InterpolationMode interpolationMode;

        [Tooltip("Speed that controls how fast the current position will be smoothed toward the target position when 'Lerp' is selected.\n\n" +
                 "'Lerp'가 선택되었을 때 현재 위치가 목표 위치로 얼마나 빠르게 부드럽게 이동될지를 제어하는 속도.")]
        [HideInInspector] public float speed = 20f;

        [Tooltip("Time that controls how fast the current position will be smoothed toward the target position when 'SmoothDamp' is selected.\n\n" +
                 "'SmoothDamp'가 선택되었을 때 현재 위치가 목표 위치로 얼마나 빠르게 부드럽게 이동될지를 제어하는 시간.")]
        [HideInInspector] public float time = 0.02f;

        #endregion

        private Vector3 _position;
        private Vector3 _localPosition;

        private Vector3 _velocity;

        private void Awake()
        {
            // If no target has been selected, choose this transform's parent as the target.
            if (target == null)
            {
                target = transform.parent;
            }

            _position = transform.position;
            _localPosition = transform.localPosition;
        }

        private void OnEnable()
        {
            // Convert local position offset to world coordinates system.
            Vector3 offset = transform.localToWorldMatrix * _localPosition;

            // Add position offset and set current position.
            _position = target.position + offset;
        }

        private void Update()
        {
            if (updateType == UpdateMode.Update)
            {
                return;
            }

            Interpolate();
        }

        private void LateUpdate()
        {
            if (updateType == UpdateMode.LateUpdate)
            {
                return;
            }

            Interpolate();
        }

        private void Interpolate()
        {
            var a = _position;
            var b = target.position;

            // Convert local position offset to world coordinates system.
            Vector3 offset = transform.localToWorldMatrix * _localPosition;

            // If it chose to extrapolate, calculate a new target rotation.
            if (isExtrapolated)
            {
                var difference = b - (a - offset);
                b += difference;
            }

            // Add local position offset to target.
            b += offset;

            // Smooth (based on chosen smooth mode) and return position.
            _position = interpolationMode switch
            {
                InterpolationMode.Lerp => Vector3.Lerp(a, b, speed * Time.deltaTime),
                InterpolationMode.SmoothDamp => Vector3.SmoothDamp(a, b, ref _velocity, time),
                _ => Vector3.zero
            };

            // Set position in transform.
            transform.position = _position;
        }

        #region NESTED ENUMERATION API

        public enum InterpolationMode
        {
            Lerp,
            SmoothDamp
        }

        #endregion
    }
}
