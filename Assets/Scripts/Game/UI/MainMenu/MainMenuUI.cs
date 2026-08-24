using Main.InputSystem;
using Main.ReplaySystem;
using Main.Sound;
using Main.Stages;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.UI
{
    public class MainMenuUI : PanelBehaviour
    {
        public Button startButton, replaysButton, settingsButton, exitButton;

        public RectTransform subPanel;
        public PanelBehaviour settingsPanel, replaysPanel;

        protected override void Awake()
        {
            Vars.StartVars();
            base.Awake();
            main = true;

            ReplayManagement.LoadAllReplayFiles();

            InputManager.LockMouse(!Vars.UseMouse);

            SoundManager.PlayMusic("Flower");

            startButton.onClick.AddListener(() => { ReplayManagement.replayMode = false; StageManager.LoadStageScene(1, Difficulty.Normal); });
            replaysButton.onClick.AddListener(() => { replaysPanel.SetOpenPanel(true); });
            settingsButton.onClick.AddListener(() => { settingsPanel.SetOpenPanel(true); });

            exitButton.onClick.AddListener(() => { Application.Quit(); });

            TimeManager.Pause(false);
        }

        protected void LateUpdate()
        {

        }

        public override void SetOpenPanel(bool open)
        {
            subPanel.gameObject.SetActive(open);

            if (open)
            {
                startButton.SelectIfMouseInactive();
                currentPanel = this;
            }
        }
    }
}
