using System;
using System.Collections.Generic;
using EditorTools;
using Main.InputSystem;
using Main.ReplaySystem;
using Main.Stages;
using UnityEngine;
using UnityEngine.Events;
using static Main.EntitySystem.PlayerEntity;

namespace Main
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Singleton { get; private set; }

        public static readonly Bounds bounds = new(new(), new(8, 10, float.PositiveInfinity));

        [Header("General")]
        [Space(10f)]
        [Header("Score")]
        public long highScore;
        public long score;
        /// <summary>
        /// (Guts)
        /// </summary>
        [Space(10f)]
        public int graze;
        [Space(10f)]
        public int maxContinues = 3;
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

        public static UnityEvent 
            onResume = new(), 
            onStageEnd = new(), 
            onGameEnd = new(), 
            onEnding = new();

        private void Awake()
        {
#if UNITY_EDITOR
            Vars.StartVars();
#endif
            Singleton = ObjectUtils.MonoBehaviourGeneral.DeclareSingleton(this, Singleton);
            playerInput.Clear();

            if (ReplayManagement.replayMode)
            {
                replay = ReplayManagement.LoadReplayFile(ReplayManagement.replayFilePath);

                seed = replay.seed;
                highScore = replay.highScore;

                StageManager.stageID = replay.stageID;
                selectedCharacterID = replay.characterID;

                StageManager.currentDifficulty = replay.difficulty;
            }
            else
            {
                replay = new Replay(seed);
                replay.dateTime = DateTime.Now.Ticks;

                seed = Time.frameCount;
                highScore = Vars.Highscore;

                replay.stageID = StageManager.stageID;
                replay.characterID = selectedCharacterID;

                replay.difficulty = StageManager.currentDifficulty;
            }

            /*var inp = new PlayerInput() { attack = true, slow = true, moveInput = new Vector2(0.71f, 0.71f) };
            inp.ConvertSerializable();
            Debug.Log(inp.c);*/

            random = new System.Random(seed);
            //useless indeed
            UnityEngine.Random.InitState(seed);

            Debug.Log("GameManager: " + (byte)StageManager.currentDifficulty + ", " + (byte)Vars.LastDifficulty);
        }

        private void Start()
        {
            GeneratePlayer();
        }

        public static void CompleteGame()
        {
            Singleton.gameCompleted = true;
            onGameEnd.Invoke();
        }

        public static void StartEnding()
        {
            onEnding.Invoke();
        }

        public static bool CanContinue() => Singleton.continues < Singleton.maxContinues && !ReplayManagement.replayMode;

        public static void Continue()
        {
            Singleton.continues++;
            if (!CanContinue())
                return;

            SetLifes(PlayerInstance.startingLifes);
            onResume.Invoke();
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
            Singleton.replay.rawEndTime = DateTime.Now.Ticks;

            if (Vars.SaveReplaysAsJson)
                ReplayManagement.SaveReplayFileAsJson(Singleton.replay);
            else 
                ReplayManagement.SaveReplayFile(Singleton.replay);
        }

        public static PlayerInput GetCurrentPlayerInput() => Singleton.currentPlayerInput;
    }
}
