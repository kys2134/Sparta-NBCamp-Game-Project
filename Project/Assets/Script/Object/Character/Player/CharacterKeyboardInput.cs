using UnityEngine;

namespace Backend.Object.Character.Player
{
	//This character movement input class is an example of how to get input from a keyboard to control the character;
    public class CharacterKeyboardInput : MonoBehaviour
    {
        #region SERIALIZABLE FIELD API

        [Header("IO System Settings")]
        [SerializeField] private string horizontalInputAxis = "Horizontal";
        [SerializeField] private string verticalInputAxis = "Vertical";
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode rollKey = KeyCode.LeftControl;

        [Tooltip("If this is enabled, Unity's internal input smoothing is bypassed.\n\n" +
                 "이 옵션이 활성화되면 Unity의 내부 입력 스무딩이 우회된다.")]
        [SerializeField] private bool useRawInput = true;

        #endregion

        private void Awake()
        {
            JumpButton = new Button(jumpKey);
            RollButton = new Button(rollKey);
        }

        private void Update()
        {
            JumpButton.Check();
            RollButton.Check();
        }

        public float GetHorizontalMovementInput()
        {
            return useRawInput ? Input.GetAxisRaw(horizontalInputAxis) : Input.GetAxis(horizontalInputAxis);
        }

		public float GetVerticalMovementInput()
        {
            return useRawInput ? Input.GetAxisRaw(verticalInputAxis) : Input.GetAxis(verticalInputAxis);
        }

        public Button JumpButton { get; private set; }

        public Button RollButton { get; private set; }

        public class Button
        {
            private readonly KeyCode _key;

            public Button(KeyCode key)
            {
                _key = key;
            }

            public void Check()
            {
                var isPressed = Input.GetKey(_key);

                switch (IsPressed)
                {
                    case false when isPressed == true:
                        WasPressed = true;
                        break;
                    case true when isPressed == false:
                        WasReleased = true;
                        IsLocked = false;
                        break;
                }

                IsPressed = isPressed;
            }

            public float LastPressedTime { get; set; }

            public bool IsLocked { get; set; }

            public bool WasPressed { get; set; }

            public bool WasReleased { get; set; }

            public bool IsPressed { get; set; }
        }
    }
}
