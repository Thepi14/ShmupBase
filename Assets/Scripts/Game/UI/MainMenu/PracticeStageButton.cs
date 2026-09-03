using System.Reflection.Emit;
using Main.InputSystem;
using Main.Stages;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Main.UI
{
    public sealed class PracticeStageButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public StageBehaviour stageBehaviour;

        [SerializeField]
        private LocalizeStringEvent stageNameButtonText;

        private Button returnToStageSelectionButton, stagePracticeButton, bossPracticeButton;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void Setup(StageBehaviour stageBehaviour)
        {
            button = GetComponent<Button>();

            this.stageBehaviour = stageBehaviour;
            stageNameButtonText.StringReference = stageBehaviour.stageName;

            button.onClick.AddListener(() =>
            {
                if (InputManager.playerInput.slow)
                {
                    StageManager.currentGameMode = GameMode.BossPractice;
                }
                else
                {
                    StageManager.currentGameMode = GameMode.StagePractice;
                }

                DifficultyPanel.instance.SetOpenPanel(true);
            });

            /*stagePracticeButton.onClick.AddListener(() =>
            {
                StageManager.currentGameMode = GameMode.StagePractice;
                difficultyPanel.SetOpenPanel(true);
            });

            bossPracticeButton.onClick.AddListener(() =>
            {
                StageManager.currentGameMode = GameMode.BossPractice;
                difficultyPanel.SetOpenPanel(true);
            });

            returnToStageSelectionButton.onClick.AddListener(() => SetOpenOptions(false));*/
        }

        public void SetOpenOptions(bool open)
        {
            button.gameObject.SetActive(!open);

            stagePracticeButton.gameObject.SetActive(open);
            bossPracticeButton.gameObject.SetActive(open);

            if (open)
                stagePracticeButton.SelectIfMouseInactive();
            else
                button.SelectIfMouseInactive();
        }

        public void OnSelect(BaseEventData eventData)
        {
            PracticePanel.instance.stageNameText.GetComponent<TextMeshProUGUI>().enabled = true;
            PracticePanel.instance.stageDescriptionText.GetComponent<TextMeshProUGUI>().enabled = true;

            PracticePanel.instance.stageNameText.StringReference = stageBehaviour.stageName;
            PracticePanel.instance.stageDescriptionText.StringReference = stageBehaviour.stageDescription;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            PracticePanel.instance.stageNameText.GetComponent<TextMeshProUGUI>().enabled = false;
            PracticePanel.instance.stageDescriptionText.GetComponent<TextMeshProUGUI>().enabled = false;
        }

        public Button GetButton() => button;
    }
}
