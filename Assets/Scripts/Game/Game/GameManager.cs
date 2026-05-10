using UnityEngine;

namespace Main
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Singleton { get; private set; }
        public static Bounds bounds = new(new(), new(8, 10, float.PositiveInfinity));
        public static readonly Vector2 startPlayerPosition = new(0, -3);

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
