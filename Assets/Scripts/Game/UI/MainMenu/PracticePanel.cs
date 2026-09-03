using System.Collections.Generic;
using Main.Stages;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Main.UI
{
    public sealed class PracticePanel : GenericPanelBehaviour
    {
        public static PracticePanel instance;

        [SerializeField]
        private GameObject stageButtonPrefab;

        [SerializeField]
        private RectTransform stageButtonsLayout;
        public LocalizeStringEvent stageNameText, stageDescriptionText;
        [SerializeField]
        private Button exitButton;

        [Space(10f)]
        [SerializeField]
        private List<PracticeStageButton> stageButtons;

        protected override void Awake()
        {
            base.Awake();

            instance = this;

            foreach (GameObject stageObject in StageManager.mainStagesPrefabs)
            {
                PracticeStageButton stageButton = Instantiate(stageButtonPrefab, stageButtonsLayout).GetComponent<PracticeStageButton>();
                stageButton.Setup(stageObject.GetComponent<StageBehaviour>());
                stageButtons.Add(stageButton);
            }

            for (int i = 0; i < stageButtons.Count; i++)
            {
                stageButtons[i].GetButton().navigation = new()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnDown = i + 1 == stageButtons.Count ? exitButton : stageButtons[i + 1].GetButton(),
                    selectOnUp = i == 0 ? exitButton : stageButtons[i - 1].GetButton()
                };
            }

            exitButton.navigation = new()
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = stageButtons[0].GetButton(),
                selectOnUp = stageButtons[^1].GetButton()
            };

            exitButton.onClick.AddListener(() =>
            {
                ReturnToMain();
            });
        }

        public override void SetOpenPanel(bool open)
        {
            base.SetOpenPanel(open);

            if (open)
            {
                stageButtons[0].GetComponent<Button>().SelectIfMouseInactive();
            }
        }
    }
}
