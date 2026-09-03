using Main.InputSystem;
using Main.ReplaySystem;
using Main.Sound;
using Main.Stages;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.UI
{
    public class MainMenuPanel : GenericPanelBehaviour
    {
        [Header("Buttons")]
        public Button playButton;
        public Button practiceButton;
        public Button replaysButton;
        public Button musicsButton;
        public Button settingsButton;

        public Button exitButton;

        [Space(10f)]
        [Header("Panels")]
        public PanelBehaviour difficultyPanel;
        public PanelBehaviour practicePanel;
        public PanelBehaviour musicsPanel;
        public PanelBehaviour replaysPanel;
        public PanelBehaviour settingsPanel;

        protected override void Awake()
        {
            Vars.StartVars();
            main = true;
            base.Awake();

            ReplayManagement.LoadAllReplayFiles();

            InputManager.LockMouse(!Vars.UseMouse);

            playButton.onClick.AddListener(() => { difficultyPanel.SetOpenPanel(true); StageManager.currentGameMode = GameMode.MainGame; StageManager.stageID = 1; });
            practiceButton.onClick.AddListener(() => { practicePanel.SetOpenPanel(true); });
            replaysButton.onClick.AddListener(() => { replaysPanel.SetOpenPanel(true); });
            musicsButton.onClick.AddListener(() => { musicsPanel.SetOpenPanel(true); });
            settingsButton.onClick.AddListener(() => { settingsPanel.SetOpenPanel(true); });

            exitButton.onClick.AddListener(() => { Application.Quit(); });

            TimeManager.Pause(false);
        }

        public override void SetOpenPanel(bool open)
        {
            base.SetOpenPanel(open);

            if (open)
            {
                playButton.SelectIfMouseInactive();
                SoundManager.PlayMusic("Flower");
            }
        }
    }
}
