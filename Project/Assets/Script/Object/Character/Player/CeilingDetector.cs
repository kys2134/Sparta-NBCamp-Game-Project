using System;
using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class CeilingDetector : MonoBehaviour
    {
        #region SERIALIZABLE FIELD API

        [Header("Detection Settings")]
        [Tooltip("Angle limit for ceiling hits\n\n" +
                 "천장 충돌 각도 제한 값")]
        [SerializeField]
        public float ceilingAngleLimit = 10f;

        public DetectionMode detectionMode;

#if UNITY_EDITOR

        [Header("Debug Settings")]
        [Tooltip("If enabled, draw debug information to show hit positions and hit normals.\n\n" +
                 "이 기능을 활성화하면, 충돌 위치와 충돌 법선을 표시하기 위한 디버그 정보를 그린다.")]
        [SerializeField]
        public bool isDebugMode;

        // How long debug information is drawn on the screen.
        private const float Duration = 2.0f;

#endif

        #endregion

        private void OnCollisionEnter(Collision collision)
        {
            CheckCollisionAngles(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            CheckCollisionAngles(collision);
        }

        /// <summary>
        /// Check if a given collision qualifies as a ceiling hit.
        /// </summary>
        private void CheckCollisionAngles(Collision collision)
        {
            var angle = 0f;

            switch (detectionMode)
            {
                case DetectionMode.OnlyCheckFirstContact:
                {
                    // Calculate angle between hit normal and character.
                    angle = Vector3.Angle(-transform.up, collision.contacts[0].normal);

                    // If angle is smaller than ceiling angle limit, register ceiling hit.
                    if (angle < ceilingAngleLimit)
                    {
                        WasDetected = true;
                    }

#if UNITY_EDITOR

                    if (isDebugMode)
                    {
                        var position = collision.contacts[0].point;
                        var direction = collision.contacts[0].normal;

                        Debug.DrawRay(position, direction, Color.red, Duration);
                    }

#endif

                    break;
                }
                case DetectionMode.CheckAllContacts:
                {
                    for (var i = 0; i < collision.contacts.Length; i++)
                    {
                        // Calculate angle between hit normal and character.
                        angle = Vector3.Angle(-transform.up, collision.contacts[i].normal);

                        // If angle is smaller than ceiling angle limit, register ceiling hit.
                        if (angle < ceilingAngleLimit)
                        {
                            WasDetected = true;
                        }

#if UNITY_EDITOR

                        if (isDebugMode == false)
                        {
                            continue;
                        }

                        var position = collision.contacts[i].point;
                        var direction = collision.contacts[i].normal;

                        Debug.DrawRay(position, direction, Color.red, Duration);

#endif
                    }

                    break;
                }
                case DetectionMode.CheckAverageOfAllContacts:
                {
                    for (var i = 0; i < collision.contacts.Length; i++)
                    {
                        // Calculate angle between hit normal and character and add it to total angle count.
                        angle += Vector3.Angle(-transform.up, collision.contacts[i].normal);

#if UNITY_EDITOR

                        if (isDebugMode == false)
                        {
                            continue;
                        }

                        var position = collision.contacts[i].point;
                        var direction = collision.contacts[i].normal;

                        Debug.DrawRay(position, direction, Color.red, Duration);

#endif
                    }

                    // If average angle is smaller than the ceiling angle limit, register ceiling hit.
                    if (angle / collision.contacts.Length < ceilingAngleLimit)
                    {
                        WasDetected = true;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Refresh()
        {
            WasDetected = false;
        }

        /// <returns>
        /// Whether ceiling was hit during the last frame.
        /// </returns>
        public bool WasDetected { get; private set; }

        #region NESTED ENUMERATION API

        public enum DetectionMode
        {
            OnlyCheckFirstContact, // Only check the very first collision contact. This option is slightly faster but less accurate than the other two options.
            CheckAllContacts, // Check all contact points and register a ceiling hit as long as just one contact qualifies.
            CheckAverageOfAllContacts // Calculate an average surface normal to check against.
        }

        #endregion
    }
}
