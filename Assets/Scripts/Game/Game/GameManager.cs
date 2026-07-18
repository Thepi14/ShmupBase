using System.Collections.Generic;
using EditorTools;
using Main.InputSystem;
using Main.ReplaySystem;
using Main.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Main
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Singleton { get; private set; }
        public static Bounds bounds = new(new(), new(8, 10, float.PositiveInfinity));
        public static readonly Vector2 startPlayerPosition = new(0, -3);

        [Header("Cameras")]
        public Camera UICamera;
        public Camera gameCamera;
        public Camera BackgroundCamera;

        [Header("General")]
        public int highScore;
        public int score;

        public int lifes;
        public int bombs;
        /// <summary>
        /// (Guts)
        /// </summary>
        public int graze;

        [ShowOnly]
        public int seed;
        public static System.Random random;

        public bool gameEnded = false;

        [Header("Replay system")]
        public Replay replay;
        [ShowOnly]
        public int currentFrameIndex = 0;
        [SerializeField]
        private PlayerInput currentPlayerInput;
        public List<PlayerInput> playerInput = new();

        public static UnityEvent gameEndedEvent = new();

        private void Awake()
        {
#if UNITY_EDITOR
            Vars.StartVars();
#endif

            Singleton = ObjectUtils.MonoBehaviourGeneral.DeclareSingleton(this, Singleton);
            gameEndedEvent = new UnityEvent();
            InputManager.LockMouse(true);

            if (ReplayManagement.replayMode)
            {
                replay = ReplayManagement.LoadReplayFile(ReplayManagement.replayFileName);

                seed = replay.seed;
                highScore = replay.highScore;
                lifes = replay.startLifes;
                bombs = replay.startBombs;

                Vars.currentDifficulty = replay.difficulty;
            }
            else
            {
                seed = Time.frameCount;
                replay = new Replay(seed);

                highScore = Vars.Highscore;
                lifes = Vars.STARTING_LIFES;
                bombs = Vars.STARTING_BOMBS;

                replay.difficulty = Vars.currentDifficulty;
            }

            /*var inp = new PlayerInput() { attack = true, slow = true, moveInput = new Vector2(0.71f, 0.71f) };
            inp.ConvertSerializable();
            Debug.Log(inp.c);*/

            random = new System.Random(seed);
            //useless indeed
            Random.InitState(seed);
        }

        private void Start()
        {
            Vars.GetManagers();

            Vars.PauseGame(false);
        }

        public static void AddLifes(byte amount = 1) => Singleton.lifes = Mathf.Clamp(Singleton.lifes + amount, 0, Vars.MAX_BOMBS);

        public static void LoseLife()
        {
            Singleton.lifes = Mathf.Clamp(Singleton.lifes - 1, 0, Vars.MAX_LIFES);
            if (Singleton.lifes == 0)
            {
                EndGame();
            }
        }

        public static void EndGame()
        {
            Singleton.gameEnded = true;
            gameEndedEvent.Invoke();
            Vars.PauseGame(true);
        }

        public static void AddBombs(byte amount = 1) => Singleton.bombs = Mathf.Clamp(Singleton.bombs + amount, 0, Vars.MAX_BOMBS);

        public static void LoseBomb() => Singleton.bombs = Mathf.Clamp(Singleton.bombs - 1, 0, Vars.MAX_BOMBS);

        private void FixedUpdate()
        {
            if (ReplayManagement.replayMode)
            {
                if (currentFrameIndex < replay.playerInput.Length)
                    currentPlayerInput = replay.playerInput[currentFrameIndex];
            }
            else
            {
                if (!gameEnded)
                {
                    currentPlayerInput = InputManager.playerInput;
                    playerInput.Add(InputManager.playerInput);
                }
            }

            //Debug.Log(currentPlayerInput.ToString());

            currentFrameIndex++;
        }

        public static void SaveReplay(string text)
        {
            Singleton.replay.name = text;
            Singleton.replay.playerInput = Singleton.playerInput.ToArray();
            Singleton.replay.framesDuration = Singleton.currentFrameIndex + 1;

            Singleton.replay.name = text;
            ReplayManagement.SaveReplayFileAsJson(Singleton.replay);
        }

        public static PlayerInput GetCurrentPlayerInput() => Singleton.currentPlayerInput;
    }
}
