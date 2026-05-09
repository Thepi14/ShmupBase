using ObjectUtils;
using UnityEngine;

namespace Main
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager InputManagerInstance;
        public PlayerInputSystem playerInput;

        public Vector2 moveInput;
        public bool attack;
        public bool slow;

        public bool mouseLocked = false;

        private void Start()
        {
            InputManagerInstance = MonoBehaviourGeneral.DeclareSingletonDontDestroyOnLoad<InputManager>(this, InputManagerInstance);

            playerInput = new PlayerInputSystem();
            playerInput.Enable();

            //GAME

            playerInput.Player.Move.started += ctx => moveInput = ctx.ReadValue<Vector2>();
            playerInput.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            playerInput.Player.Move.canceled += ctx => moveInput = ctx.ReadValue<Vector2>();

            playerInput.Player.Slow.started += ctx => slow = true;
            //playerInput.Player.Slow.performed += ctx => slow = true;
            playerInput.Player.Slow.canceled += ctx => slow = false;

            playerInput.Player.Attack.started += ctx => attack = true;
            playerInput.Player.Attack.canceled += ctx => attack = false;

            //UI

            playerInput.UI.Escape.started += ctx => LockMouse(!mouseLocked);
        }

        private void Update()
        {

        }

        public void LockMouse(bool locked)
        {
            mouseLocked = locked;
            if (mouseLocked)
            {
                playerInput.UI.Navigate.Reset();
                playerInput.UI.Navigate.Disable();

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                playerInput.UI.Navigate.Enable();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
