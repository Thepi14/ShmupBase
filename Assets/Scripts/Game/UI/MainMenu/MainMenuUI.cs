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
    public class MainMenuUI : GenericPanelBehaviour
    {
        public Button startButton, replaysButton, musicsButton, settingsButton, exitButton;
        public PanelBehaviour musicsPanel, replaysPanel, settingsPanel;

        protected override void Awake()
        {
            Vars.StartVars();
            main = true;
            base.Awake();

            ReplayManagement.LoadAllReplayFiles();

            InputManager.LockMouse(!Vars.UseMouse);

            startButton.onClick.AddListener(() => { ReplayManagement.replayMode = false; StageManager.LoadStageScene(1, Difficulty.Normal); });
            replaysButton.onClick.AddListener(() => { replaysPanel.SetOpenPanel(true); });
            musicsButton.onClick.AddListener(() => { musicsPanel.SetOpenPanel(true); });
            settingsButton.onClick.AddListener(() => { settingsPanel.SetOpenPanel(true); });

            exitButton.onClick.AddListener(() => { Application.Quit(); });

            TimeManager.Pause(false);
        }

        protected void LateUpdate()
        {

        }

        public override void SetOpenPanel(bool open)
        {
            base.SetOpenPanel(open);

            if (open)
            {
                startButton.SelectIfMouseInactive();
                SoundManager.PlayMusic("Flower");
            }
        }
    }
}
