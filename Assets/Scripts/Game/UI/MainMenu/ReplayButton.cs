using Main.ReplaySystem;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.UI
{
    public class ReplayButton : MonoBehaviour
    {
        public int replayID;
        public Replay replayReference;
        public Button button;
        public TMP_Text replayName, replayHighscore, replayDuration, replayDate;

        public void SetupButton(int replayID)
        {
            this.replayID = replayID;
            replayReference = ReplayManagement.LoadReplayFile(ReplayManagement.replaysFilesPaths[replayID]);
            button.onClick.AddListener(() => PlaySelectedReplay());

            replayName.text = replayReference.name;
            replayHighscore.text = Vars.FormatAsScoreString(replayReference.highScore);
            replayDuration.text = replayReference.ReplayDuration().ToString();
            replayDate.text = replayReference.dateTime.ToShortDateString();
        }

        public void PlaySelectedReplay()
        {
            ReplayManagement.replayMode = true;
            ReplayManagement.replayFilePath = ReplayManagement.replaysFilesPaths[replayID];
            SceneManager.LoadScene(1);
        }
    }
}
