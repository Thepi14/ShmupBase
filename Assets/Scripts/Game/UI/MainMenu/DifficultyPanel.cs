using System;
using System.Collections.Generic;
using Main.Stages;
using ObjectUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI
{
    public sealed class DifficultyPanel : GenericPanelBehaviour
    {
        public PanelBehaviour charactersPanel;

        [SerializeField]
        private Button exitButton;
        [SerializeField]
        private GameObject difficultyButtonsLayout;
        [SerializeField]
        private List<Button> difficultyButtons;

        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < difficultyButtonsLayout.transform.childCount; i++)
            {
                var button = difficultyButtonsLayout.transform.GetChild(i).GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    StageManager.currentDifficulty = (Difficulty)i;
                    charactersPanel.SetOpenPanel(true);
                });
                difficultyButtons.Add(button);
            }

            difficultyButtonsLayout.transform.GetChild((int)Difficulty.Normal).GetComponent<Button>().SelectIfMouseInactive();

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
                difficultyButtons[(int)Vars.LastDifficulty].SelectIfMouseInactive();
            }
        }
    }
}
