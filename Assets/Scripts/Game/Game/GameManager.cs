using UnityEngine;

namespace Main
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Singleton { get; private set; }
        public static readonly Bounds bounds = new(new Vector2(), new Vector3(8f, 10f, float.PositiveInfinity));

        public Camera mainCamera;

        public int highScore;
        public int score;
        public int lifes;
        /// <summary>
        /// (Guts)
        /// </summary>
        public int graze;

        public bool replay = false;

        private void Awake()
        {
            Singleton = ObjectUtils.MonoBehaviourGeneral.DeclareSingletonDontDestroyOnLoad<GameManager>(this, Singleton);
            Vars.StartVars();

        }

        private void Update()
        {

        }

        public static void LoseLife()
        {
            Singleton.lifes--;
            if (Singleton.lifes == 0)
            {

            }
        }
    }
}
