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
        public static DifficultyPanel instance;

        [SerializeField]
        private Button exitButton;
        [SerializeField]
        private GameObject difficultyButtonsLayout;

        public List<Button> difficultyButtons;

        protected override void Awake()
        {
            base.Awake();
            instance = this;

            for (int i = 0; i < difficultyButtons.Count; i++)
            {
                int j = i;
                difficultyButtons[i].onClick.AddListener(() =>
                {
                    StageManager.currentDifficulty = (Difficulty)j;
                    CharactersPanel.Instance.SetOpenPanel(true);
                });
            }

            exitButton.onClick.AddListener(() =>
            {
                ReturnToPrevious();
            });
        }

        public override void SetOpenPanel(bool open, bool overridePrevious = false)
        {
            base.SetOpenPanel(open);

            if (open)
            {
                difficultyButtons[(byte)Vars.LastDifficulty].SelectIfMouseInactive();
            }
        }
    }
}
