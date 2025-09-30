using UnityEngine;

namespace Backend.Object.Character.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovementController : MovementController
    {
        #region CONSTANT FIELD API

        private const float RadiusModifier = 0.8f;

        #endregion

        #region SERIALIZABLE FIELD API

        [Header("Movement Settings")] [Range(0f, 1f)]
        [SerializeField] private float stepHeightRatio = 0.25f;

        [Header("Collider Settings")]
        [SerializeField] private float height = 2f;
        [SerializeField] private float thickness = 1f;
        [SerializeField] private Vector3 offset = Vector3.zero;

        [Header("Debug Settings")]
        [SerializeField] private bool isDebugMode;

        [Header("Detection Settings")]
        [SerializeField] public CastMode mode = CastMode.SingleRay;
        [HideInInspector] public int rows = 1;
        [HideInInspector] public int count = 6;
        [HideInInspector] public bool isOffset;

        [HideInInspector] public Vector3[] multipleRayPositions;

        #endregion

        private Collider _collider;
        private CapsuleCollider _capsuleCollider;
        private Sensor _sensor;

        private bool _useExtendedRange = true;
        private float _extendedRange;

        private int _layer;

        // Current upwards (or downwards) velocity necessary to keep the correct distance to the ground.
        private Vector3 _adjustmentVelocity = Vector3.zero;

        protected override void Awake()
        {
            base.Awake();

            SetUp();

            _sensor = new Sensor(transform, _collider);

            RecalculateColliderDimensions();
            RecalibrateSensor();
        }

#if UNITY_EDITOR

        private void LateUpdate()
        {
            if (isDebugMode)
            {
                _sensor.DrawDebug();
            }
        }

#endif

        private void Reset()
        {
            SetUp();
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            // Recalculate collider dimensions.
            if (gameObject.activeInHierarchy)
            {
                RecalculateColliderDimensions();
            }

            // Recalculate raycast array preview positions.
            if (mode == CastMode.MultipleRay)
            {
                multipleRayPositions = Sensor.GetRaycastOriginPositions(rows, 1f, count, isOffset);
            }
        }

#endif

        private void SetUp()
        {
            _collider = GetComponent<Collider>();

            // If no collider is attached to this instance, add a collider.
            if (_collider == null)
            {
                transform.gameObject.AddComponent<CapsuleCollider>();

                _collider = GetComponent<Collider>();
            }

            // If no rigidbody is attached to this instance, add a rigidbody.
            if (Rigidbody == null)
            {
                transform.gameObject.AddComponent<Rigidbody>();

                Rigidbody = GetComponent<Rigidbody>();
            }

            _capsuleCollider = GetComponent<CapsuleCollider>();

            // Freeze rigidbody rotation and disable rigidbody gravity.
            Rigidbody.freezeRotation = true;
            Rigidbody.useGravity = false;
        }

        /// <summary>
        /// Recalculate collider height, width and thickness.
        /// </summary>
        private void RecalculateColliderDimensions()
        {
            // Check if a collider is attached to this instance.
            if (_collider == null)
            {
                // Try to get a reference to the attached collider by calling set up.
                SetUp();
            }

            if (_capsuleCollider)
            {
                _capsuleCollider.height = height;
                _capsuleCollider.center = offset * height;
                _capsuleCollider.radius = thickness / 2f;

                _capsuleCollider.center += new Vector3(0f, stepHeightRatio * _capsuleCollider.height / 2f, 0f);
                _capsuleCollider.height *= 1f - stepHeightRatio;

                if (_capsuleCollider.height / 2f < _capsuleCollider.radius)
                {
                    _capsuleCollider.radius = _capsuleCollider.height / 2f;
                }
            }

            // Recalibrate sensor variables to fit new collider dimensions.
            if (_sensor != null)
            {
                RecalibrateSensor();
            }
        }

        /// <summary>
        /// Recalibrate sensor variables.
        /// </summary>
        private void RecalibrateSensor()
        {
            //Set sensor ray origin and direction.
            _sensor.Origin = ColliderCenter;
            _sensor.DirectionType = DirectionType.Down;

            // Calculate sensor layer mask.
            RecalculateSensorLayerMask();

            // Set sensor cast type;
            _sensor.CastMode = mode;

            // Calculate sensor radius/width.
            var radius = thickness / 2f * RadiusModifier;

            // Multiply all sensor lengths with distance factor to compensate for floating point errors.
            const float factor = 0.001f;

            // Fit collider height to sensor radius.
            if (_capsuleCollider)
            {
                radius = Mathf.Clamp(radius, factor, _capsuleCollider.height / 2f * (1f - factor));
            }

            // Set sensor radius.
            _sensor.Radius = radius * transform.localScale.x;

            // Calculate and set sensor length.
            var length = 0f;
            length += height * (1f - stepHeightRatio) * 0.5f;
            length += height * stepHeightRatio;
            _extendedRange = length * (1f + factor) * transform.localScale.x;
            _sensor.MaximumDistance = length * transform.localScale.x;

            // Set sensor array variables.
            _sensor.Option.Rows = rows;
            _sensor.Option.Count = count;
            _sensor.Option.IsOffset = isOffset;
            _sensor.IsDebugMode = isDebugMode;

            // Set sensor spherecast variables.
            _sensor.UseRealisticDistance = true;
            _sensor.UseRealisticSurfaceNormal = true;

            // Recalibrate sensor to the new values.
            _sensor.RecalibrateRaycastOriginPositions();
        }

        /// <summary>
        /// Recalculate sensor layer mask based on current physics settings.
        /// </summary>
        private void RecalculateSensorLayerMask()
        {
            var layerMask = 0;
            var layer = gameObject.layer;

            // Calculate layer mask.
            for (var i = 0; i < 32; i++)
            {
                if (Physics.GetIgnoreLayerCollision(layer, i) == false)
                {
                    layerMask |= 1 << i;
                }
            }

            // Make sure that the calculated layer mask does not include the 'Ignore Raycast' layer.
            if (layerMask == (layerMask | (1 << LayerMask.NameToLayer("Ignore Raycast"))))
            {
                layerMask ^= 1 << LayerMask.NameToLayer("Ignore Raycast");
            }

            // Set sensor layer mask.
            _sensor.LayerMask = layerMask;

            // Save current layer.
            _layer = layer;
        }

        /// <summary>
        /// Check if mover is grounded.
        /// Store all relevant collision information for later.
        /// Calculate necessary adjustment velocity to keep the correct distance to the ground.
        /// </summary>
        public void Check()
        {
            // Check if object layer has been changed since last frame.
            // If so, recalculate sensor layer mask.
            if (_layer != gameObject.layer)
            {
                RecalculateSensorLayerMask();
            }

            // Reset ground adjustment velocity.
            _adjustmentVelocity = Vector3.zero;

            // Set sensor length.
            if (_useExtendedRange)
            {
                _sensor.MaximumDistance = _extendedRange + (height * transform.localScale.x * stepHeightRatio);
            }
            else
            {
                _sensor.MaximumDistance = _extendedRange;
            }

            _sensor.Cast();

            // If sensor has not detected anything, set flags and return.
            if (_sensor.Hit.IsDetected == false)
            {
                IsGrounded = false;

                return;
            }

            // Set flags for ground detection.
            IsGrounded = true;

            // Get distance that sensor ray reached.
            var distance = _sensor.Hit.Distance;

            // Calculate how much mover needs to be moved up or down.
            var length = height * transform.localScale.x * (1f - stepHeightRatio) * 0.5f;
            var middle = length + (height * transform.localScale.x * stepHeightRatio);
            var difference = middle - distance;

            // Set new ground adjustment velocity for the next frame.
            _adjustmentVelocity = transform.up * (difference / Time.fixedDeltaTime);
        }

        /// <returns>
        /// True if mover is touching ground and the angle between hte 'up' vector and ground normal is not too steep.
        /// </returns>
        public bool IsGrounded { get; private set; }

        public Vector3 FacingDirection => _adjustmentVelocity;

        /// <summary>
        /// Set movement controller's velocity.
        /// </summary>
        public Vector3 Velocity { get => Rigidbody.velocity; set => Rigidbody.velocity = value + _adjustmentVelocity; }

        public bool UseExtendedRange { set => _useExtendedRange = value; }

        public float ColliderHeight
        {
            get => height;
            set
            {
                height = value;

                RecalculateColliderDimensions();
            }
        }

        /// <returns>
        /// The collider's center in world coordinates system.
        /// </returns>
        private Vector3 ColliderCenter
        {
            get
            {
                if (_collider == null)
                {
                    SetUp();
                }

                return _collider.bounds.center;
            }
        }

        public float ColliderThickness
        {
            get => thickness;
            set
            {
                if (value < 0f)
                {
                    value = 0f;
                }

                thickness = value;

                RecalculateColliderDimensions();
            }
        }

        public float StopHeightRatio
        {
            set
            {
                value = Mathf.Clamp(value, 0f, 1f);

                stepHeightRatio = value;

                RecalculateColliderDimensions();
            }
        }

        public Vector3 GetGroundNormal()
        {
            return _sensor.Hit.Normal;
        }

        public Vector3 GetGroundPoint()
        {
            return _sensor.Hit.Position;
        }

        public Collider GetGroundCollider()
        {
            return _sensor.Hit.Collider;
        }
    }
}
