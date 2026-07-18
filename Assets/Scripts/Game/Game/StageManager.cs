using System.Collections;
using EditorTools;
using Main.Stages;
using UnityEngine;

namespace Main
{
    public sealed class StageManager : MonoBehaviour
    {
        public const string STAGE_BACKGROUNDS_PREFABS_PATH = "Prefabs/Stages/";
        public const string STAGE_DEFAULT_NAME = "Stage";

#if UNITY_EDITOR
        [ShowOnly]
        [SerializeField]
        private string stageBackgroundPrefabsPath = STAGE_BACKGROUNDS_PREFABS_PATH;
#endif

        public Camera backgroundCamera;

        public static StageManager Singleton { get; private set; }

        public static byte stageID = 0;
        public static StageBehaviour stage;

        public static bool stopScenario = false;

        private void Awake()
        {
            Singleton = ObjectUtils.MonoBehaviourGeneral.DeclareSingleton(this, Singleton);
        }

        public static void RunStage(byte id = 0)
        {
            GameObject stagePrefab = null;
            stageID = id;

            switch (stageID)
            {
                case 0:
                    Debug.Log("No Stage selected");
                    break;
                case <= 6:
                    stagePrefab = Resources.Load<GameObject>(STAGE_BACKGROUNDS_PREFABS_PATH + STAGE_DEFAULT_NAME + stageID);

                    if (stagePrefab == null)
                    {
                        Debug.LogError(STAGE_DEFAULT_NAME + stageID + " is null.");
                        return;
                    }
                    else if (stagePrefab.GetComponent<StageBehaviour>() == null)
                    {
                        Debug.LogError(STAGE_DEFAULT_NAME + stageID + "s StageBehaviour is null.");
                        return;
                    }

                    stage = Instantiate(stagePrefab).GetComponent<StageBehaviour>();

                    break;
                default:
                    Debug.Log("There is no stage " + stageID);
                    break;
            }
        }
    }
}
