using Main.EntitySystem;
using Main.InputSystem;
using Main.ReplaySystem;
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
        public static GameUI Instance;

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
        private Button unpauseButton, continueButton, saveReplayButton, restartButton, goBackToMainMenuButton;

        private void OnValidate()
        {
            //FormatScoreText();
            //FormatHighScoreText();
            main = true;
        }

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
            main = true;

            //main panel
            scoreString.Arguments = new object[] { GetScoreString() };
            highscoreString.Arguments = new object[] { GetHighscoreString() };

            scoreString.StringChanged += FormatScoreText;
            highscoreString.StringChanged += FormatHighScoreText;

            //pause panel
            unpauseButton.onClick.AddListener(() => SetOpenPanel(false));
            continueButton.onClick.AddListener(() => { GameManager.Continue(); SetOpenPanel(false); });
            saveReplayButton.onClick.AddListener(() => SaveReplayUI.Instance.SetOpenPanel(true));
            restartButton.onClick.AddListener(() => SceneManager.LoadScene(1));
            goBackToMainMenuButton.onClick.AddListener(() => SceneManager.LoadScene(0));

            //GameManager.gameEndedEvent.AddListener(() => SetOpenPanel(true));
            GameManager.unpauseEvent.AddListener(() => SetOpenPanel(false));
            PlayerEntity.PlayerLostAllLifesEvent.AddListener(() => SetOpenPanel(true));
            InputManager.UIEscapeEvent.AddListener(() => SetOpenPanel(!TimeManager.GameIsPaused));
        }

        protected override void Start()
        {
            base.Start();
        }

        private void LateUpdate()
        {
            UpdateScore();
            UpdateHighscore();

            /*if (InputManager.UIEscape)
            {
                Vars.PauseGame(!Vars.GameIsPaused);
            }*/
        }

        public string GetScoreString() => Vars.FormatAsScoreString(GameManager.Singleton.score);
        public string GetHighscoreString() => Vars.FormatAsScoreString(GameManager.Singleton.highScore);

        public void UpdateScore()
        {
            scoreString.Arguments[0] = GetScoreString();
            scoreString.RefreshString();
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
            bool
                gameCompleted = GameManager.Singleton.gameCompleted,
                replayMode = ReplayManagement.replayMode,
                continueEnabled = GameManager.CanContinue(),
                continued = GameManager.Continued(),
                playerLostLastLife = PlayerEntity.PlayerLostLastLife();

            bool
                canUnpause = continueEnabled && !playerLostLastLife && !gameCompleted,
                canContinue = !replayMode && playerLostLastLife && continueEnabled && !gameCompleted,
                canSaveReplay = !replayMode && playerLostLastLife && !continued;

            if (!canUnpause && opened && !open)
                open = true;

            base.SetOpenPanel(open);
            TimeManager.Pause(open);
            pauseSubPanel.gameObject.SetActive(open);
            InputManager.LockMouse(!(open && Vars.UseMouse));

            if (open)
            {
                unpauseButton.gameObject.SetActive(canUnpause);
                continueButton.gameObject.SetActive(canContinue);
                saveReplayButton.gameObject.SetActive(canSaveReplay);

                if (canUnpause)
                    unpauseButton.SelectIfMouseInactive();
                if (canContinue)
                    continueButton.SelectIfMouseInactive();
                if (canSaveReplay)
                    saveReplayButton.SelectIfMouseInactive();
            }
        }
    }
}
