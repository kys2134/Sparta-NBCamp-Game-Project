using UnityEngine;

namespace Backend.Object.Character.Player
{
    //This camera input class is an example of how to get input from a connected mouse using Unity's default input system;
    //It also includes an optional mouse sensitivity setting;
    public class CameraMouseInput : MonoBehaviour
    {
        #region SERIALIZABLE FIELD API

        [Header("IO System Settings")]
        [SerializeField] private string mouseHorizontalAxis = "Mouse X";
        [SerializeField] private string mouseVerticalAxis = "Mouse Y";

        [Header("Input Settings")]
        [SerializeField] private bool isHorizontalInputInverted;
        [SerializeField] private bool isVerticalInputInverted;

        [Header("Controller Settings")]
        [Tooltip("Use this value to fine-tune mouse movement. All mouse input will be multiplied by this value.\n\n" +
                 "해당 값을 사용하여 마우스 움직임을 미세 조정할 수 있다. 모든 마우스 입력값은 이 값으로 곱해진다.")]
        [SerializeField] private float multiplier = 0.01f;

        #endregion

        public float GetHorizontalCameraInput()
        {
            // Get raw mouse input.
            var input = Input.GetAxisRaw(mouseHorizontalAxis);

            // Since raw mouse input is already time-based, we need to correct for this before passing the input to the camera controller.
            if (Time.timeScale > 0f && Time.deltaTime > 0f)
            {
                input /= Time.deltaTime;
                input *= Time.timeScale;
            }
            else
            {
                input = 0f;
            }

            // Apply mouse sensitivity.
            input *= multiplier;

            // Invert input.
            if (isHorizontalInputInverted)
            {
                input *= -1f;
            }

            return input;
        }

        public float GetVerticalCameraInput()
        {
            // Get raw mouse input.
            var input = -Input.GetAxisRaw(mouseVerticalAxis);

            // Since raw mouse input is already time-based, we need to correct for this before passing the input to the camera controller.
            if (Time.timeScale > 0f && Time.deltaTime > 0f)
            {
                input /= Time.deltaTime;
                input *= Time.timeScale;
            }
            else
            {
                input = 0f;
            }

            // Apply mouse sensitivity.
            input *= multiplier;

            // Invert input.
            if (isVerticalInputInverted)
            {
                input *= -1f;
            }

            return input;
        }
    }
}
