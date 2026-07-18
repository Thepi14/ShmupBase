using Main.InputSystem;
using Main.ReplaySystem;
using Main.Sound;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.UI
{
    public class MainMenuUI : PanelBehaviour
    {
        public Button startButton, replayButton, settingsButton, exitButton;

        public RectTransform subPanel;
        public PanelBehaviour settingsPanel;

        protected override void Awake()
        {
            Vars.StartVars();
            base.Awake();
            main = true;

            ReplayManagement.LoadAllReplayFiles();

            InputManager.LockMouse(!Vars.UseMouse);

            SoundManager.PlayMusic("Flower");

            startButton.onClick.AddListener(() => { ReplayManagement.replayMode = false; SceneManager.LoadScene(1); });
            replayButton.onClick.AddListener(() => { ReplayManagement.replayMode = true; ReplayManagement.replayFileName = ReplayManagement.replaysPaths[0]; SceneManager.LoadScene(1); });
            settingsButton.onClick.AddListener(() => { settingsPanel.SetOpenPanel(true); });
            exitButton.onClick.AddListener(() => { Application.Quit(); });

            Vars.PauseGame(false);
        }

        protected void LateUpdate()
        {

        }

        public override void SetOpenPanel(bool open)
        {
            subPanel.gameObject.SetActive(open);

            if (open)
            {
                startButton.Select();
                currentPanel = this;
            }
        }
    }
}
