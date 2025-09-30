using System;
using UnityEngine;
using Backend.Util.Extension;

namespace Backend.Object.Character.Player
{
    public class CameraDistanceRaycaster : MonoBehaviour
    {
        #region SERIALIZABLE FIELD API

        [Header("Target Reference")]
        [Tooltip("The target transform reference that the camera will look at.\n\n" +
                 "카메라의 트랜스폼 참조.")]
        [SerializeField]
        private Transform cameraTransform;

        [Tooltip("The target transform reference that the camera mounted rig.\n\n" +
                 "카메라가 부착된 리그의 트랜스폼 참조.")]
        [SerializeField]
        private Transform cameraRigTransform;

        [Header("Controller Settings")]
        [Tooltip(
            "This value controls how smoothly the old camera distance will be interpolated toward the new distance. " +
            "Setting this value to '50f' (or above) will result in no (visible) smoothing at all. " +
            "Setting this value to '1f' (or below) will result in very noticeable smoothing. " +
            "In most cases, a value of '25f' is recommended.\n\n" +
            "해당 값은 기존 카메라 거리에서 새 거리로의 보간이 얼마나 부드럽게 이루어지는지를 제어한다. " +
            "이 값을 '50f'(또는 그 이상)로 설정하면 (눈에 띄는) 보간 효과가 전혀 발생하지 않는다. " +
            "이 값을 '1f'(또는 그 이하)로 설정하면 매우 뚜렷한 보간 효과가 발생한다. " +
            "대부분의 경우 '25f' 값을 권장한다.")]
        [SerializeField] private float smoothingFactor = 25f;

        [Header("Layer Overrides")]
        [Tooltip("Layer mask used for ray casting." +
                 "레이 캐스팅에 사용되는 레이어 마스크.\n\n")]
        [SerializeField] private LayerMask includeLayers = ~0;

        [Tooltip("List of colliders to exclude when ray casting.\n\n" +
                 "레이 캐스팅 시 제외할 콜라이더 목록.")]
        [SerializeField] private Collider[] excludeColliders;

        [Header("Cast Settings")]
        [Tooltip("Obstacle scanning cast method mode.\n\n" +
                 "장애물을 스캔할 캐스트 기능 모드.")]
        [SerializeField] public CastMode castMode;

        [Tooltip(
            "Additional distance which is added to the raycast's length to prevent the camera from clipping into level geometry. " +
            "For most situations, the default value of '0.1f' is sufficient. " +
            "You can try increasing this distance a bit if you notice a lot of clipping. " +
            "This value is only used when 'Raycast' is selected.\n\n" +
            "레이케스트 길이에 추가되는 거리로, 카메라가 레벨 지오메트리에 클리핑되는 것을 방지한다. " +
            "대부분의 경우 기본값인 '0.1f'로 충분하다. " +
            "클리핑 현상이 많이 발생한다면 이 거리를 조금 늘려볼 수 있다. " +
            "이 값은 'Raycast'가 선택된 경우에만 사용된다.")]
        [HideInInspector] public float minimumCastingDistance = 0.1f;

        [Tooltip("Radius of spherical cast, used only when 'SphereCast' is selected.\n\n" +
                 "구형 탐지 반경, 'SphereCast'가 선택된 경우에만 사용된다.")]
        [HideInInspector] public float radius = 0.2f;

        #endregion

        private RaycastExcluder _excluder;

        private float _distance;

        private void Awake()
        {
            includeLayers = includeLayers.Remove(RaycastExcluder.ExcludeLayer);

            _excluder = new RaycastExcluder(excludeColliders);

            if (cameraTransform == null)
            {
                Debug.LogWarning("No camera transform has been assigned.", this);
            }

            if (cameraRigTransform == null)
            {
                Debug.LogWarning("No camera target transform has been assigned.", this);
            }

            // If the necessary transform references not have been assigned, disable this script.
            if (cameraTransform == null || cameraRigTransform == null)
            {
                enabled = false;

                return;
            }

            // Set initial starting distance.
            _distance = (cameraRigTransform.position - transform.position).magnitude;
        }

        private void LateUpdate()
        {
            _excluder.Apply();

            // Calculate current distance by casting a ray cast.
            var distance = GetCameraDistance();

            _excluder.Restore();

            // Linear interpolation is applied to the movement distance for smoother transitions.
            _distance = Mathf.Lerp(_distance, distance, smoothingFactor * Time.deltaTime);

            // Set new position of camera transform reference.
            cameraTransform.position = transform.position + (cameraRigTransform.position - transform.position).normalized * _distance;
        }

        /// <returns>
        /// Maximum distance by casting a ray (or sphere) from this transform to the camera rig transform.
        /// </returns>
        private float GetCameraDistance()
        {
            return castMode switch
            {
                CastMode.Raycast => GetDistanceByRayCast(),
                CastMode.SphereCast => GetDistanceBySphereCast(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private float GetDistanceByRayCast()
        {
            var direction = cameraRigTransform.position - transform.position;

            var ray = new Ray(transform.position, direction);
            var distance = direction.magnitude + minimumCastingDistance;

            if (Physics.Raycast(ray, out var hit, distance, includeLayers, QueryTriggerInteraction.Ignore) == false)
            {
                // If no obstacle was hit, return full distance.
                return direction.magnitude;
            }

            // Check if minimum distance from obstacles can be subtracted from hit distance, then return distance.
            if (hit.distance - minimumCastingDistance < 0f)
            {
                return hit.distance;
            }

            return hit.distance - minimumCastingDistance;
        }

        private float GetDistanceBySphereCast()
        {
            var direction = cameraRigTransform.position - transform.position;

            var ray = new Ray(transform.position, direction);
            var distance = direction.magnitude;

            var isHit = Physics.SphereCast(ray, radius, out var hit, distance, includeLayers, QueryTriggerInteraction.Ignore);

            // If no obstacle was hit, return full distance.
            return isHit ? hit.distance : direction.magnitude;
        }

        #region NESTED ENUMERATION API

        public enum CastMode
        {
            Raycast,
            SphereCast
        }

        #endregion
    }
}
