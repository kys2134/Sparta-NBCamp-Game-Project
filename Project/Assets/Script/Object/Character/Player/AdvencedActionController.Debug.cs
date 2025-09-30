#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Backend.Object.Character.Player
{
    public partial class AdvancedActionController
    {
        private Vector3 _velocity = Vector3.zero;

        private void OnDrawGizmos()
        {
            if (_movementController == null)
            {
                return;
            }

            var child = transform.GetChild(0);

            var a = _facing.normalized;
            var b = _velocity.normalized;
            var normal = child.right;

            a = Vector3.ProjectOnPlane(a, normal).normalized;
            b = Vector3.ProjectOnPlane(b, normal).normalized;

            var angle = Vector3.SignedAngle(a, b, normal);
            const float radius = 0.4f;

            Handles.color = new Color(0f, 0f, 1f, 0.2f);
            Handles.DrawSolidArc(transform.position, normal, a, angle, radius);

            Debug.DrawLine(transform.position, transform.position + (a * radius), Color.blue);
            Debug.DrawLine(transform.position, transform.position + (b * radius), Color.blue);

            // Draw facing direction line for debugging.
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position + (a * radius), 0.04f);
            Gizmos.DrawSphere(transform.position + (b * radius), 0.04f);
        }
    }
}

#endif
