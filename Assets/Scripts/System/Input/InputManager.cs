using System;
using ObjectUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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

            playerInputSystem = new PlayerInputSystem();
            playerInputSystem.Enable();

            UIEscapeEvent = new();

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

            //UI
            playerInputSystem.UI.Escape.started += ctx => { UIEscapeEvent.Invoke(); };

            //playerInputSystem.UI.Escape.started += ctx => LockMouse(!mouseLocked);
        }

        private void Start()
        {
            lastSelect = new GameObject();
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
        }

        private void LateUpdate()
        {
            //Stopping mouse shenaningans
            if (mouseLocked)
            {
                if (EventSystem.current.currentSelectedGameObject == null)
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
            if (InputManagerInstance.mouseLocked)
            {
                playerInputSystem.UI.Navigate.Reset();
                playerInputSystem.UI.Navigate.Disable();

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                playerInputSystem.UI.Navigate.Enable();

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
        //NON SERIALIZED

        //actions
        [NonSerialized]
        public bool attack;
        [NonSerialized]
        public bool bomb;
        [NonSerialized]
        public bool slow;

        //move
        [NonSerialized]
        public Vector2 moveInput;

        //SERIALIZED

        //actions
        [HideInInspector]
        public byte c;

        //move
        [HideInInspector]
        public sbyte x;
        [HideInInspector]
        public sbyte y;

        public PlayerInput ConvertSerializable()
        {
            c = 0;

            c = c.SetBit(0, attack);
            c = c.SetBit(1, bomb);
            c = c.SetBit(2, slow);

            x = (sbyte)Mathf.RoundToInt(moveInput.x);
            y = (sbyte)Mathf.RoundToInt(moveInput.y);

            /*Debug.Log(Convert.ToString(c, 2).PadLeft(8, '0') + ", x: " + x + ", y: " + y);
            Debug.Log(new Vector2(x, y).normalized.ToString());

            Debug.Log(c.GetBit(0) + ", " + c.GetBit(1) + ", " + c.GetBit(2));*/

            return this;
        }

        public PlayerInput ConvertToReadableObject()
        {
            attack = c.GetBit(0);
            bomb = c.GetBit(1);
            slow = c.GetBit(2);

            moveInput = new Vector2Int { x = x, y = y };

            return this;
        }

        public override string ToString()
        {
            string str = "";

            str += "attack: " + attack + ", bomb: " + bomb + ", slow: " + slow + ", move input: " + moveInput.ToString();

            return str;
        }
    }
}
