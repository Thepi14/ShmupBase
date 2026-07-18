using Main.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.UI
{
    public class GameUI : PanelBehaviour
    {
        [Header("Main Panel")]
        [SerializeField]
        private RectTransform mainSubPanel;

        [SerializeField]
        private TMP_Text highScoreText;
        [SerializeField]
        private TMP_Text scoreText;

        [SerializeField]
        private GameObject lifeIconPrefab;
        [SerializeField]
        private GameObject lifesHorizontalLayout;

        [SerializeField]
        private LocalizedString scoreString, highscoreString;

        [Header("Pause Panel")]
        [SerializeField]
        private RectTransform pauseSubPanel;
        [SerializeField]
        private Button unpauseButton, restartButton, goBackToMainMenuButton;

        [SerializeField]
        private bool commas = true;

        private void OnValidate()
        {
            //FormatScoreText();
            //FormatHighScoreText();
            main = true;
        }

        protected override void Awake()
        {
            base.Awake();
            main = true;

            //main panel
            scoreString.Arguments = new object[] { GetScoreString() };
            highscoreString.Arguments = new object[] { GetHighscoreString() };

            scoreString.StringChanged += FormatScoreText;
            highscoreString.StringChanged += FormatHighScoreText;

            //pause panel
            unpauseButton.onClick.AddListener(() => { SetOpenPanel(false); Debug.Log("unpaused UI"); });
            restartButton.onClick.AddListener(() => { SceneManager.LoadScene(1); });
            goBackToMainMenuButton.onClick.AddListener(() => { SceneManager.LoadScene(0); });

            GameManager.gameEndedEvent.AddListener(() => SetOpenPanel(false));
            InputManager.UIEscapeEvent.AddListener(() => { SetOpenPanel(!Vars.GameIsPaused); Debug.Log("paused UI"); });
        }

        protected override void Start()
        {
            base.Start();

            InputManager.LockMouse(true);
            SetOpenPanel(false);
        }

        private void LateUpdate()
        {
            if (!GameManager.Singleton.gameEnded)
            {
                UpdateScore();
                UpdateHighscore();

                /*if (InputManager.UIEscape)
                {
                    Vars.PauseGame(!Vars.GameIsPaused);
                }*/
            }
        }

        public string GetScoreString() => commas ? string.Format("{0:n}", GameManager.Singleton.score) : GameManager.Singleton.score.ToString();
        public string GetHighscoreString() => commas ? string.Format("{0:n}", GameManager.Singleton.highScore) : GameManager.Singleton.highScore.ToString();

        public void UpdateScore()
        {
            scoreString.Arguments[0] = GetScoreString();
            highscoreString.RefreshString();
        }

        public void UpdateHighscore()
        {
            highscoreString.Arguments[0] = GetHighscoreString();
            highscoreString.RefreshString();
        }

        public void FormatScoreText(string text)
        {
            scoreText.text = text;
        }

        public void FormatHighScoreText(string text)
        {
            highScoreText.text = text;
        }

        public override void SetOpenPanel(bool open)
        {
            base.SetOpenPanel(open);
            Vars.PauseGame(open);
            pauseSubPanel.gameObject.SetActive(open);

            if (open)
            {
                unpauseButton.Select();
            }
        }
    }
}
