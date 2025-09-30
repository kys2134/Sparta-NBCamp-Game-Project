using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Object.Character.Player
{
    public class Sensor
    {
        private readonly Transform _transform;

        private readonly RaycastExcluder _excluder;

        // Starting point of ray cast.
        private Vector3 _origin = Vector3.zero;
        private DirectionType _directionType;

        // Backup normal used for specific edge cases when using sphere casts.
        private Vector3 _cache;

        private Vector3[] _origins;
        private readonly List<Vector3> _points = new ();
        private readonly List<Vector3> _normals = new ();

        public Sensor(Transform transform, Collider collider)
        {
            _transform = transform;

            if (collider == null)
            {
                return;
            }

            _excluder = new RaycastExcluder(new [] { collider });
        }

        /// <summary>
        /// Cast a ray (or sphere or array of rays) to check for colliders.
        /// </summary>
        public void Cast()
        {
            Refresh();

            // Calculate origin and direction of ray in world coordinates system.
            var origin = _transform.TransformPoint(Origin);
            var direction = Direction;

            _excluder.Apply();

            // Depending on the chosen mode of detection, call different functions to check for colliders.
            switch (CastMode)
            {
                case CastMode.SingleRay:
                    CastBySingleRay(origin, direction);
                    break;
                case CastMode.Sphere:
                    CastBySphere(origin, direction);
                    break;
                case CastMode.MultipleRay:
                    CastByMultipleRay(origin, direction);
                    break;
                default:
                    Hit.IsDetected = false;
                    break;
            }

            _excluder.Restore();
        }

        /// <summary>
        /// Cast a single ray into direction from origin position.
        /// </summary>
        private void CastBySingleRay(Vector3 origin, Vector3 direction)
        {
            Hit.IsDetected = Physics.Raycast(origin, direction, out var hit, MaximumDistance, LayerMask, QueryTriggerInteraction.Ignore);

            if (Hit.IsDetected == false)
            {
                return;
            }

            Hit.Position = hit.point;
            Hit.Normal = hit.normal;
            Hit.Distance = hit.distance;

            Hit.Colliders.Add(hit.collider);
            Hit.Transforms.Add(hit.transform);
        }

        /// <summary>
        /// Cast an array of rays into direction and centered around origin.
        /// </summary>
        private void CastByMultipleRay(Vector3 origin, Vector3 direction)
        {
            // Clear results from last frame.
            _normals.Clear();
            _points.Clear();

            // Calculate origin and direction of ray in world coordinates system.
            var position = Vector3.zero;
            var rayDirection = Direction;

            for (var i = 0; i < _origins.Length; i++)
            {
                // Calculate ray origin position.
                position = origin + _transform.TransformDirection(_origins[i]);

                var isHit = Physics.Raycast(position, rayDirection, out var hit, MaximumDistance, LayerMask, QueryTriggerInteraction.Ignore);
                if (isHit == false)
                {
                    continue;
                }

#if UNITY_EDITOR

                if (IsDebugMode)
                {
                    Debug.DrawRay(hit.point, hit.normal, Color.red, Time.fixedDeltaTime * 1.01f);
                }

#endif

                Hit.Colliders.Add(hit.collider);
                Hit.Transforms.Add(hit.transform);

                _normals.Add(hit.normal);
                _points.Add(hit.point);
            }

            Hit.IsDetected = _points.Count > 0;

            if (Hit.IsDetected == false)
            {
                return;
            }

            // Calculate average surface normal.
            var normal = _normals.Aggregate(Vector3.zero, (current, v) => current + v);
            normal.Normalize();

            //Calculate average surface point;
            var point = _points.Aggregate(Vector3.zero, (current, p) => current + p);
            point /= _points.Count;

            Hit.Position = point;
            Hit.Normal = normal;
            Hit.Distance = (origin - Hit.Position).Project(direction).magnitude;
        }

        private void CastBySphere(Vector3 origin, Vector3 direction)
        {
            var distance = MaximumDistance - Radius;

            Hit.IsDetected = Physics.SphereCast(origin, Radius, direction, out var hit, distance, LayerMask, QueryTriggerInteraction.Ignore);

            if (Hit.IsDetected == false)
            {
                return;
            }

            Hit.Position = hit.point;
            Hit.Normal = hit.normal;

            Hit.Distance = hit.distance;
            Hit.Distance += Radius;

            Hit.Colliders.Add(hit.collider);
            Hit.Transforms.Add(hit.transform);

            // Calculate real distance.
            if (UseRealisticDistance)
            {
                Hit.Distance = (origin - Hit.Position).Project(direction).magnitude;
            }

            var collider = Hit.Collider;

            // Calculate real surface normal by casting an additional ray cast.
            if (UseRealisticSurfaceNormal == false)
            {
                return;
            }

            var ray = new Ray(Hit.Position - direction, direction);
            if (collider.Raycast(ray, out hit, 1.5f))
            {
                Hit.Normal = Vector3.Angle(hit.normal, -direction) >= 89f ? _cache : hit.normal;
            }
            else
            {
                Hit.Normal = _cache;
            }

            _cache = Hit.Normal;
        }

        /// <summary>
        /// Reset all variables related to storing information on ray cast hits.
        /// </summary>
        private void Refresh()
        {
            Hit.Position = Vector3.zero;
            Hit.Normal = -Direction;
            Hit.Distance = 0f;

            Hit.IsDetected = false;

            Hit.Transforms.Clear();
            Hit.Colliders.Clear();
        }

#if UNITY_EDITOR

        /// <summary>
        /// Draw debug information in editor (hit positions and ground surface normals).
        /// </summary>
        public void DrawDebug()
        {
            const float size = 0.2f;

            if (Hit.IsDetected == false || IsDebugMode == false)
            {
                return;
            }

            var position = Hit.Position;
            var color = Color.red;
            var time = Time.deltaTime;
            Debug.DrawLine(position, position + Hit.Normal, color, time);

            color = Color.green;
            Debug.DrawLine(position + (Vector3.up * size), position - (Vector3.up * size), color, time);
            Debug.DrawLine(position + (Vector3.right * size), position - (Vector3.right * size), color, time);
            Debug.DrawLine(position + (Vector3.forward * size), position - (Vector3.forward * size), color, time);
        }

#endif

        /// <summary>
        /// Set the position for the raycast to start from.
        /// </summary>
        public Vector3 Origin
        {
            get => _origin;
            set
            {
                if (_transform == null)
                {
                    return;
                }

                _origin = _transform.InverseTransformPoint(value);
            }
        }

        /// <summary>
        /// Set axis of this instance's transform will be used as the direction for the raycast.
        /// </summary>
        public DirectionType DirectionType
        {
            set
            {
                if (_transform == null)
                {
                    return;
                }

                _directionType = value;
            }
        }

        /// <returns>
        /// Calculate a direction in world coordinates based on the local axes of this instance's transform component.
        /// </returns>
        private Vector3 Direction
        {
            get => _directionType switch
            {
                DirectionType.Up => _transform.up,
                DirectionType.Down => -_transform.up,
                DirectionType.Left => -_transform.right,
                DirectionType.Right => _transform.right,
                DirectionType.Forward => _transform.forward,
                DirectionType.Backward => -_transform.forward,
                _ => Vector3.one
            };
        }

        /// <summary>
        /// Recalculate start positions for the raycast array
        /// </summary>
        public void RecalibrateRaycastOriginPositions()
        {
            _origins = GetRaycastOriginPositions(Option.Rows, Radius, Option.Count, Option.IsOffset);
        }

        /// <returns>
        /// An array containing the starting positions of all array rays (in local coordinates system) based on the input arguments.
        /// </returns>
        public static Vector3[] GetRaycastOriginPositions(int rows, float radius, int count, bool isOffset)
        {
            // Initialize list used to store the positions.
            // Add central start position to the list.
            var positions = new List<Vector3> { Vector3.zero };

            for (var i = 0; i < rows; i++)
            {
                // Calculate radius for all positions on this row.
                var r = (float)(i + 1) / rows;

                for (var j = 0; j < count * (i + 1); j++)
                {
                    // Calculate angle (in degrees) for this individual position.
                    var angle = 360f / (count * (i + 1)) * j;

                    //If 'offsetRows' is set to 'true', every other row is offset;
                    if (isOffset && i % 2 == 0)
                    {
                        angle += 360f / (count * (i + 1)) / 2f;
                    }

                    // Combine radius and angle into one position and add it to the list.
                    var x = r * Mathf.Cos(Mathf.Deg2Rad * angle);
                    var y = r * Mathf.Sin(Mathf.Deg2Rad * angle);

                    positions.Add(new Vector3(x, 0f, y) * radius);
                }
            }

            // Convert list to array and return array.
            return positions.ToArray();
        }

        public CastMode CastMode { get; set; } = CastMode.SingleRay;
        public LayerMask LayerMask { get; set; } = 255;

        public float MaximumDistance { get; set; } = 1f;
        public float Radius { get; set; } = 0.2f;

        /// <summary>
        /// Cast an additional ray to get the true surface normal.
        /// </summary>
        public bool UseRealisticSurfaceNormal { get; set; }

        /// <summary>
        /// Cast an additional ray to get the true distance to the ground.
        /// </summary>
        public bool UseRealisticDistance { get; set; }

#if UNITY_EDITOR

        // Whether to draw debug information in the editor.
        public bool IsDebugMode { get; set; }

#endif

        public HitInformation Hit { get; } = new();

        public MultipleRayOption Option { get; } = new();

        #region NESTED STRUCTURE API

        public class HitInformation
        {
            public Vector3 Position { get; set; }
            public Vector3 Normal { get; set; }
            public float Distance { get; set; }

            public bool IsDetected { get; set; }

            public List<Transform> Transforms { get; } = new ();

            public List<Collider> Colliders { get; } = new ();

            public Transform Transform => Transforms[0];

            public Collider Collider => Colliders[0];
        }

        public class MultipleRayOption
        {
            // Number of rays in every row.
            public int Rows { get; set; } = 3;

            // Number of rows around the central ray;
            public int Count { get; set; } = 9;

            // Whether offset every other row.
            public bool IsOffset { get; set; }
        }

        #endregion
    }
}
