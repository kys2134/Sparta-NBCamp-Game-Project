using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class InterpolatedRotationTranslator : MonoBehaviour
    {
        #region SERIALIZABLE FIELD API

        [Header("Target Reference")]
        [Tooltip("The target transform reference, whose rotation values will be copied and smoothed\n\n." +
                 "회전 값이 복사되고 부드럽게 조정될 트렌스폼 레퍼런스.")]
        [SerializeField] private Transform target;

        [Header("Method Settings")]
        [SerializeField] private UpdateMode updateMode;

        [Tooltip("If the delay occurring during the process of estimating the rotation value and smoothing it is corrected, then true. otherwise, false.\n\n" +
                 "회전 값이 추정되어 부드럽게 처리하는 과정에서 발생하는 지연을 보정할 것인지 여부.")]
        [SerializeField] private bool isExtrapolated;

        [Header("Translation Settings")]
        [Tooltip("Speed that controls how fast the current rotation will be smoothed toward the target rotation.\n\n" +
                 "현재 회전 속도를 목표 회전 속도로 얼마나 빠르게 부드럽게 조정할지를 제어하는 속도.")]
        [SerializeField] private float speed = 20f;

        #endregion

        private Quaternion _rotation;

        private void Awake()
        {
            // If no target has been selected, choose this transform's parent as target.
            if (target == null)
            {
                target = transform.parent;
            }

            _rotation = transform.rotation;
        }

        private void OnEnable()
        {
            // Reset current rotation when game object is re-enabled to prevent unwanted interpolation from last rotation.
            _rotation = target.rotation;
        }

        private void Update()
        {
            if (updateMode == UpdateMode.LateUpdate)
            {
                return;
            }

            Interpolate();
        }

        private void LateUpdate()
        {
            if (updateMode == UpdateMode.Update)
            {
                return;
            }

            Interpolate();
        }

        /// <summary>
        /// Smooth a rotation toward a target rotation based on given speed.
        /// </summary>
        private void Interpolate()
        {
            var a = _rotation;
            var b = target.rotation;

            // If it chose to extrapolate, calculate a new target rotation.
            if (isExtrapolated && Quaternion.Angle(a, b) < 90f)
            {
                var difference = b * Quaternion.Inverse(a);
                b *= difference;
            }

            // Smooth current rotation.
            _rotation = Quaternion.Slerp(a, b, speed * Time.deltaTime);

            // Set rotation in transform.
            transform.rotation = _rotation;
        }
    }
}
