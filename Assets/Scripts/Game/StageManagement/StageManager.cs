using System;
using System.Collections.Generic;
using System.Linq;
using EditorTools;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Stages
{
    public sealed class StageManager : MonoBehaviour
    {
        public static StageManager Singleton;

        public const string MAIN_STAGE_PREFABS_PATH = "Prefabs/Stages/Main/";
        public const string EXTRA_STAGE_PREFABS_PATH = "Prefabs/Stages/Extra/";
        public const string STAGE_DEFAULT_NAME = "Stage";

#if UNITY_EDITOR
#pragma warning disable 0414
        [ShowOnly]
        [SerializeField]
        private string mainStagesPrefabsPath = MAIN_STAGE_PREFABS_PATH, extraStagesPrefabsPath = EXTRA_STAGE_PREFABS_PATH;
#pragma warning restore 0414
#endif

        public static int totalMainStages;
        public static List<GameObject> mainStagesPrefabs;

        public static int totalExtraStages;
        public static List<GameObject> extraStagesPrefabs;

        public static byte stageID = 1;

        [Space(10f)]
        [Header("Stage")]
        public StageBehaviour currentStage;

        public static Difficulty currentDifficulty { get; set; } = Difficulty.Normal;
        public static GameMode currentGameMode = GameMode.MainGame;

        public static void SetupStages()
        {
            mainStagesPrefabs = Resources.LoadAll<GameObject>(MAIN_STAGE_PREFABS_PATH).ToList();
            totalMainStages = mainStagesPrefabs.Count;

            extraStagesPrefabs = Resources.LoadAll<GameObject>(EXTRA_STAGE_PREFABS_PATH).ToList();
            totalExtraStages = extraStagesPrefabs.Count;
        }

        /// <summary>
        /// Loads a Stage at the game scene, first change stageID before using this function.
        /// </summary>
        private static void LoadStage()
        {
            Singleton.currentStage = Instantiate(mainStagesPrefabs[stageID - 1]).GetComponent<StageBehaviour>();
        }

        public static void LoadStageScene(Difficulty difficulty)
        {
            currentDifficulty = difficulty;
            SceneManager.LoadScene(1);
        }

        public static void LoadStageScene(byte id, Difficulty difficulty)
        {
            stageID = id;
            LoadStageScene(difficulty);
        }

        private void Awake()
        {
            Singleton = ObjectUtils.MonoBehaviourGeneral.DeclareSingleton(this, Singleton);
            LoadStage();
        }
    }

    [Serializable]
    public enum Difficulty : byte
    {
        Easy,
        Normal,
        Hard,
        Lunatic,
        Extra,
    }

    [Serializable]
    public enum GameMode : byte
    {
        MainGame,
        StagePractice,
        BossPractice,
    }
}
