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
        [SerializeField]
        private Button exitButton;
        [SerializeField]
        private GameObject difficultyButtonsLayout;

        public List<Button> difficultyButtons;

        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < difficultyButtons.Count; i++)
            {
                int j = i;
                difficultyButtons[i].onClick.AddListener(() =>
                {
                    StageManager.currentDifficulty = (Difficulty)j;
                    CharactersPanel.Instance.SetOpenPanel(true);
                    Debug.Log("Button: " + (byte)StageManager.currentDifficulty + ", " +(byte)Vars.LastDifficulty);
                });
            }

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
                difficultyButtons[(byte)Vars.LastDifficulty].SelectIfMouseInactive();
            }
        }
    }
}
