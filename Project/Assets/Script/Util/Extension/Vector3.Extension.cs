using UnityEngine;

namespace Backend.Object.Character.Player
{
    public static class Vector3Extension
    {

        /// <summary>
        /// Calculate signed angle (ranging from -180 to +180) between 'a' and 'b'.
        /// </summary>
        public static float SignedAngle(this Vector3 a, Vector3 b, Vector3 normal)
        {
            // Calculate angle and sign.
            var angle = Vector3.Angle(a, b);
            var sign = Mathf.Sign(Vector3.Dot(normal, Vector3.Cross(a, b)));

            //Combine angle and sign.
            return angle * sign;
        }

        /// <summary>
        /// Remove all parts from a vector that are pointing in the same direction as 'b'.
        /// </summary>
        public static Vector3 Reject(this Vector3 a, Vector3 b)
        {
            b = b.normalized;
            a -= b * Vector3.Dot(a, b);

            return a;
        }

        /// <summary>
        /// Extract and return parts from a vector that are pointing in the same direction as 'b'.
        /// </summary>
        public static Vector3 Project(this Vector3 a, Vector3 b)
        {
            return b * Vector3.Dot(a, b.normalized);
        }
    }
}
