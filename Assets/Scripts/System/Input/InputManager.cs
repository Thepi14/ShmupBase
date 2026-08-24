using System;
using EditorTools;
using Main.ReplaySystem;
using ObjectUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Main.InputSystem
{
    public sealed class InputManager : MonoBehaviour
    {
        public static InputManager InputManagerInstance { get; private set; }
        public static PlayerInputSystem playerInputSystem;

        [SerializeField]
        private Vector2 moveInput;
        [SerializeField]
        private bool attack;
        [SerializeField]
        private bool bomb;
        [SerializeField]
        private bool slow;

        public static PlayerInput playerInput;

        public bool mouseLocked = false;
        public GameObject lastSelect;

        public static UnityEvent UIEscapeEvent;

        private void Awake()
        {
            InputManagerInstance = MonoBehaviourGeneral.DeclareSingletonDontDestroyOnLoad(this, InputManagerInstance);
            SceneManager.activeSceneChanged += OnSceneWasSwitched;

            playerInputSystem = new PlayerInputSystem();
            playerInputSystem.Enable();

            //UI
            UIEscapeEvent = new();

            playerInputSystem.UI.Escape.started += ctx => { UIEscapeEvent.Invoke(); };

            SetControlsEvents();
        }

        public void SetControlsEvents()
        {
            //GAME

            playerInputSystem.Player.Move.started += ctx => moveInput = ctx.ReadValue<Vector2>();
            playerInputSystem.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            playerInputSystem.Player.Move.canceled += ctx => moveInput = ctx.ReadValue<Vector2>();

            playerInputSystem.Player.Attack.started += ctx => attack = true;
            playerInputSystem.Player.Attack.canceled += ctx => attack = false;

            playerInputSystem.Player.Bomb.started += ctx => bomb = true;
            playerInputSystem.Player.Bomb.canceled += ctx => bomb = false;

            playerInputSystem.Player.Slow.started += ctx => slow = true;
            //playerInput.Player.Slow.performed += ctx => slow = true;
            playerInputSystem.Player.Slow.canceled += ctx => slow = false;

            //playerInputSystem.UI.Escape.started += ctx => LockMouse(!mouseLocked);
        }

        private void OnSceneWasSwitched(Scene arg0, Scene arg1)
        {
            SetControlsEvents();
            playerInputSystem.Enable();
        }

        private void Start()
        {
            //lastSelect = new GameObject();
        }

        private void FixedUpdate()
        {
            playerInput = new PlayerInput()
            {
                moveInput = moveInput,
                attack = attack,
                bomb = bomb,
                slow = slow,
            };

            //Debug.Log("Replay: " + ReplayManagement.replayMode + ", Controls: " + playerInput.ToString());
        }

        private void LateUpdate()
        {
            //Stopping mouse shenaningans
            if (mouseLocked)
            {
                if (EventSystem.current.currentSelectedGameObject == null && lastSelect != null)
                {
                    EventSystem.current.SetSelectedGameObject(lastSelect);
                }
                else
                {
                    lastSelect = EventSystem.current.currentSelectedGameObject;
                }
            }
        }

        public static void LockMouse(bool locked)
        {
            InputManagerInstance.mouseLocked = locked;
            if (locked)
            {
                playerInputSystem.UI.Navigate.Enable();

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                playerInputSystem.UI.Navigate.Reset();
                playerInputSystem.UI.Navigate.Disable();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void OnDisable()
        {
            playerInputSystem.Disable();
        }

        public void OnEnable()
        {
            playerInputSystem.Enable();
        }
    }

    /// <summary>
    /// Struct que descreve o que o player fez e o que o replay deve fazer no replay.
    /// </summary>
    [Serializable]
    public struct PlayerInput
    {
        //actions
        [ShowOnly]
        public bool attack;
        [ShowOnly]
        public bool bomb;
        [ShowOnly]
        public bool slow;

        //move
        public Vector2 moveInput;

        //TODO: criar um stringBuilder pra isso pq isso aqui é horrível
        public override string ToString()
        {
            string str = "";

            str += "attack: " + attack + ", bomb: " + bomb + ", slow: " + slow + ", move input: " + moveInput.ToString();

            return str;
        }
    }
}
