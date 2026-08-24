using System;
using System.Collections.Generic;
using EditorTools;
using Main.EntitySystem;
using Main.InputSystem;
using Main.ReplaySystem;
using Main.Stages;
using Main.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Main
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Singleton { get; private set; }

        public static readonly Bounds bounds = new(new(), new(8, 10, float.PositiveInfinity));
        public static readonly Vector2 startPlayerPosition = new(0, -3);

        [Header("General")]
        [Space(10f)]
        [Header("Score")]
        public long highScore;
        public long score;
        [Space(10f)]
        [Header("Player")]
        public int lifes;
        [ShowOnly]
        public int lostLifes = 0;
        [Space(10f)]
        public int bombs;
        [ShowOnly]
        public int usedBombs = 0;
        [Space(10f)]
        /// <summary>
        /// (Guts)
        /// </summary>
        public int graze;
        [Space(10f)]
        public int maxContinutes = 3;
        public int continues = 0;
        public static bool Continued() => Singleton.continues > 0;
        [Space(10f)]
        [ShowOnly]
        public int seed;
        public static System.Random random;
        [Space(10f)]
        [ShowOnly]
        public bool gameCompleted = false;
        [Space(10f)]
        [Header("Replay system")]
        public Replay replay;
        [ShowOnly]
        public int currentFrameIndex = 0;
        [SerializeField]
        private PlayerInput currentPlayerInput;
        public List<PlayerInput> playerInput = new();

        public static UnityEvent unpauseEvent, stageEndedEvent, gameEndedEvent, endingEvent, playerDiedEvent, playerDiedLastLifeEvent;

        private void Awake()
        {
#if UNITY_EDITOR
            Vars.StartVars();
#endif

            Singleton = ObjectUtils.MonoBehaviourGeneral.DeclareSingleton(this, Singleton);
            unpauseEvent = new UnityEvent();
            stageEndedEvent = new UnityEvent();
            gameEndedEvent = new UnityEvent();
            endingEvent = new UnityEvent();
            playerDiedEvent = new UnityEvent();
            playerDiedLastLifeEvent = new UnityEvent();

            if (ReplayManagement.replayMode)
            {
                replay = ReplayManagement.LoadReplayFile(ReplayManagement.replayFilePath);

                seed = replay.seed;
                highScore = replay.highScore;
                lifes = replay.startLifes;
                bombs = replay.startBombs;

                StageManager.currentDifficulty = replay.difficulty;
            }
            else
            {
                seed = Time.frameCount;
                replay = new Replay(seed);
                replay.dateTime = DateTime.Now;

                highScore = Vars.Highscore;
                lifes = Vars.STARTING_LIFES;
                bombs = Vars.STARTING_BOMBS;

                replay.difficulty = StageManager.currentDifficulty;
            }

            /*var inp = new PlayerInput() { attack = true, slow = true, moveInput = new Vector2(0.71f, 0.71f) };
            inp.ConvertSerializable();
            Debug.Log(inp.c);*/

            random = new System.Random(seed);
            //useless indeed
            UnityEngine.Random.InitState(seed);

            EntityManager.GeneratePlayer();
        }

        private void Start()
        {
            GameUI.Instance.SetOpenPanel(false);
        }

        public static void CompleteGame()
        {
            Singleton.gameCompleted = true;
            gameEndedEvent.Invoke();
            TimeManager.Pause(true);
        }

        public static void StartEnding()
        {
            endingEvent.Invoke();
        }

        public static bool CanContinue() => Singleton.continues < Singleton.maxContinutes && !ReplayManagement.replayMode;

        public static void Continue()
        {
            Singleton.continues++;
            if (!CanContinue())
                return;

            AddLifes(Vars.STARTING_LIFES);
            SetBombs(Vars.STARTING_BOMBS);
            unpauseEvent.Invoke();
        }

        public static bool PlayerDied() => Singleton.lifes == 0;

        public static void AddLifes(byte amount = 1) => Singleton.lifes = Mathf.Clamp(Singleton.lifes + amount, 0, Vars.MAX_BOMBS);

        public static void LoseLife(bool invokeEvents = true)
        {
            Singleton.lifes = Mathf.Clamp(Singleton.lifes - 1, 0, Vars.MAX_LIFES);
            Singleton.lostLifes++;

            if (Singleton.lifes == 0)
            {
                if (invokeEvents)
                    playerDiedLastLifeEvent.Invoke();
                //CompleteGame();
            }
            else if (invokeEvents)
                playerDiedEvent.Invoke();
        }

        public static void AddBombs(byte amount = 1) => Singleton.bombs = Mathf.Clamp(Singleton.bombs + amount, 0, Vars.MAX_BOMBS);
        public static void SetBombs(byte amount) => Singleton.bombs = Mathf.Clamp(amount, 0, Vars.MAX_BOMBS);

        public static void LoseBomb()
        {
            Singleton.usedBombs++;
            Singleton.bombs = Mathf.Clamp(Singleton.bombs - 1, 0, Vars.MAX_BOMBS);
        }

        private void FixedUpdate()
        {
            if (ReplayManagement.replayMode)
            {
                if (currentFrameIndex < replay.playerInput.Length)
                    currentPlayerInput = replay.playerInput[currentFrameIndex];
            }
            else
            {
                if (!gameCompleted)
                {
                    currentPlayerInput = InputManager.playerInput;
                    playerInput.Add(InputManager.playerInput);
                }
            }

            //Debug.Log(currentPlayerInput.ToString());

            currentFrameIndex++;
        }

        public static void SaveReplay(string replayName)
        {
            Singleton.replay.name = replayName;
            Singleton.replay.playerInput = Singleton.playerInput.ToArray();
            Singleton.replay.framesDuration = Singleton.currentFrameIndex + 1;
            Singleton.replay.rawEndTime = DateTime.Now;

            if (Vars.SaveReplaysAsJson)
                ReplayManagement.SaveReplayFileAsJson(Singleton.replay);
            else 
                ReplayManagement.SaveReplayFile(Singleton.replay);
        }

        public static PlayerInput GetCurrentPlayerInput() => Singleton.currentPlayerInput;
    }
}
